using System;

namespace ray_tracer
{
    public class Bounds
    {
        public Tuple PMin { get; set; }
        public Tuple PMax { get; set; }
        
        public bool IntersectLocal(Ray ray)
        {
            Helper.CheckAxis(ray.Origin.X, ray.Direction.X, out var xtMin, out var xtMax, PMin.X, PMax.X);
            Helper.CheckAxis(ray.Origin.Y, ray.Direction.Y, out var ytMin, out var ytMax, PMin.Y, PMax.Y);
            Helper.CheckAxis(ray.Origin.Z, ray.Direction.Z, out var ztMin, out var ztMax, PMin.Z, PMax.Z);

            var tMin = Math.Max(xtMin, Math.Max(ytMin, ztMin));
            var tMax = Math.Min(xtMax, Math.Min(ytMax, ztMax));
            return tMin <= tMax;
        }        
    }
}