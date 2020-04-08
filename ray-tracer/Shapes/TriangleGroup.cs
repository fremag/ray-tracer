#define OPTIM
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ray_tracer.Shapes
{
    public class TriangleGroup : AbstractShape
    {
        private readonly ConcurrentDictionary<long, bool> cacheContains = new ConcurrentDictionary<long, bool>();
        private TriangleGroup leftGroup;
        private TriangleGroup rightGroup;
        
        private Bounds box = null;
        private bool cached = false;
        
        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            if (!Box.IntersectLocal(ref origin, ref direction))
            {
                return;
            }

            leftGroup?.Intersect(ref origin, ref direction, intersections);
            rightGroup?.Intersect(ref origin, ref direction, intersections);
#if OPTIM
            if (!cached)
            {
                BuildCaches();
                cached = true;
            }
            IntersectAll(ref origin, ref direction, intersections);
#else
            for (int i = 0; i < Triangles.Count; i++)
            {
                var tri = Triangles[i];
                tri.Intersect(ref origin, ref direction, intersections);
            }
#endif            
        }

        float[] p1_X; 
        float[] p1_Y; 
        float[] p1_Z; 
        float[] e1_X; 
        float[] e1_Y; 
        float[] e1_Z; 
        float[] e2_X; 
        float[] e2_Y; 
        float[] e2_Z;
        private void BuildCaches()
        {
            int size = Triangles.Count + Size - Triangles.Count % Size; 
            p1_X = new float[size]; 
            p1_Y = new float[size]; 
            p1_Z = new float[size]; 
            
            e1_X = new float[size]; 
            e1_Y = new float[size]; 
            e1_Z = new float[size];
            
            e2_X = new float[size]; 
            e2_Y = new float[size]; 
            e2_Z = new float[size];
            
            for (var i = 0; i < Triangles.Count; i++)
            {
                var tri = Triangles[i];
                p1_X[i] = (float)tri.P1.X;
                p1_Y[i] = (float)tri.P1.Y;
                p1_Z[i] = (float)tri.P1.Z;
                
                e1_X[i] = (float)tri.E1.X;
                e1_Y[i] = (float)tri.E1.Y;
                e1_Z[i] = (float)tri.E1.Z;
                
                e2_X[i] = (float)tri.E2.X;
                e2_Y[i] = (float)tri.E2.Y;
                e2_Z[i] = (float)tri.E2.Z;
            }
        }

        static readonly int Size = Vector<float>.Count;

        public unsafe void IntersectAll(ref Tuple origin, ref Tuple rayDir, Intersections intersections)
        {
            var count = p1_X.Length;
            float* dirCrossE2_X = stackalloc float[count];
            float* dirCrossE2_Y = stackalloc float[count];
            float* dirCrossE2_Z = stackalloc float[count];
            float* det = stackalloc float[count];

            var rayDirX = (float) rayDir.X;
            var rayDirY = (float) rayDir.Y;
            var rayDirZ = (float) rayDir.Z;
            Vector256<float> vRayDirX = Vector256.Create(rayDirX);
            Vector256<float> vRayDirY = Vector256.Create(rayDirY);
            Vector256<float> vRayDirZ = Vector256.Create(rayDirZ);

            fixed (float* ptre1x = e1_X)
            fixed (float* ptre1y = e1_Y)
            fixed (float* ptre1z = e1_Z)
            fixed (float* ptre2x = e2_X)
            fixed (float* ptre2y = e2_Y)
            fixed (float* ptre2z = e2_Z)
            {
                for (int i = 0; i < count; i += Size)
                {
                    Vector256<float> e1x = Avx.LoadVector256(ptre1x + i);
                    Vector256<float> e1y = Avx.LoadVector256(ptre1y + i);
                    Vector256<float> e1z = Avx.LoadVector256(ptre1z + i);

                    Vector256<float> e2x = Avx.LoadVector256(ptre2x + i);
                    Vector256<float> e2y = Avx.LoadVector256(ptre2y + i);
                    Vector256<float> e2z = Avx.LoadVector256(ptre2z + i);

                    var crossX = Avx.Subtract(Avx.Multiply(vRayDirY, e2z), Avx.Multiply(vRayDirZ, e2y));
                    var crossY = Avx.Subtract(Avx.Multiply(vRayDirZ, e2x), Avx.Multiply(vRayDirX, e2z));
                    var crossZ = Avx.Subtract(Avx.Multiply(vRayDirX, e2y), Avx.Multiply(vRayDirY, e2x));

                    var vDet = Avx.Add(Avx.Multiply(e1x, crossX), Avx.Add( Avx.Multiply(e1y, crossY), Avx.Multiply(e1z, crossZ)));
                    Avx.Store(det+i, vDet);
                    Avx.Store(dirCrossE2_X+i, crossX);
                    Avx.Store(dirCrossE2_Y+i, crossY);
                    Avx.Store(dirCrossE2_Z+i, crossZ);
                }
            }
            
            float originX = (float)origin.X;
            float originY = (float)origin.Y;
            float originZ = (float)origin.Z;
            var u = stackalloc float[count];
            var f = stackalloc float[count];
            var p1ToOrigin_X = stackalloc float[count];
            var p1ToOrigin_Y = stackalloc float[count];
            var p1ToOrigin_Z = stackalloc float[count];
            bool skipAll = true;
            bool* skip = stackalloc bool[count];
            Vector256<float> epsPos = Vector256.Create((float)Helper.Epsilon);
            Vector256<float> epsNeg = Vector256.Create((float)Helper.Epsilon);
            Vector256<float> ones = Vector256.Create(1f);

            for (int i = 0; i < count; i++)
            {
                skip[i] = det[i] > -Helper.Epsilon && det[i] < Helper.Epsilon;
                skipAll &= skip[i];
            }
            
            if (skipAll)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (skip[i]) 
                {
                    continue;
                }

                f[i] = 1.0f / det[i];
                p1ToOrigin_X[i] = originX - p1_X[i];
                p1ToOrigin_Y[i] = originY - p1_Y[i];
                p1ToOrigin_Z[i] = originZ - p1_Z[i];

                float uu = p1ToOrigin_X[i] * dirCrossE2_X[i];
                uu += p1ToOrigin_Y[i] * dirCrossE2_Y[i];
                uu += p1ToOrigin_Z[i] * dirCrossE2_Z[i];
                u[i] = f[i] *uu;
                skip[i] = u[i] < 0 || u[i] > 1;
            }

            var v = stackalloc float[count];
            var originCrossE1_X = stackalloc float[count];
            var originCrossE1_Y = stackalloc float[count];
            var originCrossE1_Z = stackalloc float[count];
            skipAll = true;

            for (int i = 0; i < count; i++)
            {
                if (skip[i])
                {
                    continue;
                }
                skipAll = false;
                originCrossE1_X[i] = p1ToOrigin_Y[i] * e1_Z[i] - p1ToOrigin_Z[i] * e1_Y[i];
                originCrossE1_Y[i] = p1ToOrigin_Z[i] * e1_X[i] - p1ToOrigin_X[i] * e1_Z[i];
                originCrossE1_Z[i] = p1ToOrigin_X[i] * e1_Y[i] - p1ToOrigin_Y[i] * e1_X[i];

                v[i] = f[i] * (rayDirX * originCrossE1_X[i] + rayDirY * originCrossE1_Y[i] + rayDirZ * originCrossE1_Z[i]);
            }
            if (skipAll)
            {
                return;
            }
            
            for (int i = 0; i < Triangles.Count; i++)
            {
                if (skip[i] || v[i] < 0 || (u[i] + v[i]) > 1)
                {
                    continue;
                }

                var t = f[i] * (e2_X[i] * originCrossE1_X[i] + e2_Y[i] * originCrossE1_Y[i] + e2_Z[i] * originCrossE1_Z[i]);
                var intersection = new Intersection(t, Triangles[i] , u[i], v[i]);
                intersections.Add(intersection);
            }
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
                    leftGroup = new TriangleGroup(left, true);
                    leftGroup.Divide(threshold);
                }

                if (right.Count > 0)
                {
                    rightGroup = new TriangleGroup(right, true);
                    rightGroup.Divide(threshold);
                }
            }
            return this;
        }
        
        
        private Material material;

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
        
        public IShape this[int i] => Triangles[i];
        public int Count => Triangles.Count;
        
        private List<Triangle> Triangles { get; } = new List<Triangle>();

        public TriangleGroup()
        {
        }

        public TriangleGroup(Group group)
        {
            Triangles.Capacity = group.Count;
            for (int i = 0; i < group.Count; i++)
            {
                var shape = group[i];
                if (shape is Triangle tri)
                {
                    Add(tri);
                }
            }
        }

        private TriangleGroup(List<Triangle> triangles, bool keepParent)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                var triangle = triangles[i];
                if (keepParent)
                {
                    Add(triangle, keepParent);
                }
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
    }
}