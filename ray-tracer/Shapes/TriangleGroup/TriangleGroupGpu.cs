#define GPU
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ILGPU;
using ILGPU.Runtime;

namespace ray_tracer.Shapes.TriangleGroup
{
    public readonly struct RayData
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public float DirX { get; }
        public float DirY { get; }
        public float DirZ { get; }

        public RayData(float x, float y, float z, float dirX, float dirY, float dirZ)
        {
            X = x;
            Y = y;
            Z = z;
            DirX = dirX;
            DirY = dirY;
            DirZ = dirZ;
        }
    }

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
        public int skip { get; set; }
        public float u { get; set; }
        public float f { get; set; }
        public float v { get; set; }
        public float originCrossE1_X { get; set; }
        public float originCrossE1_Y { get; set; }
        public float originCrossE1_Z { get; set; }
    }

    public class TriangleGroupGpu : AbstractTriangleGroup
    {
        private static Accelerator cuda;
        private static Context context;
        private static Action<Index1, ArrayView<TriangleData>, ArrayView<TriangleResult>, RayData> kernel;

        static TriangleGroupGpu()
        {
            context = new Context();
            var cudaId = Accelerator.Accelerators.FirstOrDefault(acceleratorId => acceleratorId.AcceleratorType == AcceleratorType.Cuda);
            cuda = Accelerator.Create(context, cudaId);

            kernel = cuda.LoadAutoGroupedStreamKernel<Index1, ArrayView<TriangleData>, ArrayView<TriangleResult>, RayData>(ComputeKernel);
        }

        private bool cached = false;
        private TriangleData[] triangleData;
        MemoryBuffer<TriangleData> dataBuffer;
        MemoryBuffer<TriangleResult> resultBuffer;
        private readonly object objLock = new object();

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
            triangleData = new TriangleData[size];

            for (var i = 0; i < size; i++)
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

            dataBuffer = cuda.Allocate<TriangleData>(size);
            dataBuffer.CopyFrom(cuda.DefaultStream, triangleData, 0, 0, size);
            resultBuffer = cuda.Allocate<TriangleResult>(size);
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

        private Stopwatch sw = new Stopwatch();

        private unsafe void IntersectAll(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            var count = Triangles.Count;
            var ray = new RayData((float) origin.X, (float) origin.Y, (float) origin.Z, (float) direction.X, (float) direction.Y, (float) direction.Z);
            sw.Restart();
#if GPU
            TriangleResult[] triangleResult;
            lock (objLock)
            {
                kernel(dataBuffer.Length, dataBuffer.View, resultBuffer, ray);
                cuda.Synchronize();
                triangleResult = resultBuffer.GetAsArray();
            }

            sw.Stop();
            for (int i = 0; i < count; i++)
            {
                if (triangleResult[i].skip == 0)
                {
                    continue;
                }

                var result = triangleResult[i];
                var dataTriangle = triangleData[i];
                var t = result.f * (dataTriangle.e2_X * result.originCrossE1_X + dataTriangle.e2_Y * result.originCrossE1_Y + dataTriangle.e2_Z * result.originCrossE1_Z);
                var intersection = new Intersection(t, Triangles[i], result.u, result.v);
                intersections.Add(intersection);
            }
#else
            TriangleResult* triangleResult = stackalloc TriangleResult[count];
            for (int i = 0; i < count; i++)
            {
                Compute(i, ref triangleData, ref triangleResult, ref ray);
            }
            sw.Stop();
            for (int i = 0; i < count; i++)
            {
                if (triangleResult[i].skip == 0)
                {
                    continue;
                }
                var result = triangleResult[i];
                var dataTriangle = triangleData[i];
                var t = result.f * (dataTriangle.e2_X * result.originCrossE1_X + dataTriangle.e2_Y * result.originCrossE1_Y + dataTriangle.e2_Z * result.originCrossE1_Z);
                var intersection = new Intersection(t, Triangles[i], result.u, result.v);
                intersections.Add(intersection);
            }
#endif
            var t0 = (float) sw.ElapsedTicks / Stopwatch.Frequency;
            Console.WriteLine(t0 * 1e6);
        }

        private static unsafe void Compute(in int i, ref TriangleData[] triangleDatas, ref TriangleResult* triangleResult, ref RayData ray)
        {
            var result = triangleResult[i];
            var dataTriangle = triangleDatas[i];

            float dirCrossE2_X = ray.DirY * dataTriangle.e2_Z - ray.DirZ * dataTriangle.e2_Y;
            float dirCrossE2_Y = ray.DirZ * dataTriangle.e2_X - ray.DirX * dataTriangle.e2_Z;
            float dirCrossE2_Z = ray.DirX * dataTriangle.e2_Y - ray.DirY * dataTriangle.e2_X;

            float det = dataTriangle.e1_X * dirCrossE2_X + dataTriangle.e1_Y * dirCrossE2_Y + dataTriangle.e1_Z * dirCrossE2_Z;

            result.f = 1.0f / det;

            float p1ToOrigin_X = ray.X - dataTriangle.p1_X;
            float p1ToOrigin_Y = ray.Y - dataTriangle.p1_Y;
            float p1ToOrigin_Z = ray.Z - dataTriangle.p1_Z;

            result.u = result.f * (p1ToOrigin_X * dirCrossE2_X + p1ToOrigin_Y * dirCrossE2_Y + p1ToOrigin_Z * dirCrossE2_Z);

            result.originCrossE1_X = p1ToOrigin_Y * dataTriangle.e1_Z - p1ToOrigin_Z * dataTriangle.e1_Y;
            result.originCrossE1_Y = p1ToOrigin_Z * dataTriangle.e1_X - p1ToOrigin_X * dataTriangle.e1_Z;
            result.originCrossE1_Z = p1ToOrigin_X * dataTriangle.e1_Y - p1ToOrigin_Y * dataTriangle.e1_X;

            result.v = result.f * (ray.DirX * result.originCrossE1_X + ray.DirY * result.originCrossE1_Y + ray.DirZ * result.originCrossE1_Z);

            result.skip = MathF.Abs(det) < Helper.Epsilon || result.u < 0 || result.u > 1 || result.v < 0 || (result.u + result.v) > 1 ? 0 : 1;
            triangleResult[i] = result;
        }

        private static void ComputeKernel(Index1 i, ArrayView<TriangleData> triangleDatas, ArrayView<TriangleResult> triangleResult, RayData ray)
        {
            var result = triangleResult[i];
            var dataTriangle = triangleDatas[i];

            float dirCrossE2_X = ray.DirY * dataTriangle.e2_Z - ray.DirZ * dataTriangle.e2_Y;
            float dirCrossE2_Y = ray.DirZ * dataTriangle.e2_X - ray.DirX * dataTriangle.e2_Z;
            float dirCrossE2_Z = ray.DirX * dataTriangle.e2_Y - ray.DirY * dataTriangle.e2_X;

            float det = dataTriangle.e1_X * dirCrossE2_X + dataTriangle.e1_Y * dirCrossE2_Y + dataTriangle.e1_Z * dirCrossE2_Z;

            result.f = 1.0f / det;

            float p1ToOrigin_X = ray.X - dataTriangle.p1_X;
            float p1ToOrigin_Y = ray.Y - dataTriangle.p1_Y;
            float p1ToOrigin_Z = ray.Z - dataTriangle.p1_Z;

            result.u = result.f * (p1ToOrigin_X * dirCrossE2_X + p1ToOrigin_Y * dirCrossE2_Y + p1ToOrigin_Z * dirCrossE2_Z);

            result.originCrossE1_X = p1ToOrigin_Y * dataTriangle.e1_Z - p1ToOrigin_Z * dataTriangle.e1_Y;
            result.originCrossE1_Y = p1ToOrigin_Z * dataTriangle.e1_X - p1ToOrigin_X * dataTriangle.e1_Z;
            result.originCrossE1_Z = p1ToOrigin_X * dataTriangle.e1_Y - p1ToOrigin_Y * dataTriangle.e1_X;

            result.v = result.f * (ray.DirX * result.originCrossE1_X + ray.DirY * result.originCrossE1_Y + ray.DirZ * result.originCrossE1_Z);

            result.skip = MathF.Abs(det) < Helper.Epsilon || result.u < 0 || result.u > 1 || result.v < 0 || (result.u + result.v) > 1 ? 0 : 1;
            triangleResult[i] = result;
        }

        protected override AbstractTriangleGroup GetSubGroup(List<Triangle> triangles, bool keepParent)
        {
            return new TriangleGroupGpu(triangles, keepParent);
        }
    }
}