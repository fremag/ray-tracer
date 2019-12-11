using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ray_tracer
{
    public class World
    {
        public List<IShape> Shapes { get; } = new List<IShape>();
        public List<PointLight> Lights { get; } = new List<PointLight>();

        public Intersections Intersect(ref Vector4 origin, ref Vector4 direction)
        {
            var intersections = new Intersections();
            foreach (var shape in Shapes)
            {
                var inters = shape.Intersect(ref origin, ref direction);
                foreach (var inter in inters)
                {
                    intersections.Add(inter);
                }   
            }
            intersections.Sort();
            return intersections;
        }

        public Color ShadeHit(IntersectionData intersectionData, int remaining = 5)
        {
            var color = Color.Black;
            foreach (var light in Lights)
            {
                var isShadowed = IsShadowed(intersectionData.OverPoint, light);
                var surface = intersectionData.Object.Material.Lighting(light, intersectionData.Object, intersectionData.OverPoint, intersectionData.EyeVector, intersectionData.Normal, isShadowed);
                var reflected = ReflectedColor(intersectionData, remaining);
                var refracted = RefractedColor(intersectionData, remaining);

                var material = intersectionData.Object.Material;
                if (material.Reflective > 0 && material.Transparency > 0)
                {
                    var reflectance = intersectionData.Schlick();
                    color += surface + reflected * reflectance + refracted * (1 - reflectance);
                }
                else
                {
                    color += surface + reflected + refracted;
                }
            }

            return color;
        }

        public Color ColorAt(Ray ray, int remaining = 5)
        {
            var intersections = Intersect(ref ray.Origin.vector, ref ray.Direction.vector);
            var hit = intersections.Hit();
            if (hit == null)
            {
                return Color.Black;
            }

            var intersectionData = hit.Compute(ray, intersections);
            var color = ShadeHit(intersectionData, remaining);
            return color;
        }

        public bool IsShadowed(Tuple point, PointLight light)
        {
            var v = light.Position - point;
            var distance = v.Magnitude;
            var direction = v.Normalize();
            var intersections = Intersect(ref point.vector, ref direction.vector);
            var h = intersections.Hit();
            if (h != null && h.T < distance)
            {
                return true;
            }

            return false;
        }

        public bool IsShadowed(Tuple point)
        {
            return Lights.Any(light => IsShadowed(point, light));
        }

        public Color ReflectedColor(IntersectionData intersectionData, int remaining = 5)
        {
            var materialReflective = intersectionData.Object.Material.Reflective;
            if (remaining == 0 || materialReflective < double.Epsilon)
            {
                return Color.Black;
            }

            var reflectRay = Helper.Ray(intersectionData.OverPoint, intersectionData.ReflectionVector);
            var color = ColorAt(reflectRay, remaining - 1);
            return color * materialReflective;
        }

        public Color RefractedColor(IntersectionData intersectionData, int remaining = 5)
        {
            if (intersectionData.Object.Material.Transparency <= double.Epsilon || remaining == 0)
            {
                return Color.Black;
            }

            double nRatio = intersectionData.N1 / intersectionData.N2;
            // cos(theta_i) is the same as the dot product of the two vectors
            double cosI = intersectionData.EyeVector.DotProduct(intersectionData.Normal);
            // Find sin(theta_t)^2 via trigonometric identity
            double sin2t = nRatio * nRatio * (1 - cosI * cosI);
            if (sin2t > 1)
            {
                return Color.Black;
            }
            
            // Find cos(theta_t) via trigonometric identity
            var cost = Math.Sqrt(1.0 - sin2t);
            // Compute the direction of the refracted ray
            var d = (nRatio * cosI - cost);
            var normal = intersectionData.Normal * d;
            var eyeVector = intersectionData.EyeVector * nRatio;
            var direction = normal - eyeVector;
            // Create the refracted ray
            var refractRay = Helper.Ray(intersectionData.UnderPoint, direction);
            // Find the color of the refracted ray, making sure to multiply
            // by the transparency value to account for any opacity
            var color = ColorAt(refractRay, remaining - 1) * intersectionData.Object.Material.Transparency;            
           
            return color;
        }

        public Intersections Intersect(Ray ray)
        {
            return Intersect(ref ray.Origin.vector, ref ray.Direction.vector);
        }
    }
}