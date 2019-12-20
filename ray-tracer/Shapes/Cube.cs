using System;

namespace ray_tracer.Shapes
{
    public class Cube : AbstractShape
    {
        private static Bounds CubeBox { get; } = new Bounds {PMin = Helper.CreatePoint(-1, -1, -1), PMax = Helper.CreatePoint(1, 1, 1)};
        public override Bounds Box { get; } = CubeBox;

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            Helper.CheckAxis(origin.X, direction.X, out var xtMin, out var xtMax);
            Helper.CheckAxis(origin.Y, direction.Y, out var ytMin, out var ytMax);
            if (xtMin > ytMax || ytMin > xtMax)
            {
                return;
            }

            Helper.CheckAxis(origin.Z, direction.Z, out var ztMin, out var ztMax);

            var tMin = Math.Max(xtMin, Math.Max(ytMin, ztMin));
            var tMax = Math.Min(xtMax, Math.Min(ytMax, ztMax));
            if (tMin > tMax)
            {
                return;
            }

            intersections.Add(new Intersection(tMin, this));
            intersections.Add(new Intersection(tMax, this));
        }

        private static readonly Tuple NegNormX = Helper.CreateVector(-1, 0, 0);
        private static readonly Tuple NormX = Helper.CreateVector(1, 0, 0);
        private static readonly Tuple NegNormY = Helper.CreateVector(0, -1, 0);
        private static readonly Tuple NormY = Helper.CreateVector(0, 1, 0);
        private static readonly Tuple NegNormZ = Helper.CreateVector(0, 0, -1);
        private static readonly Tuple NormZ = Helper.CreateVector(0, 0, 1);

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit = null)
        {
            var absX = Math.Abs(worldPoint.X);
            var absY = Math.Abs(worldPoint.Y);
            var absZ = Math.Abs(worldPoint.Z);
            var maxc = Math.Max(absX, Math.Max(absY, absZ));
            if (maxc == absX)
            {
                return worldPoint.X > 0 ? NormX : NegNormX;
            }

            if (maxc == absY)
            {
                return worldPoint.Y > 0 ? NormY : NegNormY;
            }

            return worldPoint.Z > 0 ? NormZ : NegNormZ;
        }
    }
}