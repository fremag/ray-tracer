using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Timers;
using ray_tracer;

namespace ray_tracer_demos
{
    public static class Program
    {
        static void Main()
        {
            Console.WriteLine(Vector.IsHardwareAccelerated);
            int nbThreads = Environment.ProcessorCount * 0 + 4;
            if (!true)
            {
                Run(new List<Type>
                {
                    typeof(SingleSphereScene)
   //                 typeof(SingleCylinderScene),
  //                  typeof(CylinderScene),
 //                   typeof(CylinderAltitudeScene),
//                    typeof(CsgBasicScene),
//                    typeof(MengerSpongeScene),

//                    typeof(TeapotScene),
//                    typeof(CurveSweepScene),
//                    typeof(PikachuScene),
                    //    typeof(CubeScene),
//                typeof(SquareMeshScene),
//                typeof(SurfaceOfRevolutionScene),
//                typeof(CurveSweepScene),
//                typeof(LabyrinthScene),
                }, nbThreads, true, false);
            }
            else
            {
                BenchmarkFull();
            }
        }

        private static void BenchmarkFull()
        {
            var scenes = Helper.GetScenes<IcosahedronScene>().Values.ToList();
            Run(scenes, display: true);
        }

        public static void Run(IEnumerable<Type> scenes, int nbThreads = -1, bool display = false, bool shuffle = false)
        {
            string dir = Path.Combine(Path.GetTempPath(), "raytracer");
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }

            Directory.CreateDirectory(dir);

            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Start time: {DateTime.Now:HH:mm:ss}");
            Run(dir, nbThreads, shuffle, scenes.ToArray());
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds:###,###,##0} ms");
            if (display)
            {
                Helper.Display(dir);
            }
        }

        public const int L = 30;

        public static void Run(string dir, int nbThreads, bool shuffle, params Type[] sceneTypes)
        {
            RenderManager renderMgr = new RenderManager(dir);

            foreach (var sceneType in sceneTypes)
            {
                Console.CursorLeft = 0;
                Timer timer = new Timer(500)
                {
                    AutoReset = true
                };
                timer.Start();
                timer.Elapsed += (sender, args) => { Print(sceneType.Name, renderMgr); };
                var scene = renderMgr.Render(sceneType, nbThreads, shuffle);
                renderMgr.Wait();
                timer.Stop();

                Print(sceneType.Name, renderMgr);
                Console.WriteLine();
                renderMgr.Save($"{scene.GetType().Name}.ppm");
            }
        }

        private static void Print(string name, RenderManager renderMgr)
        {
            Console.CursorLeft = 0;
            var stats = renderMgr.RenderStatistics;
            if (stats == null)
            {
                return;
            }

            Console.Write($"{name,-L} {stats.Progress,8:p2}      {stats.Time:hh\\:mm\\:ss} {stats.Speed,15:n2} px/s");
        }
    }
}