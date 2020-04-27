using System;
using System.Collections.Generic;
using System.Numerics;

namespace ray_tracer.Shapes.TriangleGroup
{
    public struct TriangleData
    {
        public float p1_X { get; set; }
        public float p1_Y { get; set; }
        public float p1_Z { get; set; }
        public float e1_X { get; set; }
        public float e1_Y { get; set; }
        public float e1_Z { get; set; }
        public float e2_X { get; set; }
        public float e2_Y { get; set; }
        public float e2_Z { get; set; }
    }

    public class TriangleGroupGpu : AbstractTriangleGroup
    {
        private bool cached = false;
        private TriangleData[] TriangleData { get; set; }

        public TriangleGroupGpu()
        {
        }

        public TriangleGroupGpu(Group triangleGroup) : base(triangleGroup)
        {
        }

        private TriangleGroupGpu(List<Triangle> triangles, in bool keepParent) : base(triangles, keepParent)
        {
        }

        private void BuildCaches()
        {
            int size = Triangles.Count;
            int remains = Triangles.Count % Size;
            if (remains != 0)
            {
                size += Size - remains;
            }

            TriangleData = new TriangleData[size];

            for (var i = 0; i < Triangles.Count; i++)
            {
                var tri = Triangles[i];
                var transformedP1 = Vector4.Transform(tri.P1.vector, tri.Transform.matrix);
                var transformedE1 = Vector4.Transform(tri.E1.vector, tri.Transform.matrix);
                var transformedE2 = Vector4.Transform(tri.E2.vector, tri.Transform.matrix);

                var triangleData = new TriangleData
                {
                    p1_X = transformedP1.X,
                    p1_Y = transformedP1.Y,
                    p1_Z = transformedP1.Z,
                    e1_X = transformedE1.X,
                    e1_Y = transformedE1.Y,
                    e1_Z = transformedE1.Z,
                    e2_X = transformedE2.X,
                    e2_Y = transformedE2.Y,
                    e2_Z = transformedE2.Z
                };

                TriangleData[i] = triangleData;
            }
        }

        protected override void IntersectTriangles(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            if (!cached)
            {
                BuildCaches();
                cached = true;
            }

            IntersectAll(ref origin, ref direction, intersections);
        }

        private unsafe void IntersectAll(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            var count = Triangles.Count;
            var skip = stackalloc bool[count];
            var dirCrossE2_X = stackalloc float[count];
            var dirCrossE2_Y = stackalloc float[count];
            var dirCrossE2_Z = stackalloc float[count];
            var det = stackalloc float[count];

            var rayDirX = (float) direction.X;
            var rayDirY = (float) direction.Y;
            var rayDirZ = (float) direction.Z;

            for (int i = 0; i < count; i++)
            {
                var dataTriangle = TriangleData[i];
                dirCrossE2_X[i] = rayDirY * dataTriangle.e2_Z - rayDirZ * dataTriangle.e2_Y;
                dirCrossE2_Y[i] = rayDirZ * dataTriangle.e2_X - rayDirX * dataTriangle.e2_Z;
                dirCrossE2_Z[i] = rayDirX * dataTriangle.e2_Y - rayDirY * dataTriangle.e2_X;

                det[i] = dataTriangle.e1_X * dirCrossE2_X[i] + dataTriangle.e1_Y * dirCrossE2_Y[i] + dataTriangle.e1_Z * dirCrossE2_Z[i];
            }

            float originX = (float) origin.X;
            float originY = (float) origin.Y;
            float originZ = (float) origin.Z;
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
                var dataTriangle = TriangleData[i];
                p1ToOrigin_X[i] = originX - dataTriangle.p1_X;
                p1ToOrigin_Y[i] = originY - dataTriangle.p1_Y;
                p1ToOrigin_Z[i] = originZ - dataTriangle.p1_Z;

                float uu = p1ToOrigin_X[i] * dirCrossE2_X[i];
                uu += p1ToOrigin_Y[i] * dirCrossE2_Y[i];
                uu += p1ToOrigin_Z[i] * dirCrossE2_Z[i];
                u[i] = f[i] * uu;
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
                var dataTriangle = TriangleData[i];
                originCrossE1_X[i] = p1ToOrigin_Y[i] * dataTriangle.e1_Z - p1ToOrigin_Z[i] * dataTriangle.e1_Y;
                originCrossE1_Y[i] = p1ToOrigin_Z[i] * dataTriangle.e1_X - p1ToOrigin_X[i] * dataTriangle.e1_Z;
                originCrossE1_Z[i] = p1ToOrigin_X[i] * dataTriangle.e1_Y - p1ToOrigin_Y[i] * dataTriangle.e1_X;

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
                var dataTriangle = TriangleData[i];
                var t = f[i] * (dataTriangle.e2_X * originCrossE1_X[i] + dataTriangle.e2_Y * originCrossE1_Y[i] + dataTriangle.e2_Z * originCrossE1_Z[i]);
                var intersection = new Intersection(t, Triangles[i], u[i], v[i]);
                intersections.Add(intersection);
            }            
        }

        protected override AbstractTriangleGroup GetSubGroup(List<Triangle> triangles, bool keepParent)
        {
            return new TriangleGroupGpu(triangles, keepParent);
        }
    }
}