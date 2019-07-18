using System;

namespace ray_tracer
{
    public class Sphere
    {
        public Matrix Transform { get; set; } = Helper.CreateIdentity();
        
        public Intersections Intersect(Ray ray)
        {
            var transformedRay = ray.Transform(Transform.Inverse());
            var sphereToRay = transformedRay.Origin - Helper.CreatePoint(0, 0, 0);
            var a = transformedRay.Direction.DotProduct(transformedRay.Direction);
            var b = 2 * transformedRay.Direction.DotProduct(sphereToRay);
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