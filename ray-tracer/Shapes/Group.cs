using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ray_tracer.Shapes
{
    public class Group : AbstractShape
    {
        protected List<IShape> Shapes { get; } = new List<IShape>();
        private Bounds box;
        private readonly ConcurrentDictionary<long, bool> cacheContains = new ConcurrentDictionary<long, bool>();
        public IShape this[int i] => Shapes[i];
        public int Count => Shapes.Count;
        
        public override Bounds Box
        {
            get
            {
                if (box == null)
                {
                    ComputeBox();
                }

                return box;
            }
        }

        private void ComputeBox()
        {
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double minZ = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;
            double maxZ = double.NegativeInfinity;

            foreach (var shape in Shapes)
            {
                var b = shape.Box;
                var p1 = b.PMin;
                var p2 = Helper.CreatePoint(b.PMin.X, b.PMin.Y, b.PMax.Z);
                var p3 = Helper.CreatePoint(b.PMin.X, b.PMax.Y, b.PMin.Z);
                var p4 = Helper.CreatePoint(b.PMin.X, b.PMax.Y, b.PMax.Z);
                var p5 = Helper.CreatePoint(b.PMax.X, b.PMin.Y, b.PMin.Z);
                var p6 = Helper.CreatePoint(b.PMax.X, b.PMin.Y, b.PMax.Z);
                var p7 = Helper.CreatePoint(b.PMax.X, b.PMax.Y, b.PMin.Z);
                var p8 = b.PMax;
                var transformP1 = shape.Transform * p1;
                var transformP2 = shape.Transform * p2;
                var transformP3 = shape.Transform * p3;
                var transformP4 = shape.Transform * p4;
                var transformP5 = shape.Transform * p5;
                var transformP6 = shape.Transform * p6;
                var transformP7 = shape.Transform * p7;
                var transformP8 = shape.Transform * p8;
                var points = new[] {transformP1, transformP2, transformP3, transformP4, transformP5, transformP6, transformP7, transformP8};
                minX = Math.Min(minX, points.Select(p => p.X).Min());
                minY = Math.Min(minY, points.Select(p => p.Y).Min());
                minZ = Math.Min(minZ, points.Select(p => p.Z).Min());
                maxX = Math.Max(maxX, points.Select(p => p.X).Max());
                maxY = Math.Max(maxY, points.Select(p => p.Y).Max());
                maxZ = Math.Max(maxZ, points.Select(p => p.Z).Max());
            }

            box = new Bounds {PMin = Helper.CreatePoint(minX, minY, minZ), PMax = Helper.CreatePoint(maxX, maxY, maxZ)};
        }

        public void Add(params IShape[] shapes)
        {
            box = null;
            foreach (var shape in shapes)
            {
                Shapes.Add(shape);
                shape.Parent = this;
            }
        }

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            if (!Box.IntersectLocal(ref origin, ref direction))
            {
                return;
            }

            foreach (var shape in Shapes)
            {
                shape.Intersect(ref origin, ref direction, intersections);
            }
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null)
        {
            throw new InvalidOperationException();
        }
        
        public override bool Contains(IShape shape)
        {
            if (cacheContains.TryGetValue(shape.Id, out var result))
            {
                return result;
            }
            if (ReferenceEquals(shape, this))
            {
                cacheContains[shape.Id] = true;
                return true;
            }

            foreach (var s in Shapes)
            {
                if (s.Contains(shape))
                {
                    cacheContains[shape.Id] = true;
                    return true;
                }
            }

            cacheContains[shape.Id] = false;
            return false;
        }

    }
}