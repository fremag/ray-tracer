#define OPTIM_SHADOW
using System;
using System.Collections.Generic;
using System.Linq;
using ray_tracer.Lights;

namespace ray_tracer
{
    public class World
    {
        public List<IShape> Shapes { get; } = new List<IShape>();
        public List<ILight> Lights { get; } = new List<ILight>();

        public void Intersect(Ray ray, Intersections intersections)
        {
            for (var i = 0; i < Shapes.Count; i++)
            {
                var shape = Shapes[i];
                shape.Intersect(ref ray.Origin, ref ray.Direction, intersections);
            }

            intersections.Sort();
        }

        public unsafe Color ShadeHit(IntersectionData intersectionData, int remaining = 5)
        {
            double* x = stackalloc double[ILight.MAX_SAMPLE];
            double* y = stackalloc double[ILight.MAX_SAMPLE];
            double* z = stackalloc double[ILight.MAX_SAMPLE];
            
            var overPoint = intersectionData.OverPoint;
            var eyeVector = intersectionData.EyeVector;
            var normal = intersectionData.Normal;
            var material = intersectionData.Object.Material;
            var shapeColor = material.Pattern.GetColorAtShape(intersectionData.Object, ref overPoint);

            var surface = Color.Black;
            for (var i = 0; i < Lights.Count; i++)
            {
                var light = Lights[i];
                
                int nbSamples = light.GetPositions(x, y, z);
                double lightIntensity = 0;
                var lightColor = Color.Black;
                for (int j = 0; j < nbSamples; j++)
                {
                    var sampleLightColor = light.GetIntensityAt(x[j], y[j], z[j], ref overPoint);
                    lightColor += sampleLightColor;
                    if (lightColor.Equals(Color.Black))
                    {
                        continue;
                    }
                    bool isShadowed = IsShadowed(overPoint, x[j], y[j], z[j]);
                    lightIntensity += isShadowed ? 0 : 1;
                }

                lightIntensity /= nbSamples;
                lightColor /= nbSamples;
                surface += material.Lighting(nbSamples, x, y, z, ref overPoint, ref eyeVector, ref normal, lightIntensity, shapeColor, lightColor);
            }

            var reflected = ReflectedColor(intersectionData, remaining);
            var refracted = RefractedColor(intersectionData, remaining);

            var color = surface + reflected + refracted;
            return color;
        }

        public Color ColorAt(Ray ray, int remaining = 5)
        {
            var intersections = new Intersections();
            Intersect(ray, intersections);
            var hit = intersections.Hit();
            if (hit == null)
            {
                return Color.Black;
            }

            var intersectionData = hit.Compute(ray, intersections);
            var color = ShadeHit(intersectionData, remaining);
            return color;
        }
#if OPTIM_SHADOW
        public bool IsShadowed(Tuple point, ILight light)
        {
            return IsShadowed(point, light.Position);
        }

        public bool IsShadowed(Tuple point, Tuple position)
        {
            return IsShadowed(point, position.X, position.Y, position.Z);
        }
        
        public bool IsShadowed(Tuple point, double x, double y, double z)
        {
            var v = Helper.CreateVector(x - point.X, y - point.Y, z - point.Z) ;
            var distance = v.Magnitude;
            var direction = v.Normalize();
            var intersections = new Intersections();

            for (var i = 0; i < Shapes.Count; i++)
            {
                var shape = Shapes[i];
                if (!shape.HasShadow)
                {
                    continue;
                }
                intersections.Clear();
                shape.Intersect(ref point, ref direction, intersections);
                for (var j = 0; j < intersections.Count; j++)
                {
                    var intersection = intersections[j];
                    if (intersection.T >= 0 && intersection.T < distance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
#else
        public bool IsShadowed(Tuple point, ILight light)
        {
            var v = light.Position - point;
            var distance = v.Magnitude;
            var direction = v.Normalize();
            var r = Helper.Ray(point, direction);
            var intersections = new Intersections();
            Intersect(r, intersections);
            var h = intersections.Hit();
            if (h != null && h.T < distance)
            {
                return true;
            }

            return false;
        }
#endif

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
    }
}