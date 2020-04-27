using System;
using System.Collections.Generic;
using System.Numerics;

namespace ray_tracer.Shapes.TriangleGroup
{
    public readonly struct TriangleData
    {
        public float p1_X { get; }
        public float p1_Y { get; }
        public float p1_Z { get; }
        public float e1_X { get; }
        public float e1_Y { get; }
        public float e1_Z { get; }
        public float e2_X { get; }
        public float e2_Y { get; }
        public float e2_Z { get; }

        public TriangleData(float p1X, float p1Y, float p1Z, float e1X, float e1Y, float e1Z, float e2X, float e2Y, float e2Z)
        {
            p1_X = p1X;
            p1_Y = p1Y;
            p1_Z = p1Z;
            e1_X = e1X;
            e1_Y = e1Y;
            e1_Z = e1Z;
            e2_X = e2X;
            e2_Y = e2Y;
            e2_Z = e2Z;
        }
    }

    public struct TriangleResult
    {
        public bool skip{ get; set; }
        public float u { get; set; }
        public float f { get; set; }
        public float v { get; set; }
        public float originCrossE1_X { get; set; }
        public float originCrossE1_Y { get; set; }
        public float originCrossE1_Z { get; set; }
    }
    
    public class TriangleGroupGpu : AbstractTriangleGroup
    {
        private bool cached = false;
        private TriangleData[] triangleData;

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

            triangleData = new TriangleData[size];

            for (var i = 0; i < Triangles.Count; i++)
            {
                var tri = Triangles[i];
                var transformedP1 = Vector4.Transform(tri.P1.vector, tri.Transform.matrix);
                var transformedE1 = Vector4.Transform(tri.E1.vector, tri.Transform.matrix);
                var transformedE2 = Vector4.Transform(tri.E2.vector, tri.Transform.matrix);

                var data = new TriangleData
                (
                    transformedP1.X,
                    transformedP1.Y,
                    transformedP1.Z,
                    transformedE1.X,
                    transformedE1.Y,
                    transformedE1.Z,
                    transformedE2.X,
                    transformedE2.Y,
                    transformedE2.Z
                );

                triangleData[i] = data;
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
            var triangleResult = stackalloc TriangleResult[count];
            var rayDirX = (float) direction.X;
            var rayDirY = (float) direction.Y;
            var rayDirZ = (float) direction.Z;
            float originX = (float) origin.X;
            float originY = (float) origin.Y;
            float originZ = (float) origin.Z;

            for (int i = 0; i < count; i++)
            {
                Compute(i, ref triangleData, ref triangleResult, originX, originY, originZ, rayDirX, rayDirY, rayDirZ);
            }

            for (int i = 0; i < count; i++)
            {
                if (triangleResult[i].skip)
                {
                    continue;
                }
                var result = triangleResult[i];
                var dataTriangle = triangleData[i];
                var t = result.f * (dataTriangle.e2_X * result.originCrossE1_X + dataTriangle.e2_Y * result.originCrossE1_Y + dataTriangle.e2_Z * result.originCrossE1_Z);
                var intersection = new Intersection(t, Triangles[i], result.u, result.v);
                intersections.Add(intersection);
            }
        }

        private unsafe void Compute(in int i, ref TriangleData[] triangleDatas, ref TriangleResult* triangleResult, in float originX, in float originY, in float originZ, in float rayDirX, in float rayDirY, in float rayDirZ)
        {
            var result = triangleResult[i];
            var dataTriangle = triangleDatas[i];
            
            float dirCrossE2_X = rayDirY * dataTriangle.e2_Z - rayDirZ * dataTriangle.e2_Y;
            float dirCrossE2_Y = rayDirZ * dataTriangle.e2_X - rayDirX * dataTriangle.e2_Z;
            float dirCrossE2_Z = rayDirX * dataTriangle.e2_Y - rayDirY * dataTriangle.e2_X;

            float det = dataTriangle.e1_X * dirCrossE2_X + dataTriangle.e1_Y * dirCrossE2_Y + dataTriangle.e1_Z * dirCrossE2_Z;

            result.f = 1.0f / det;
                
            float p1ToOrigin_X = originX - dataTriangle.p1_X;
            float p1ToOrigin_Y = originY - dataTriangle.p1_Y;
            float p1ToOrigin_Z = originZ - dataTriangle.p1_Z;

            result.u = result.f * (p1ToOrigin_X * dirCrossE2_X + p1ToOrigin_Y * dirCrossE2_Y + p1ToOrigin_Z * dirCrossE2_Z);
                 
            result.originCrossE1_X = p1ToOrigin_Y * dataTriangle.e1_Z - p1ToOrigin_Z * dataTriangle.e1_Y;
            result.originCrossE1_Y = p1ToOrigin_Z * dataTriangle.e1_X - p1ToOrigin_X * dataTriangle.e1_Z;
            result.originCrossE1_Z = p1ToOrigin_X * dataTriangle.e1_Y - p1ToOrigin_Y * dataTriangle.e1_X;

            result.v = result.f * (rayDirX * result.originCrossE1_X + rayDirY * result.originCrossE1_Y + rayDirZ * result.originCrossE1_Z);

            result.skip = MathF.Abs(det) < Helper.Epsilon || result.u < 0 || result.u > 1 || result.v < 0 || (result.u + result.v) > 1;
            triangleResult[i] = result;
        }

        protected override AbstractTriangleGroup GetSubGroup(List<Triangle> triangles, bool keepParent)
        {
            return new TriangleGroupGpu(triangles, keepParent);
        }
    }
}