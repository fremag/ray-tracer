using System;
using System.Diagnostics;
using System.IO;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using Tuple = ray_tracer.Tuple;

namespace ray_tracer_demos
{
    public static partial class Program
    {
        static void Main()
        {
            //GCSettings.LatencyMode = GCLatencyMode.Batch;
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Start time: {DateTime.Now:HH:mm:ss}");
            var scene = new MengerSpongeScene();
            scene.RowRendered += OnRowRendered;
            var file = scene.Render("menger_sponge.ppm", 4, 5, -8, lookX: -1);
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds:###,###,##0} ms");
            Helper.Display(file);
            File.Delete(file);
        }

        public static string RevolutionDemo()
        {
            var world = new World();
            var floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            world.Add(floor);

            IShape shape = CreateSurfaceRevolution((u, v) => 0.5 + 0.25 * +Math.Sin(Math.PI * 2 * v), 20, 20);
            world.Add(shape);

            var point = Helper.CreatePoint(4, 2, -4);
            world.Lights.Add(new PointLight(Helper.CreatePoint(15, 5, -15), Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 1, 0)));
            camera.RowRendered += Program.OnRowRendered;
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "sor.ppm");
            canvas.SavePPM(file);
            return file;
        }

        private static IShape CreateSurfaceRevolution(Func<double, double, double> ray, int n1 = 10, int n2 = 10)
        {
            var group = new Group();

            for (int i = 0; i < n1; i++)
            {
                var u = (double) i / n1;
                for (int j = 0; j < n1; j++)
                {
                    var v = (double) j / n2;
                    var p1 = Compute(u, v, ray);
                    var p2 = Compute(u + 1.0 / n1, v, ray);
                    var p3 = Compute(u, v + 1.0 / n2, ray);
                    var p4 = Compute(u + 1.0 / n1, v + 1.0 / n2, ray);

                    //group.Add(new Sphere().Scale(0.1).Translate(x, y, z));
                    group.Add(new Triangle(p1, p2, p3));
                    group.Add(new Triangle(p4, p2, p3));
                }
            }

            return group;
        }

        private static Tuple Compute(in double u, in double v, Func<double, double, double> ray)
        {
            var r = ray(u, v);
            var x = r * Math.Cos(2 * Math.PI * u);
            var y = v;
            var z = r * Math.Sin(2 * Math.PI * u);
            return Helper.CreatePoint(x, y, z);
        }
    }
}