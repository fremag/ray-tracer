using System.Collections.Generic;
using System.Linq;

namespace ray_tracer
{
    public class World
    {
        public List<Sphere> Spheres { get;  } = new List<Sphere>();
        public List<PointLight> Lights { get;  } = new List<PointLight>();

        public Intersections Intersect(Ray ray)
        {
            var intersections = Spheres.SelectMany(sphere => sphere.Intersect(ray));
            return new Intersections(intersections);
        }
        
        public Color ShadeHit(IntersectionData intersectionData)
        {
            var color = Color.Black;
            foreach (var light in Lights)
            {
                var c = intersectionData.Object.Material.Lighting(light, intersectionData.Point,
                    intersectionData.EyeVector, intersectionData.Normal);
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
    }
}