using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ray_tracer
{
    public class RenderManager
    {
        public Canvas Image { get; private set; }
        private ConcurrentQueue<PixelJob> PixelJobs { get; } = new ConcurrentQueue<PixelJob>();
        private Thread[] threads;
        private bool stopRequested;

        public void Render(CameraParameters camParams, RenderParameters renderParameters, World world)
        {
            Image = new Canvas(camParams.Width, camParams.Height);
            var point = Helper.CreatePoint(camParams.CameraX, camParams.CameraY, camParams.CameraZ);
            var look = Helper.CreatePoint(camParams.LookX, camParams.LookY, camParams.LookZ);

            var viewTransform = Helper.ViewTransform(point, look, Helper.CreateVector(0, 1, 0));
            var camera = new Camera(camParams.Width, camParams.Height, Math.PI / 3, viewTransform);
            stopRequested = false;
            Render(camera, world, renderParameters.NbThreads);
        }

        public void Stop()
        {
            stopRequested = true;
            PixelJobs.Clear();
        }
        
        public void Render(World world, double camX, double camY, double camZ, double lookX=0, double lookY =0, double lookZ =0, int h=400, int w=600)
        {
            var camParams = new CameraParameters
            {
                CameraX =  camX,
                CameraY = camY,
                CameraZ = camZ,
                LookX = lookX,
                LookY =  lookY,
                LookZ = lookZ,
                Height = h,
                Width = w
            };
            Render(camParams, new RenderParameters(), world);
        }
        
        public void Render(Camera camera, World world, int nbThreads = 4, int maxRecursion = 10, bool shuffle=true)
        {
            var pixelJobs = new List<PixelJob>(camera.VSize*camera.HSize);
            for (int y = 0; y < camera.VSize; y++)
            {
                for (int x = 0; x < camera.HSize; x++)
                {
                    var ray = camera.RayForPixel(x, y);
                    var renderJob = new PixelJob(x, y, Image, world, maxRecursion, ray);
                    pixelJobs.Add(renderJob);
                }
            }

            if (shuffle)
            {
                Random r = new Random();
                pixelJobs = pixelJobs.OrderBy(job => r.Next()).ToList();
            }
            pixelJobs.ForEach(job => PixelJobs.Enqueue(job));
            threads = new Thread[nbThreads];

            for (int i = 0; i < nbThreads; i++)
            {
                var tName = $"RayTracerWorker_{i}";
                Thread t = new Thread(() => Run(tName));
                threads[i] = t;
                t.Name = tName;
                t.Start();
            }
        }

        private void Run(string name)
        {
            int n = 0;
            while (! stopRequested && PixelJobs.TryDequeue(out var renderJob))
            {
                renderJob.DoWork();
                n++;
            }
            Console.WriteLine($"{name}: {n}");
        }

        public void Wait()
        {
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        public string Save(string file)
        {
            string outFilePath = Path.Combine(Path.GetTempPath(), file);
            Image.SavePPM(outFilePath);
            return outFilePath;
        }
    }
}