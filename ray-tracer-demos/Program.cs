using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using ray_tracer;

namespace ray_tracer_demos
{
    public static class Program
    {
        static void Main()
        {
            int nbThreads = 4+0*Environment.ProcessorCount;
            Run(new List<Type>
            {
                typeof(CubeScene),
//                typeof(SquareMeshScene),
//                typeof(SurfaceOfRevolutionScene),
//                typeof(CurveSweepScene),
//                typeof(LabyrinthScene),
            }, nbThreads, true);
            

//            BenchmarkFull();
        }

        private static void BenchmarkFull()
        {
            var scenes = Helper.GetScenes<IcosahedronScene>().Values.ToList();
            Run(scenes, display: true);
        }

        public static void Run(IEnumerable<Type> scenes, int nbThreads = -1, bool display = false)
        {
            string dir = Path.Combine(Path.GetTempPath(), "raytracer");
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }

            Directory.CreateDirectory(dir);

            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Start time: {DateTime.Now:HH:mm:ss}");
            Run(dir, nbThreads, scenes.ToArray());
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds:###,###,##0} ms");
            if (display)
            {
                Helper.Display(dir);
            }
        }

        public const int L = 30;

        public static void Run(string dir, int nbThreads, params Type[] sceneTypes)
        {
            RenderManager renderMgr = new RenderManager(dir);
            Timer timer = new Timer(500)
            {
                AutoReset = true
            };
            timer.Elapsed += (sender, args) => { Print(renderMgr); };

            foreach (var sceneType in sceneTypes)
            {
                Console.CursorLeft = 0;
                Console.Write($"{sceneType.Name,-L}");
                timer.Start();
                var scene = renderMgr.Render(sceneType, nbThreads);
                renderMgr.Wait();
                timer.Stop();

                Print(renderMgr);
                Console.WriteLine();
                renderMgr.Save($"{scene.GetType().Name}.ppm");
            }
        }

        private static void Print(RenderManager renderMgr)
        {
            Console.CursorLeft = L + 1;
            var stats = renderMgr.RenderStatistics;
            Console.Write($"{stats.Progress,8:p2}      {stats.Time:hh\\:mm\\:ss} {stats.Speed,15:n2} px/s");
        }
    }
}