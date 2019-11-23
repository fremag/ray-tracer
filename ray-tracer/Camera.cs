#define OPTIM_PARALLEL
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ray_tracer
{
    public class Camera
    {
        public int HSize { get; }
        public int VSize { get; }
        public double FieldOfView { get; }
        public Matrix Transform { get; }
        public double PixelSize { get; }
        public double HalfHeight { get; }
        public double HalfWidth { get; }

        private Matrix InverseTransform { get; }
        private ConcurrentQueue<RenderJob> RenderJobs { get; } = new ConcurrentQueue<RenderJob>();
        public event Action<int, int> RowRendered;
        private readonly object lockObj = new object();

        public Camera(int hSize, int vSize, double fieldOfView) :
            this(hSize, vSize, fieldOfView, Helper.CreateIdentity())
        {
        }

        public Camera(int hSize, int vSize, double fieldOfView, Matrix transform)
        {
            HSize = hSize;
            VSize = vSize;
            FieldOfView = fieldOfView;
            Transform = transform;

            var halfView = Math.Tan(fieldOfView / 2);
            var aspect = (double) HSize / VSize;
            if (aspect >= 1)
            {
                HalfWidth = halfView;
                HalfHeight = halfView / aspect;
            }
            else
            {
                HalfWidth = halfView * aspect;
                HalfHeight = halfView;
            }

            PixelSize = HalfWidth * 2 / HSize;

            InverseTransform = Transform.Inverse();
        }

        public Ray RayForPixel(int px, int py)
        {
// the offset from the edge of the canvas to the pixel's center
            var xOffset = (px + 0.5) * PixelSize;
            var yOffset = (py + 0.5) * PixelSize;
// the untransformed coordinates of the pixel in world space.
// (remember that the camera looks toward -z, so +x is to the *left*.)
            var worldX = HalfWidth - xOffset;
            var worldY = HalfHeight - yOffset;
// using the camera matrix, transform the canvas point and the origin,
// and then compute the ray's direction vector.
// (remember that the canvas is at z=-1)
            var pixel = InverseTransform * Helper.CreatePoint(worldX, worldY, -1);
            var origin = InverseTransform * Helper.CreatePoint(0, 0, 0);
            var direction = (pixel - origin).Normalize();
            return Helper.Ray(origin, direction);
        }

        public void Render(Canvas canvas, World world, int nbThreads = 4, int maxRecursion = 10, bool shuffle=true)
        {
            var jobs = new List<RenderJob>(VSize*HSize);
            for (int y = 0; y < VSize; y+=1)
            {
                for (int x = 0; x < HSize; x+=1)
                {
                    var ray = RayForPixel(x, y);
                    var renderJob = new RenderJob(x, y, canvas, world, maxRecursion, ray, 1, 1);
                    jobs.Add(renderJob);
                }
            }

            if (shuffle)
            {
                Random r = new Random();
                foreach(var renderJob in jobs.OrderBy(job =>  r.Next()))
                {
                    RenderJobs.Enqueue(renderJob);
                }
            }
            else
            {
                jobs.ForEach(job => RenderJobs.Enqueue(job));
            }

            for (int i = 0; i < nbThreads; i++)
            {
                Thread t = new Thread(Run);
                t.Name = "RayTracerWorker_" + i;
                t.Start();
            }
        }

        private void Run()
        {
            while (RenderJobs.TryDequeue(out var renderJob))
            {
                renderJob.DoWork();
            }
        }
        
        public Canvas Render(World world, int maxRecursion = 10)
        {
            var image = new Canvas(HSize, VSize);
            Render(image, world, maxRecursion);
            
            return image;
        }
    }

    public class RenderJob
    {
        public int X { get; }
        public int Y { get; }
        public int XSize { get; }
        public int YSize { get; }
        public Canvas Canvas { get; }
        public World World { get; }
        public int MaxRecursion { get; }
        public Ray Ray { get; }

        public RenderJob(in int x, in int y, Canvas canvas, World world, int maxRecursion, Ray ray, int xSize, int ySize)
        {
            X = x;
            Y = y;
            XSize = xSize;
            YSize = ySize;
            Canvas = canvas;
            World = world;
            MaxRecursion = maxRecursion;
            Ray = ray;
        }

        public void DoWork()
        {
            var color = World.ColorAt(Ray, MaxRecursion);
            for(int i=0; i < XSize; i++)
            {
                for(int j=0; j < YSize; j++)
                {
                    Canvas.SetPixel(X+i, Y+j, color);
                }
            }
        }
    }
}