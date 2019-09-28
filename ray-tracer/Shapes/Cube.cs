using System;

namespace ray_tracer.Shapes
{
    public class Cube : AbstractShape
    {
        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, -1, -1), PMax = Helper.CreatePoint(1, 1, 1)};
        
        public void CheckAxis(double origin, double direction, out double tMin, out double tMax)
        {
            var tMinNumerator = (-1 - origin);
            var tMaxNumerator = (1 - origin);
            
            if (Math.Abs(direction) >= double.Epsilon)
            {
                tMin = tMinNumerator / direction;
                tMax = tMaxNumerator / direction;
            }
            else
            {
                tMin = tMinNumerator * double.PositiveInfinity;
                tMax = tMaxNumerator * double.PositiveInfinity;
            }

            if (tMin > tMax)
            {
                var t = tMin;
                tMin = tMax;
                tMax = t;
            }
        }

        public override Intersections IntersectLocal(Ray ray)
        {
            CheckAxis(ray.Origin.X, ray.Direction.X, out var xtMin, out var xtMax);
            CheckAxis(ray.Origin.Y, ray.Direction.Y, out var ytMin, out var ytMax);
            CheckAxis(ray.Origin.Z, ray.Direction.Z, out var ztMin, out var ztMax);

            var tMin = Math.Max(xtMin, Math.Max(ytMin, ztMin));
            var tMax = Math.Min(xtMax, Math.Min(ytMax, ztMax));
            if (tMin > tMax)
            {
                return new Intersections();
            }
            return new Intersections{new Intersection(tMin, this), new Intersection(tMax, this)};
        }

        public override Tuple NormalAtLocal(Tuple worldPoint)
        {
            var maxc = Math.Max(Math.Abs(worldPoint.X), Math.Max(Math.Abs(worldPoint.Y), Math.Abs(worldPoint.Z)));
            if (maxc == Math.Abs(worldPoint.X)) {
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