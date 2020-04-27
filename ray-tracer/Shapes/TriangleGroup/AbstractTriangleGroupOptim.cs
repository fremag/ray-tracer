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
}