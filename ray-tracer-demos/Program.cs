using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Timers;
using ray_tracer;
using ray_tracer_demos.Basic;

namespace ray_tracer_demos
{
    public static class Program
    {
        static void Main()
        {
            GCSettings.LatencyMode = GCLatencyMode.Batch;
            Console.WriteLine($"IsHardwareAccelerated: {Vector.IsHardwareAccelerated}");
            int nbThreads = Environment.ProcessorCount*1+0;
            bool display = true;
            bool shuffle = true;
            if (true)
            {
                Run(new List<Type>
                {
                    //typeof(PenroseTriangleScene),
                    //typeof(GlassSphereScene),
//                    typeof(TestScene),
                    typeof(IsoSurfaceScene),
//                    typeof(ChristmasScene),
//                    typeof(BlobScene),
//                    typeof(CloverWireScene),
//                    typeof(WireFrameScene),
//                    typeof(RingPerlinScene),
//                    typeof(OneRingPerlinScene),
//                    typeof(SpotLightSoftShadowScene),
//                    typeof(SpotLightScene),
//                    typeof(PerlinScene),
//                    typeof(ShadowGlamourShotScene),
//                    typeof(SoftShadowScene),
//                    typeof(IcosahedronScene),
//                    typeof(PrismMeshScene),
//                    typeof(CylinderScene),
//                    typeof(CylinderAltitudeScene),
//                    typeof(MengerCastleScene),
//                   typeof(TeapotScene),
//                   typeof(PikachuScene),
//                    typeof(CubeScene),
//                typeof(SquareMeshScene),
//                typeof(SurfaceOfRevolutionScene),
//                typeof(CurveSweepScene),
//                typeof(LabyrinthScene),
                }, nbThreads, display, shuffle);
            }
            else
            {
                BenchmarkFull(nbThreads);
            }
        }

        private static void BenchmarkFull(int nbThreads)
        {
            var scenes = Helper.GetScenes<ConeScene>(typeof(ConeScene).Namespace).Values.ToList();
            Run(scenes, nbThreads: nbThreads, display: !true, shuffle: false);
        }

        public static void Run(IEnumerable<Type> scenes, int nbThreads = -1, bool display = false, bool shuffle=true)
        {
            string dir = Path.Combine(Path.GetTempPath(), "raytracer");
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Console.WriteLine($"CreateDirectory: {dir}");
            Directory.CreateDirectory(dir);

            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Start time: {DateTime.Now:HH:mm:ss}");
            var files = Run(dir, nbThreads, shuffle, scenes.ToArray());
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds:###,###,##0} ms");
            if (display)
            {
                Helper.Display(files.Count == 1 ? files[0] : dir);
            }
        }

        public static List<string> Run(string dir, int nbThreads, bool shuffle, params Type[] sceneTypes)
        {
            var files = new List<string>();
            RenderManager renderMgr = new RenderManager(dir);

            foreach (var sceneType in sceneTypes)
            {
                Timer timer = new Timer(500)
                {
                    AutoReset = true
                };
                timer.Elapsed += (sender, args) => { Print(sceneType.Name, renderMgr); };
                timer.Start();
                var scene = renderMgr.Render(sceneType, nbThreads, shuffle);
                renderMgr.Wait();
                timer.Stop();

                Print(sceneType.Name, renderMgr);
                Console.WriteLine();
               var file = renderMgr.Save($"{scene.GetType().Name}.ppm");
               files.Add(file);
            }

            return files;
        }

        private static void Print(string sceneTypeName, RenderManager renderMgr)
        {
            Console.CursorLeft = 0;
            var stats = renderMgr.RenderStatistics;
            if (stats == null)
            {
                return;
            }
            Console.Write($"{sceneTypeName,-30} {stats.Progress,8:p2}      {stats.Time:hh\\:mm\\:ss} {stats.Speed,10:###,###,##0} px/s");
        }
    }
}