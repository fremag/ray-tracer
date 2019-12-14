using System;

namespace ray_tracer.Shapes
{
    public class Sphere : AbstractShape
    {
        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, -1, -1), PMax = Helper.CreatePoint(1, 1, 1)};

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            var a = 2*(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
            var b = 2 * (direction.X * origin.X + direction.Y * origin.Y + direction.Z * origin.Z); 
            var c = origin.X * origin.X + origin.Y * origin.Y + origin.Z * origin.Z -1;
            var discriminant = b * b - 2* a * c;

            if (discriminant < 0)
            {
                return;
            }

            var x1 = -b/a;
            var sqrtDet = Math.Sqrt(discriminant)/a;
            var t1 = x1 - sqrtDet;
            var t2 = x1 + sqrtDet;

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