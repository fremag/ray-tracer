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
        public RenderStatistics RenderStatistics { get; private set; }
        public Canvas Image { get; private set; }
        private ConcurrentQueue<PixelJob> PixelJobs { get; } = new ConcurrentQueue<PixelJob>();
        private Thread[] threads;
        private bool stopRequested;
        public int PixelsLeft => PixelJobs.Count;
        public string OutputDir { get; } 

        public RenderManager() : this(Path.GetTempPath())
        {
        }

        public RenderManager(string outputDir)
        {
            OutputDir = outputDir;
        }

        public void Render(CameraParameters camParams, RenderParameters renderParameters, World world)
        {
            Image = new Canvas(camParams.Width, camParams.Height);
            var point = Helper.CreatePoint(camParams.CameraX, camParams.CameraY, camParams.CameraZ);
            var look = Helper.CreatePoint(camParams.LookX, camParams.LookY, camParams.LookZ);

            var viewTransform = Helper.ViewTransform(point, look, Helper.CreateVector(0, 1, 0));
            var camera = new Camera(camParams.Width, camParams.Height, Math.PI / 3, viewTransform);
            stopRequested = false;
            RenderStatistics = new RenderStatistics {Start =  DateTime.Now, TotalPixels = camParams.Width * camParams.Height};
            Render(camera, world, renderParameters.NbThreads);
        }

        public void Stop()
        {
            stopRequested = true;
            PixelJobs.Clear();
        }
        
        public void Render(World world, double camX, double camY, double camZ, double lookX=0, double lookY =0, double lookZ =0, int h=400, int w=600, int nbThreads=8)
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
            var renderParameters = new RenderParameters
            {
                NbThreads = nbThreads
            };
            
            Render(camParams, renderParameters, world);
        }
        
        public void Render(Camera camera, World world, int nbThreads = 4, int maxRecursion = 10, bool shuffle=true)
        {
            var pixelJobs = new List<PixelJob>(camera.VSize*camera.HSize);
            for (int y = 0; y < camera.VSize; y++)
            {
                for (int x = 0; x < camera.HSize; x++)
                {
                    var ray = camera.RayForPixel(x, y);
                    var renderJob = new PixelJob(x, y, Image, world, maxRecursion, ray, RenderStatistics);
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

            if (nbThreads == 0)
            {
                Run();
                return;
            }

            for (int i = 0; i < nbThreads; i++)
            {
                Thread t = new Thread(Run) {Name = $"RayTracerWorker_{i}"};
                t.Start();
                threads[i] = t;
            }
        }

        private void Run()
        {
            while (! stopRequested && PixelJobs.TryDequeue(out var renderJob))
            {
                renderJob.DoWork();
            }

            RenderStatistics.Stop();
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
            string outFilePath = Path.Combine(OutputDir, file);
            Image.SavePPM(outFilePath);
            return outFilePath;
        }

        public void Render(AbstractScene scene, int nbThreads=-1)
        {
            Render(scene.CameraParameters[0], new RenderParameters
            {
                NbThreads = nbThreads >= 0 ? nbThreads : Environment.ProcessorCount
            }, scene.World);
        }

        public AbstractScene Render<T>(int nbThreads = -1) where T : AbstractScene
        {
            return Render(typeof(T), nbThreads);
        }
        
        public AbstractScene Render(Type sceneType, int nbThreads=-1)
        {
            AbstractScene scene = Activator.CreateInstance(sceneType) as AbstractScene;
            if (scene == null)
            {
                throw new InvalidOperationException($"Wrong scene type: {sceneType}");
            }
            scene.InitWorld();
            Render(scene, nbThreads);
            return scene;
        }
    }
}