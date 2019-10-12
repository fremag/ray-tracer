using System;

namespace ray_tracer.Shapes
{
    public class Sphere : AbstractShape
    {
        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, -1, -1), PMax = Helper.CreatePoint(1, 1, 1)};

        public override Intersections IntersectLocal(Ray ray)
        {
            var sphereToRay = ray.Origin - Helper.CreatePoint(0, 0, 0);
            var a = ray.Direction.DotProduct(ray.Direction);
            var b = 2 * ray.Direction.DotProduct(sphereToRay);
            var c = sphereToRay.DotProduct(sphereToRay) - 1;
            var discriminant = b * b - 4 * a * c;

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

        public override Tuple NormalAtLocal(Tuple objectPoint, Intersection hit=null)
        {
            var objectNormal = objectPoint - Helper.CreatePoint(0, 0, 0);
            return objectNormal;
        }
    }
}