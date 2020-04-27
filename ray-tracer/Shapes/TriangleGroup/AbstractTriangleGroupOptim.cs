using System;
using System.Collections.Generic;
using System.Numerics;

namespace ray_tracer.Shapes.TriangleGroup
{
    public abstract class AbstractTriangleGroupOptim : AbstractTriangleGroup
    {
        private bool cached;
        protected float[] p1_X, p1_Y, p1_Z;
        protected float[] e1_X, e1_Y, e1_Z;
        protected float[] e2_X, e2_Y, e2_Z;

        protected abstract void IntersectAll(ref Tuple origin, ref Tuple direction, Intersections intersections);

        public AbstractTriangleGroupOptim()
        {
        }

        public AbstractTriangleGroupOptim(Group triangleGroup) : base(triangleGroup)
        {
        }

        public AbstractTriangleGroupOptim(List<Triangle> triangles, bool keepParent) : base(triangles, keepParent)
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

        protected override void IntersectTriangles(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            if (!cached)
            {
                BuildCaches();
                cached = true;
            }

            IntersectAll(ref origin, ref direction, intersections);
        }
    }

    class TriangleGroupBatch : AbstractTriangleGroupOptim
    {
        public TriangleGroupBatch()
        {
        }

        public TriangleGroupBatch(Group triangleGroup) : base(triangleGroup)
        {
        }

        private TriangleGroupBatch(List<Triangle> triangles, in bool keepParent)
            : base(triangles, keepParent)
        {
        }

        protected override AbstractTriangleGroup GetSubGroup(List<Triangle> triangles, bool keepParent)
        {
            return new TriangleGroupBatch(triangles, keepParent);
        }

        protected override unsafe void IntersectAll(ref Tuple origin, ref Tuple direction, Intersections intersections)
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
                dirCrossE2_X[i] = rayDirY * e2_Z[i] - rayDirZ * e2_Y[i];
                dirCrossE2_Y[i] = rayDirZ * e2_X[i] - rayDirX * e2_Z[i];
                dirCrossE2_Z[i] = rayDirX * e2_Y[i] - rayDirY * e2_X[i];

                det[i] = e1_X[i] * dirCrossE2_X[i] + e1_Y[i] * dirCrossE2_Y[i] + e1_Z[i] * dirCrossE2_Z[i];
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
                p1ToOrigin_X[i] = originX - p1_X[i];
                p1ToOrigin_Y[i] = originY - p1_Y[i];
                p1ToOrigin_Z[i] = originZ - p1_Z[i];

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
                var intersection = new Intersection(t, Triangles[i], u[i], v[i]);
                intersections.Add(intersection);
            }
        }
    }
}