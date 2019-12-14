using System;

namespace ray_tracer.Shapes
{
    public class Sphere : AbstractShape
    {
        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, -1, -1), PMax = Helper.CreatePoint(1, 1, 1)};

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            var sphereToRay = origin - Helper.CreatePoint(0, 0, 0);
            var a = direction.DotProduct(direction);
            var b = 2 * direction.DotProduct(sphereToRay);
            var c = sphereToRay.DotProduct(sphereToRay) - 1;
            var discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                return;
            }

            var t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            var t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);

            intersections.Add(new Intersection(t1, this));
            intersections.Add(new Intersection(t2, this));
        }

        public override Tuple NormalAtLocal(Tuple objectPoint, Intersection hit=null)
        {
            var objectNormal = objectPoint - Helper.CreatePoint(0, 0, 0);
            return objectNormal;
        }
    }
}