using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public bool IntersectLocal(ref Tuple origin, ref Tuple direction)
        {
            Helper.CheckAxis(origin.X, direction.X, out var xtMin, out var xtMax, PMin.X, PMax.X);
            Helper.CheckAxis(origin.Y, direction.Y, out var ytMin, out var ytMax, PMin.Y, PMax.Y);
            if (xtMin > ytMax || ytMin > xtMax)
            {
                return false;
            }
            Helper.CheckAxis(origin.Z, direction.Z, out var ztMin, out var ztMax, PMin.Z, PMax.Z);

            var tMin = Math.Max(xtMin, Math.Max(ytMin, ztMin));
            var tMax = Math.Min(xtMax, Math.Min(ytMax, ztMax));
            return tMin <= tMax;
        }

        public IEnumerable<Bounds> Split()
        {
            var dx = Math.Abs(PMax.X - PMin.X);
            var dy = Math.Abs(PMax.Y - PMin.Y);
            var dz = Math.Abs(PMax.Z - PMin.Z);

            var max = Math.Max(dx, Math.Max(dy, dz));

            var x0 = PMin.X;
            var y0 = PMin.Y;
            var z0 = PMin.Z;
            var x1 = PMax.X;
            var y1 = PMax.Y;
            var z1 = PMax.Z;
            
            if (max == dx)
            {
                x0 = x0 + dx / 2;
                x1 = x0;
            } 
            else if (max == dy) 
            {
                y0 = y0 + dy / 2;
                y1 = y0;
            }
            else
            {
                z0 = z0 + dz / 2;
                z1 = z0;

            }

            var midMin = Helper.CreatePoint(x0, y0, z0);
            var midMax = Helper.CreatePoint(x1, y1, z1);
            
            var left = new Bounds()
            {
                PMin = PMin, 
                PMax = midMax
            };
            var right = new Bounds
            {
                PMin = midMin,
                PMax = PMax
            };
            
            yield return left;
            yield return right;
        }

        public bool Contains(ref Tuple point)
        {
            return (point.X >= PMin.X && point.X <= PMax.X)
                   && (point.Y >= PMin.Y && point.Y <= PMax.Y)
                   && (point.Z >= PMin.Z && point.Z <= PMax.Z);

        }
        
        public bool Contains(Bounds box)
        {
            var min = box.PMin;
            if (!Contains(ref min))
            {
                return false;
            } 

            var max = box.PMax;
            return Contains(ref max);
        }

        public Bounds Transform(Matrix matrix)
        {
            var p1 = PMin;
            var p2 = Helper.CreatePoint(PMin.X, PMin.Y, PMax.Z);
            var p3 = Helper.CreatePoint(PMin.X, PMax.Y, PMin.Z);
            var p4 = Helper.CreatePoint(PMin.X, PMax.Y, PMax.Z);
            var p5 = Helper.CreatePoint(PMax.X, PMin.Y, PMin.Z);
            var p6 = Helper.CreatePoint(PMax.X, PMin.Y, PMax.Z);
            var p7 = Helper.CreatePoint(PMax.X, PMax.Y, PMin.Z);
            var p8 = PMax;
            var transformP1 = matrix * p1;
            var transformP2 = matrix * p2;
            var transformP3 = matrix * p3;
            var transformP4 = matrix * p4;
            var transformP5 = matrix * p5;
            var transformP6 = matrix * p6;
            var transformP7 = matrix * p7;
            var transformP8 = matrix * p8;
            var box = new Bounds();
            box.Add(transformP1);
            box.Add(transformP2);
            box.Add(transformP3);
            box.Add(transformP4);
            box.Add(transformP5);
            box.Add(transformP6);
            box.Add(transformP7);
            box.Add(transformP8);
            return box;
        }
    }
}