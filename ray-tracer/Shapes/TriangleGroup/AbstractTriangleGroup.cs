using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ray_tracer.Shapes.TriangleGroup
{
    public abstract class AbstractTriangleGroup : AbstractShape
    {
        private readonly ConcurrentDictionary<long, bool> cacheContains = new ConcurrentDictionary<long, bool>();
        private AbstractTriangleGroup leftGroup;
        private AbstractTriangleGroup rightGroup;
        private Material material;
        private Bounds box;
        public IShape this[int i] => Triangles[i];
        public int Count => Triangles.Count;
        protected List<Triangle> Triangles { get; } = new List<Triangle>();

        protected abstract void IntersectTriangles(ref Tuple origin, ref Tuple direction, Intersections intersections);
        protected abstract AbstractTriangleGroup GetSubGroup(List<Triangle> triangles, bool keepParent);
        
        public AbstractTriangleGroup()
        {
        }

        public AbstractTriangleGroup(Group triangleGroup)
        {
            Triangles.Capacity = triangleGroup.Count;
            for (int i = 0; i < triangleGroup.Count; i++)
            {
                var shape = triangleGroup[i];
                if (shape is Triangle tri)
                {
                    Add(tri);
                }
            }
        }

        public AbstractTriangleGroup(List<Triangle> triangles, bool keepParent)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                var triangle = triangles[i];
                Add(triangle, keepParent);
            }
        }

        public override Material Material
        {
            get => material;
            set {
                material = value;
                foreach (var shape in Triangles)
                {
                    shape.Material = material;
                }

                if (leftGroup != null)
                {
                    leftGroup.Material = material;
                }
                if (rightGroup != null)
                {
                    rightGroup.Material = material;
                }
            }
        }

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

        public void Add(Triangle triangle, bool keepParent=false)
        {
            Triangles.Add(triangle);
            if (!keepParent)
            {
                triangle.Parent = this;
            }
        }

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            if (!Box.IntersectLocal(ref origin, ref direction))
            {
                return;
            }

            leftGroup?.IntersectLocal(ref origin, ref direction, intersections);
            rightGroup?.IntersectLocal(ref origin, ref direction, intersections);
            if (!Triangles.Any())
            {
                return;
            }

            IntersectTriangles(ref origin, ref direction, intersections);
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit = null)
        {
            throw new InvalidOperationException();
        }

        public override bool Contains(IShape shape)
        {
            if (leftGroup != null && leftGroup.Contains(shape))
            {
                return true;
            }
            if (rightGroup != null && rightGroup.Contains(shape))
            {
                return true;
            }

            if (cacheContains.TryGetValue(shape.Id, out var result))
            {
                return result;
            }
            if (ReferenceEquals(shape, this))
            {
                cacheContains[shape.Id] = true;
                return true;
            }

            foreach (var s in Triangles)
            {
                if (s.Id == shape.Id)
                {
                    cacheContains[shape.Id] = true;
                    return true;
                }
            }

            cacheContains[shape.Id] = false;
            return false;
        }

        public void Partition(IList<Triangle> left, IList<Triangle> right)
        {
            var split = Box.Split();
            var sides = split.GetEnumerator();
            sides.MoveNext();
            var leftSide = sides.Current;
            sides.MoveNext();
            var rightSide = sides.Current;
            List<Triangle> remainingTriangles = new List<Triangle>();
            foreach (var triangle in Triangles)
            {
                if (leftSide.Contains(triangle.TransformedBox))
                {
                    left.Add(triangle);
                }
                else if (rightSide.Contains(triangle.TransformedBox))
                {
                    right.Add(triangle);
                }
                else
                {
                    remainingTriangles.Add(triangle);
                }
            }
            
            Triangles.Clear();
            Triangles.AddRange(remainingTriangles);
        }

        public override IShape Divide(int threshold)
        {
            if (threshold <= Triangles.Count)
            {
                var left = new List<Triangle>();
                var right = new List<Triangle>();
                Partition(left, right);
                if (left.Count > 0)
                {
                    leftGroup = GetSubGroup(left, true);
                    leftGroup.Divide(threshold);
                }

                if (right.Count > 0)
                {
                    rightGroup = GetSubGroup(right, true);
                    rightGroup.Divide(threshold);
                }
            }
            return this;
        }

        private void ComputeBox()
        {
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double minZ = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;
            double maxZ = double.NegativeInfinity;

            foreach (var shape in Triangles)
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
    }
}