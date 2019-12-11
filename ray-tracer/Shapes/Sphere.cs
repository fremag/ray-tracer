using System;
using System.Numerics;

namespace ray_tracer.Shapes
{
    public class Sphere : AbstractShape
    {
        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, -1, -1), PMax = Helper.CreatePoint(1, 1, 1)};

        public override Intersections IntersectLocal(Ray ray)
        {
            throw new InvalidOperationException();
        }

        public override Intersections IntersectLocal(ref Vector4 origin, ref Vector4 direction)
        {
            var sphereToRay = new Vector4(origin.X, origin.Y, origin.Z, 0);
            var a = Vector4.Dot(direction, direction);
            var b = 2 * Vector4.Dot(direction, sphereToRay);
            var c = Vector4.Dot(sphereToRay, sphereToRay) - 1;
            var discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                return Helper.Intersections();
            }

            var sqrtDet = Math.Sqrt(discriminant) / (2 * a);
            var d = -b / (2 * a);
            var t1 = d - sqrtDet;
            var t2 = d + sqrtDet;

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