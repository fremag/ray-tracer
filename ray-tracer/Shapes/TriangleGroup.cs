#define AVX
#define OPTIM
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
#if AVX
using System.Runtime.Intrinsics.X86;
#endif

namespace ray_tracer.Shapes
{
    public class TriangleGroup : AbstractShape
    {
        static readonly int Size = Vector256<float>.Count;
        public IShape this[int i] => Triangles[i];
        public int Count => Triangles.Count;
        private List<Triangle> Triangles { get; } = new List<Triangle>();

        private readonly ConcurrentDictionary<long, bool> cacheContains = new ConcurrentDictionary<long, bool>();
        private TriangleGroup leftGroup;
        private TriangleGroup rightGroup;
        private Material material;
        
        private Bounds box;
        private bool cached;
        private float[] p1_X, p1_Y, p1_Z; 
        private float[] e1_X, e1_Y, e1_Z; 
        private float[] e2_X, e2_Y, e2_Z;

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

        public TriangleGroup(List<Triangle> triangles, bool keepParent)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                var triangle = triangles[i];
                Add(triangle, keepParent);
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

        private void BuildCaches()
        {
            int size = Triangles.Count;
            int remains = Triangles.Count % Size;
            if (remains != 0)
            {
                size +=Size - remains;
            }

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
                var transformedP1 = Vector4.Transform(tri.P1.vector, tri.Transform.matrix); 
                var transformedE1 = Vector4.Transform(tri.E1.vector, tri.Transform.matrix);
                var transformedE2 = Vector4.Transform(tri.E2.vector, tri.Transform.matrix);
                p1_X[i] = transformedP1.X;
                p1_Y[i] = transformedP1.Y;
                p1_Z[i] = transformedP1.Z;
                
                e1_X[i] = transformedE1.X;
                e1_Y[i] = transformedE1.Y;
                e1_Z[i] = transformedE1.Z;
                
                e2_X[i] = transformedE2.X;
                e2_Y[i] = transformedE2.Y;
                e2_Z[i] = transformedE2.Z;
            }
        }
#if AVX
        public unsafe void IntersectAll(ref Tuple origin, ref Tuple rayDir, Intersections intersections)
        {
            var count = p1_X.Length;
            var dirCrossE2_X = stackalloc float[count];
            var dirCrossE2_Y = stackalloc float[count];
            var dirCrossE2_Z = stackalloc float[count];
            var det = stackalloc float[count];

            var vRayDirX = Vector256.Create((float) rayDir.X);
            var vRayDirY = Vector256.Create((float) rayDir.Y);
            var vRayDirZ = Vector256.Create((float) rayDir.Z);

            fixed (float* ptre1x = e1_X)
            fixed (float* ptre1y = e1_Y)
            fixed (float* ptre1z = e1_Z)
            fixed (float* ptre2x = e2_X)
            fixed (float* ptre2y = e2_Y)
            fixed (float* ptre2z = e2_Z)
            {
                for (var i = 0; i < count; i += Size)
                {
                    var e1x = Avx.LoadVector256(ptre1x + i);
                    var e1y = Avx.LoadVector256(ptre1y + i);
                    var e1z = Avx.LoadVector256(ptre1z + i);

                    var e2x = Avx.LoadVector256(ptre2x + i);
                    var e2y = Avx.LoadVector256(ptre2y + i);
                    var e2z = Avx.LoadVector256(ptre2z + i);

                    var crossX = Avx.Subtract(Avx.Multiply(vRayDirY, e2z), Avx.Multiply(vRayDirZ, e2y));
                    var crossY = Avx.Subtract(Avx.Multiply(vRayDirZ, e2x), Avx.Multiply(vRayDirX, e2z));
                    var crossZ = Avx.Subtract(Avx.Multiply(vRayDirX, e2y), Avx.Multiply(vRayDirY, e2x));

                    var vDet = Avx.Add(Avx.Multiply(e1x, crossX), Avx.Add(Avx.Multiply(e1y, crossY), Avx.Multiply(e1z, crossZ)));
                    Avx.Store(det + i, vDet);
                    Avx.Store(dirCrossE2_X + i, crossX);
                    Avx.Store(dirCrossE2_Y + i, crossY);
                    Avx.Store(dirCrossE2_Z + i, crossZ);
                }
            }

            var skipAll = true;
            var skip = stackalloc float[count];
            var epsilon = (float) Helper.Epsilon;
            var epsPos = Vector256.Create(epsilon);
            var epsNeg = Vector256.Create(-epsilon);

            for (var i = 0; i < count; i += Size)
            {
                var vDet = Avx.LoadVector256(det + i);

                var v1 = Avx.Compare(vDet, epsNeg, FloatComparisonMode.OrderedLessThanNonSignaling);
                var v2 = Avx.Compare(vDet, epsPos, FloatComparisonMode.OrderedGreaterThanNonSignaling);
                var result = Avx.And(v1, v2);
                Avx.Store(skip + i, result);
                skipAll &= !Avx.TestZ(result, result);
            }

            if (skipAll)
            {
                return;
            }

            var u = stackalloc float[count];
            var f = stackalloc float[count];
            var p1ToOrigin_X = stackalloc float[count];
            var p1ToOrigin_Y = stackalloc float[count];
            var p1ToOrigin_Z = stackalloc float[count];
            var vOriginX = Vector256.Create((float) origin.X);
            var vOriginY = Vector256.Create((float) origin.Y);
            var vOriginZ = Vector256.Create((float) origin.Z);

            var vOne = Vector256.Create(1f);
            var vZero = Vector256.Create(0f);
            
            var v = stackalloc float[count];
            var originCrossE1_X = stackalloc float[count];
            var originCrossE1_Y = stackalloc float[count];
            var originCrossE1_Z = stackalloc float[count];

            fixed (float* ptrp1x = p1_X)
            fixed (float* ptrp1y = p1_Y)
            fixed (float* ptrp1z = p1_Z)
            fixed (float* ptre1x = e1_X)
            fixed (float* ptre1y = e1_Y)
            fixed (float* ptre1z = e1_Z)
            {
                for (var i = 0; i < count; i += Size)
                {
                    var vSkip = Avx.LoadVector256(skip + i);
                    var skipVector = ! Avx.TestZ(vSkip, vSkip);
                    if (skipVector)
                    {
                        continue;
                    }

                    var p1x = Avx.LoadVector256(ptrp1x + i);
                    var p1y = Avx.LoadVector256(ptrp1y + i);
                    var p1z = Avx.LoadVector256(ptrp1z + i);
                    
                    var vp1ToOrigin_X = Avx.Subtract(vOriginX, p1x);
                    var vp1ToOrigin_Y = Avx.Subtract(vOriginY, p1y);
                    var vp1ToOrigin_Z = Avx.Subtract(vOriginZ, p1z);
                    
                    var vdirCrossE2_X = Avx.LoadVector256(dirCrossE2_X + i);
                    var vdirCrossE2_Y = Avx.LoadVector256(dirCrossE2_Y + i);
                    var vdirCrossE2_Z = Avx.LoadVector256(dirCrossE2_Z + i);
                    
                    var uuX = Avx.Multiply(vp1ToOrigin_X, vdirCrossE2_X);
                    var uuY = Avx.Multiply(vp1ToOrigin_Y, vdirCrossE2_Y);
                    var uuZ = Avx.Multiply(vp1ToOrigin_Z, vdirCrossE2_Z);
                    var uu = Avx.Add(uuX, Avx.Add(uuY, uuZ));

                    var vDet = Avx.LoadVector256(det + i);
                    var vF = Avx.Divide(vOne, vDet);
                    var vU = Avx.Multiply(vF, uu);

                    var vCmpZero = Avx.Compare(vU, vZero, FloatComparisonMode.OrderedLessThanNonSignaling);
                    var vCmpOne = Avx.Compare(vU, vOne, FloatComparisonMode.OrderedGreaterThanNonSignaling);
                    vSkip = Avx.Or(vCmpOne, vCmpZero);
                    
                    Avx.Store(skip+i, vSkip);
                    Avx.Store(f+i, vF);
                    Avx.Store(u+i, vU);
                    Avx.Store(p1ToOrigin_X + i, vp1ToOrigin_X);
                    Avx.Store(p1ToOrigin_Y + i, vp1ToOrigin_Y);
                    Avx.Store(p1ToOrigin_Z + i, vp1ToOrigin_Z);

                    var e1x = Avx.LoadVector256(ptre1x + i);
                    var e1y = Avx.LoadVector256(ptre1y + i);
                    var e1z = Avx.LoadVector256(ptre1z + i);
                    var p1ToOriginX = Avx.LoadVector256(p1ToOrigin_X + i);
                    var p1ToOriginY = Avx.LoadVector256(p1ToOrigin_Y + i);
                    var p1ToOriginZ = Avx.LoadVector256(p1ToOrigin_Z + i);

                    var vx1 = Avx.Multiply(p1ToOriginY, e1z);
                    var vx2 = Avx.Multiply(p1ToOriginZ, e1y);
                    var vX = Avx.Subtract(vx1, vx2);
                    Avx.Store(originCrossE1_X + i, vX);

                    var vy1 = Avx.Multiply(p1ToOriginZ, e1x);
                    var vy2 = Avx.Multiply(p1ToOriginX, e1z);
                    var vY = Avx.Subtract(vy1, vy2);
                    Avx.Store(originCrossE1_Y + i, vY);

                    var vz1 = Avx.Multiply(p1ToOriginX, e1y);
                    var vz2 = Avx.Multiply(p1ToOriginY, e1x);
                    var vZ = Avx.Subtract(vz1, vz2);
                    Avx.Store(originCrossE1_Z + i, vZ);

                    var vOriginCrossE1_X = Avx.LoadVector256(originCrossE1_X + i);
                    var vOriginCrossE1_Y = Avx.LoadVector256(originCrossE1_Y + i);
                    var vOriginCrossE1_Z = Avx.LoadVector256(originCrossE1_Z + i);
                    
                    var vf = Avx.LoadVector256(f + i);
                    var vvX = Avx.Multiply(vRayDirX, vOriginCrossE1_X);
                    var vvY = Avx.Multiply(vRayDirY, vOriginCrossE1_Y);
                    var vvZ = Avx.Multiply(vRayDirZ, vOriginCrossE1_Z);
                    var vv = Avx.Multiply(vf, Avx.Add(vvX, Avx.Add(vvY, vvZ)));
                    Avx.Store(v + i, vv);
                }
            }
            
            for (var i = 0; i < Triangles.Count; i++)
            {
                if (float.IsNaN(skip[i]) || v[i] < 0 || (u[i] + v[i]) > 1)
                {
                    continue;
                }

                var t = f[i] * (e2_X[i] * originCrossE1_X[i] + e2_Y[i] * originCrossE1_Y[i] + e2_Z[i] * originCrossE1_Z[i]);
                var intersection = new Intersection(t, Triangles[i] , u[i], v[i]);
                intersections.Add(intersection);
            }
        }
#else
       public unsafe void IntersectAll(ref Tuple origin, ref Tuple rayDir, Intersections intersections)
        {
            var count = Triangles.Count;
            var skip = stackalloc bool[count];
            var dirCrossE2_X = stackalloc float[count];
            var dirCrossE2_Y = stackalloc float[count];
            var dirCrossE2_Z = stackalloc float[count];
            var det = stackalloc float[count];
            
            var rayDirX = (float)rayDir.X;
            var rayDirY = (float)rayDir.Y;
            var rayDirZ = (float)rayDir.Z;

            for (int i = 0; i < count; i++)
            {
                dirCrossE2_X[i] = rayDirY * e2_Z[i] - rayDirZ * e2_Y[i];
                dirCrossE2_Y[i] = rayDirZ * e2_X[i] - rayDirX * e2_Z[i];
                dirCrossE2_Z[i] = rayDirX * e2_Y[i] - rayDirY * e2_X[i];

                det[i] = e1_X[i] * dirCrossE2_X[i] + e1_Y[i] * dirCrossE2_Y[i] + e1_Z[i] * dirCrossE2_Z[i];
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
            for (int i = 0; i < count; i++)
            {
                if (Math.Abs(det[i]) < Helper.Epsilon)
                {
                    skip[i] = true;
                    continue;
                }

                skipAll = false;
                skip[i] = false;
                f[i] = 1.0f / det[i];
                p1ToOrigin_X[i] = originX - p1_X[i];
                p1ToOrigin_Y[i] = originY - p1_Y[i];
                p1ToOrigin_Z[i] = originZ - p1_Z[i];

                float uu = p1ToOrigin_X[i] * dirCrossE2_X[i];
                uu += p1ToOrigin_Y[i] * dirCrossE2_Y[i];
                uu += p1ToOrigin_Z[i] * dirCrossE2_Z[i];
                u[i] = f[i] *uu;
            }

            if (skipAll)
            {
                return;
            }

            skipAll = true;
            var v = stackalloc float[count];
            var originCrossE1_X = stackalloc float[count];
            var originCrossE1_Y = stackalloc float[count];
            var originCrossE1_Z = stackalloc float[count];
            for (int i = 0; i < count; i++)
            {
                if (u[i] < 0 || u[i] > 1)
                {
                    skip[i] = true;
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
            
            for (int i = 0; i < count; i++)
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
#endif
        
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
    }
}