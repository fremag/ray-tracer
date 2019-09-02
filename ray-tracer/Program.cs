using System;
using System.Diagnostics;
using System.IO;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var file = RenderWorldPlaneStripePatternTest();
            sw.Stop();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds:###,###,##0} ms");
            Helper.Display(file);
            File.Delete(file);
        }

        public static string RenderWorldTest()
        {
            var floor = Helper.Sphere();
            floor.Transform = Helper.Scaling(10, 0.01, 10);
            floor.Material = new Material(new Color(1, 0.9, 0.9), specular: 0);

            var leftWall = Helper.Sphere();
            leftWall.Transform = Helper.Translation(0, 0, 5) * Helper.RotationY(-Math.PI / 4) *
                                 Helper.RotationX(Math.PI / 2) * Helper.Scaling(10, 0.01, 10);
            leftWall.Material = floor.Material;

            var rightWall = Helper.Sphere();
            rightWall.Transform = Helper.Translation(0, 0, 5) * Helper.RotationY(Math.PI / 4) * Helper.RotationX(Math.PI / 2) * Helper.Scaling(10, 0.01, 10);
            rightWall.Material = floor.Material;

            var middle = Helper.Sphere();
            middle.Transform = Helper.Translation(-0.5, 1, 0.5);
            middle.Material = new Material(new Color(0.1, 1, 0.5), diffuse: 0.7, specular: 0.3);

            var right = Helper.Sphere();
            right.Transform = Helper.Translation(1.5, 0.5, -0.5) * Helper.Scaling(0.5, 0.5, 0.5);
            right.Material = new Material(new Color(0.5, 1, 0.1), diffuse: 0.7, specular: 0.3);

            var left = Helper.Sphere();
            left.Transform = Helper.Translation(-1.5, 0.33, -0.75) * Helper.Scaling(0.33, 0.33, 0.33);
            left.Material = new Material(new Color(1, 0.8, 0.1), diffuse: 0.7, specular: 0.3);

            var world = new World();
            world.Shapes.AddRange(new [] {floor, leftWall, rightWall, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10,10,-10), Color.White));

            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 1.5, -5), Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "helloword.ppm");
            
            Helper.SavePPM(canvas, file);
            return file;
        }
        public static string RenderWorldPlaneTest()
        {
            IShape floor = new Plane();
            floor.Material = new Material(new Color(1, 0.9, 0.9), specular: 0);

            var middle = Helper.Sphere();
            middle.Transform = Helper.Translation(-0.5, 1, 0.5);
            middle.Material = new Material(new Color(0.1, 1, 0.5), diffuse: 0.7, specular: 0.3);

            var right = Helper.Sphere();
            right.Transform = Helper.Translation(1.5, 0.5, -0.5) * Helper.Scaling(0.5, 0.5, 0.5);
            right.Material = new Material(new Color(0.5, 1, 0.1), diffuse: 0.7, specular: 0.3);

            var left = Helper.Sphere();
            left.Transform = Helper.Translation(-1.5, 0.33, -0.75) * Helper.Scaling(0.33, 0.33, 0.33);
            left.Material = new Material(new Color(1, 0.8, 0.1), diffuse: 0.7, specular: 0.3);

            var world = new World();
            world.Shapes.AddRange(new [] {floor, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10,10,-10), Color.White));

            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 1.5, -5), Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "world_plane.ppm");
            
            Helper.SavePPM(canvas, file);
            return file;
        }
        
        public static string RenderWorldPlaneStripePatternTest()
        {
            IShape floor = new Plane();
            floor.Material = new Material(new RingPattern(Color.White, new Color(0,0,1)), specular: 0);

            var middle = Helper.Sphere();
            middle.Transform = Helper.Translation(-0.5, 1, 0.5);
            middle.Material = new Material(new StripePattern(new Color(0.5, 1, 0.1), new Color(1, 0.5, 0.1)) {Transform = Helper.Scaling(0.1, 1, 1)}, diffuse: 0.7, specular: 0.3);

            var right = Helper.Sphere();
            right.Transform = Helper.Translation(1.5, 0.5, -0.5) * Helper.Scaling(0.5, 0.5, 0.5);
            right.Material = new Material(new GradientPattern(new Color(0, 0, 1), new Color(1, 0.5, 0.1)) {Transform = Helper.RotationY(30)*Helper.Scaling(0.25, 1, 1)}, diffuse: 0.7, specular: 1);

            var left = Helper.Sphere();
            left.Transform = Helper.Translation(-1.5, 0.33, -0.75) * Helper.Scaling(0.33, 0.33, 0.33);
            left.Material = new Material(new CheckerPattern(new Color(1, 0, 0), new Color(0, 1, 0)) {Transform = Helper.Scaling(0.25, 0.25, 0.25)}, diffuse: 0.7, specular: 0.3);

            var world = new World();
            world.Shapes.AddRange(new [] {floor, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10,10,-10), Color.White));

            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 3.5, -5), Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "world_patterns.ppm");
            
            Helper.SavePPM(canvas, file);
            return file;
        }
    }
}
