using System.Collections.Generic;
using System.Linq;

namespace ray_tracer
{
    public class World
    {
        public List<IShape> Shapes { get; } = new List<IShape>();
        public List<PointLight> Lights { get; } = new List<PointLight>();

        public Intersections Intersect(Ray ray)
        {
            var intersections = Shapes.SelectMany(sphere => sphere.Intersect(ray));
            return new Intersections(intersections);
        }

        public Color ShadeHit(IntersectionData intersectionData)
        {
            var color = Color.Black;
            foreach (var light in Lights)
            {
                var isShadowed = IsShadowed(intersectionData.OverPoint, light);
                var c = intersectionData.Object.Material.Lighting(light, intersectionData.OverPoint, intersectionData.EyeVector, intersectionData.Normal, isShadowed);
                color += c;
            }

            return color;
        }

        public Color ColorAt(Ray ray)
        {
            var intersections = Intersect(ray);
            var hit = intersections.Hit();
            if (hit == null)
            {
                return Color.Black;
            }

            var intersectionData = hit.Compute(ray);
            var color = ShadeHit(intersectionData);
            return color;
        }

        public bool IsShadowed(Tuple point, PointLight light)
        {
            var v = light.Position - point;
            var distance = v.Magnitude;
            var direction = v.Normalize();
            var r = Helper.Ray(point, direction);
            var intersections = Intersect(r);
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
    }
}