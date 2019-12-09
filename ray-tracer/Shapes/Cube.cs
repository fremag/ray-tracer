using System;
using System.Numerics;

namespace ray_tracer.Shapes
{
    public class Cube : AbstractShape
    {
        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, -1, -1), PMax = Helper.CreatePoint(1, 1, 1)};

        public override Intersections IntersectLocal(ref Vector4 origin, ref Vector4 direction)
        {
            Helper.CheckAxis(origin.X, direction.X, out var xtMin, out var xtMax);
            Helper.CheckAxis(origin.Y, direction.Y, out var ytMin, out var ytMax);
            if (xtMin > ytMax || ytMin > xtMax)
            {
                return Intersections.Empty;
            }
            Helper.CheckAxis(origin.Z, direction.Z, out var ztMin, out var ztMax);

            var tMin = Math.Max(xtMin, Math.Max(ytMin, ztMin));
            var tMax = Math.Min(xtMax, Math.Min(ytMax, ztMax));
            if (tMin > tMax)
            {
                return Intersections.Empty;
            }
            return new Intersections{new Intersection(tMin, this), new Intersection(tMax, this)};
        }

        public override Intersections IntersectLocal(Ray ray)
        {
            Helper.CheckAxis(ray.Origin.X, ray.Direction.X, out var xtMin, out var xtMax);
            Helper.CheckAxis(ray.Origin.Y, ray.Direction.Y, out var ytMin, out var ytMax);
            if (xtMin > ytMax || ytMin > xtMax)
            {
                return Intersections.Empty;
            }
            Helper.CheckAxis(ray.Origin.Z, ray.Direction.Z, out var ztMin, out var ztMax);

            var tMin = Math.Max(xtMin, Math.Max(ytMin, ztMin));
            var tMax = Math.Min(xtMax, Math.Min(ytMax, ztMax));
            if (tMin > tMax)
            {
                return Intersections.Empty;
            }
            return new Intersections{new Intersection(tMin, this), new Intersection(tMax, this)};
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null)
        {
            var maxc = Math.Max(Math.Abs(worldPoint.X), Math.Max(Math.Abs(worldPoint.Y), Math.Abs(worldPoint.Z)));
            if (maxc == Math.Abs(worldPoint.X)) 
            {
                return Helper.CreateVector(worldPoint.X, 0, 0);
            }
            if (maxc == Math.Abs(worldPoint.Y))
            {
                return Helper.CreateVector(0, worldPoint.Y, 0);
            }

            return Helper.CreateVector(0, 0, worldPoint.Z);
        }
    }
}