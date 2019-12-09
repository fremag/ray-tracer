using System;
using System.Linq;
using System.Numerics;

namespace ray_tracer
{
    public class Bounds
    {
        public Tuple PMin { get; set; } = Helper.CreatePoint(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
        public Tuple PMax { get; set; } = Helper.CreatePoint(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

        public void Init(params Tuple[] points)
        {
            var xMin = points.Select(p => p.X).Min();
            var yMin = points.Select(p => p.Y).Min();
            var zMin = points.Select(p => p.Z).Min();
            
            var xMax = points.Select(p => p.X).Max();
            var yMax = points.Select(p => p.Y).Max();
            var zMax = points.Select(p => p.Z).Max();

            PMin = Helper.CreatePoint(xMin, yMin, zMin);
            PMax = Helper.CreatePoint(xMax, yMax, zMax);
        }
        
        public void Add(Tuple p)
        {
            var xMin = Math.Min(PMin.X, p.X);
            var yMin = Math.Min(PMin.Y, p.Y);
            var zMin = Math.Min(PMin.Z, p.Z);
            
            var xMax = Math.Max(PMax.X, p.X);
            var yMax = Math.Max(PMax.Y, p.Y);
            var zMax = Math.Max(PMax.Z, p.Z);

            PMin = Helper.CreatePoint(xMin, yMin, zMin);
            PMax = Helper.CreatePoint(xMax, yMax, zMax);
        }
        
        public bool IntersectLocal(ref Vector4 origin, ref Vector4 direction)
        {
            Helper.CheckAxis(origin.X, direction.X, out var xtMin, out var xtMax, PMin.X, PMax.X);
            Helper.CheckAxis(origin.Y, direction.Y, out var ytMin, out var ytMax, PMin.Y, PMax.Y);
            Helper.CheckAxis(origin.Z, direction.Z, out var ztMin, out var ztMax, PMin.Z, PMax.Z);
            var tMin = Math.Max(xtMin, Math.Max(ytMin, ztMin));
            var tMax = Math.Min(xtMax, Math.Min(ytMax, ztMax));
            return tMin <= tMax;
        }        
    }
}