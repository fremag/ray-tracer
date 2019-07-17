using System;

namespace ray_tracer
{
    public class Sphere
    {
        public Intersections Intersect(Ray r)
        {
            var sphereToRay = r.Origin - Helper.CreatePoint(0, 0, 0);
            var a = r.Direction.DotProduct(r.Direction);
            var b = 2 * r.Direction.DotProduct(sphereToRay);
            var c = sphereToRay.DotProduct(sphereToRay) - 1;
            var discriminant = b * b -4 * a * c;

            if (discriminant < 0)
            {
                return Helper.Intersections();
            }

            var t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            var t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            
            return Helper.Intersections(
                new Intersection(t1, this), 
                new Intersection(t2, this)
            );
        }
    }
}