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
            var file = RenderCylinderAltitudeScene();
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
            world.Shapes.AddRange(new[] {floor, leftWall, rightWall, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10, 10, -10), Color.White));

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
            world.Shapes.AddRange(new[] {floor, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10, 10, -10), Color.White));

            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 1.5, -5), Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "world_plane.ppm");

            Helper.SavePPM(canvas, file);
            return file;
        }

        public static string RenderWorldPlaneStripePatternTest()
        {
            IShape floor = new Plane();
            floor.Material = new Material(new RingPattern(Color.White, new Color(0, 0, 1)), specular: 0);
            floor.Transform = Helper.Translation(0, 0.5, 0);
            var middle = Helper.Sphere();
            middle.Transform = Helper.Translation(-0.5, 1, 0.5);
            middle.Material = new Material(new StripePattern(new Color(0.5, 1, 0.1), new Color(1, 0.5, 0.1)) {Transform = Helper.Scaling(0.1, 1, 1)}, diffuse: 0.7, specular: 0.3);

            var right = Helper.Sphere();
            right.Transform = Helper.Translation(1.5, 0.5, -0.5) * Helper.Scaling(0.5, 0.5, 0.5);
            right.Material = new Material(new GradientPattern(new Color(0, 0, 1), new Color(1, 0.5, 0.1)) {Transform = Helper.RotationY(30) * Helper.Scaling(0.25, 1, 1)}, diffuse: 0.7, specular: 1);

            var left = Helper.Sphere();
            left.Transform = Helper.Translation(-1.5, 0.33, -0.75) * Helper.Scaling(0.33, 0.33, 0.33);
            left.Material = new Material(new CheckerPattern(new Color(1, 0, 0), new Color(0, 1, 0)) {Transform = Helper.Scaling(0.25, 0.25, 0.25)}, diffuse: 0.7, specular: 0.3);

            var world = new World();
            world.Shapes.AddRange(new[] {floor, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10, 10, -10), Color.White));

            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 3.5, -5), Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "world_patterns.ppm");

            Helper.SavePPM(canvas, file);
            return file;
        }

        public static string RenderWorldReflectionTest()
        {
            IShape floor = new Plane();
            floor.Material = new Material(new CheckerPattern(Color.White, Color.Black), specular: 0, reflective: 0.5);
            floor.Transform = Helper.Translation(0, 0.25, 0);

            var middle = Helper.Sphere();
            middle.Transform = Helper.Translation(-0.5, 1, 0.5);
            middle.Material = new Material(new Color(0.5, 1, 0.1), diffuse: 0.7, specular: 0.3, reflective: 1);

            var right = Helper.Sphere();
            right.Transform = Helper.Translation(1.5, 0.5, -0.5) * Helper.Scaling(0.5, 0.5, 0.5);
            right.Material = new Material(new GradientPattern(new Color(0, 0, 1), new Color(1, 0.5, 0.1)) {Transform = Helper.RotationY(30) * Helper.Scaling(0.25, 1, 1)}, diffuse: 0.7, specular: 1);

            var left = Helper.Sphere();
            left.Transform = Helper.Translation(-1.5, 0.33, -0.75) * Helper.Scaling(0.33, 0.33, 0.33);
            left.Material = new Material(new CheckerPattern(new Color(1, 0, 0), new Color(0, 1, 0)) {Transform = Helper.Scaling(0.25, 0.25, 0.25)}, diffuse: 0.7, specular: 0.3);

            var world = new World();
            world.Shapes.AddRange(new[] {floor, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10, 10, -10), Color.White));

            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 1.5, -3), Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "world_reflection.ppm");

            Helper.SavePPM(canvas, file);
            return file;
        }

        /*
         * Thanks to Javan Makhmali (https://github.com/javan)
         * 
         * I wanted to check my ray tracer was correct so I got the same scene as him to compare results.
         * https://github.com/javan/ray-tracer-challenge/blob/master/src/controllers/chapter_11_worker.js
         */
        public static string RenderWorldReflectionRefractionTest()
        {
            IShape floor = new Plane();
            floor.Material = new Material(new CheckerPattern(Color.White * 0.35, Color.White * 0.65)
            {
                Transform = Helper.RotationY(45)
            }, reflective: 0.4, specular: 0);

            Material material = new Material(new Color(1, 0.3, 0.2), specular: 0.4, shininess: 5);
            var s1 = new Sphere {Material = material, Transform = Helper.Translation(6, 1, 4)};
            var s2 = new Sphere {Material = material, Transform = Helper.Translation(2, 1, 3)};
            var s3 = new Sphere {Material = material, Transform = Helper.Translation(-1, 1, 2)};
            var blueSphere = new Sphere
            {
                Material = new Material(new Color(0, 0, 0.2), ambient: 0, diffuse: 0.4, specular: 0.9, shininess: 300, reflective: 0.9, transparency: 0.9, refractiveIndex: 1.5),
                Transform = Helper.Translation(0.6, 0.7, -0.6) * Helper.Scaling(0.7, 0.7, 0.7)
            };
            var greenSphere = new Sphere
            {
                Material = new Material(new Color(0, 0.2, 0), ambient: 0, diffuse: 0.4, specular: 0.9, shininess: 300, reflective: 0.9, transparency: 0.9, refractiveIndex: 1.5),
                Transform = Helper.Translation(-0.7, 0.5, -0.8) * Helper.Scaling(0.5, 0.5, 0.5)
            };
            var world = new World();
            world.Shapes.AddRange(new[] {floor, s1, s2, s3, blueSphere, greenSphere});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-4.9, 4.9, -1), Color.White));

            var camera = new Camera(1600, 1200, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(-2.6, 1.5, -4.9), Helper.CreatePoint(-0.6, 1, -0.8), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world, 10);
            string file = Path.Combine(Path.GetTempPath(), "world_reflection_refraction.ppm");

            Helper.SavePPM(canvas, file);
            return file;
        }

        public static string RenderGlassSphereTest()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White) {Transform = Helper.Scaling(4, 4, 4)}, reflective: 0, specular: 0, diffuse: 0, shininess: 0, ambient: 1),
                Transform = Helper.Translation(0, -1, 0)
            };
            var s2 = new Sphere
            {
                Material = new Material(Color.White, transparency: 0.9, refractiveIndex: 1.5, reflective: 1, shininess: 300, specular: 0.9, ambient: 0, diffuse: 0.4)
            };

            var world = new World();
            world.Shapes.AddRange(new[]
            {
                floor,
                s2
            });
            world.Lights.Add(new PointLight(Helper.CreatePoint(-100, 0, -50), Color.White));

            var camera = new Camera(1200, 800, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 0, -3), Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world, 10);
            string file = Path.Combine(Path.GetTempPath(), "glass_sphere.ppm");
            Helper.SavePPM(canvas, file);
            return file;
        }

        public static string RenderCubeScene()
        {
            const int N = 10;
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White).Scale(N), reflective: 0.3),
                Transform = Helper.Translation(0, -1, 0)
            };
            world.Add(floor);
            Random r = new Random(0);
            for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
            for (int k = 0; k < N; k++)
            {
                var red = i / (double) N;
                var green = j / (double) N;
                var blue = k / (double) N;
                var color = new Color(red, green, blue);
                double ambient = r.NextDouble()/2+0.25;
                double diffuse = r.NextDouble()/2+0.5;
                double specular = r.NextDouble()/2+0.5;
                var cube = new Cube {Material = new Material(color, ambient, diffuse, specular)}.Scale(1 - r.NextDouble() + 0.5, 1 - r.NextDouble() + 0.5, 1 - r.NextDouble() + 0.5).Translate(i, j, k);
                world.Add(cube);
            }

            var d = 5 * Math.Sqrt(N);
            var point = Helper.CreatePoint(d,  d, -d);
            world.Lights.Add(new PointLight(2*point, Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(N / 2, N / 2, N / 2), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "cubes.ppm");
            Helper.SavePPM(canvas, file);
            return file;
        }

        private static void AddObject(World world, double tx, double ty, double tz, int n)
        {
            var sphere = new Sphere().Scale(0.3).Translate(tx, ty, tz);
            var cyl1 = new Cylinder{Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(ry: Math.PI / 2).Translate(tx, ty, tz);
            var cyl2 = new Cylinder{Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(rx: Math.PI / 2).Translate(tx, ty, tz);
            var cyl3 = new Cylinder{Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(rz: Math.PI / 2).Translate(tx, ty, tz);
            
            cyl1.Material.Pattern = new SolidPattern(Color._Red * (tx / n));
            cyl2.Material.Pattern = new SolidPattern(Color._Green * (ty / n));
            cyl3.Material.Pattern = new SolidPattern(Color._Blue * (tz / n));

            var red = tx /  n;
            var green = ty / n;
            var blue = tz / n;
            var color = new Color(red, green, blue);
            sphere.Material.Pattern = new SolidPattern(color);
            world.Add(sphere, cyl1,cyl2,cyl3);
        }
        
        public static string RenderCylinderScene()
        {
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White).Scale(1), reflective: 0.3, transparency: 0.9),
                Transform = Helper.Translation(0, -0.5, 0)
            };
            world.Add(floor);

            const int N = 5;
            for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
            for (int k = 0; k < N; k++)
                AddObject(world, i, j, k, N);
            
            var d = 3 * Math.Sqrt(N);
            var point = Helper.CreatePoint(d,  d, -d);
            world.Lights.Add(new PointLight(2*point, Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(N / 2, N / 2, N / 2), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "cylinders.ppm");
            Helper.SavePPM(canvas, file);
            return file;
        }
        
        public static string RenderCylinderAltitudeScene()
        {
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White).Scale(1), reflective: 0.3, transparency: 0.9),
                Transform = Helper.Translation(0, -0.5, 0)
            };
            world.Add(floor);

            const int N = 50;
            for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
            {
                double x = (double)i/N - 0.5;
                double z =  (double)j / N - 0.5;
                double r = Math.Sqrt(x * x + z * z);
                double y = 1+Math.Sin(Math.PI * 2 * r*5)*1/r;
                var cyl = new Cylinder(minimum:0, maximum:1, closed: true).Scale(sx: 0.5, sy: y, sz: 0.5).Translate(tx: i, tz: j);
                cyl.Material.Pattern = new SolidPattern(Color.White*r);
                world.Add(cyl);
            }
            
            var d = 3 * Math.Sqrt(N);
            var point = Helper.CreatePoint(d,  d, -d);
            world.Lights.Add(new PointLight(2*point, Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(N / 2, 0*N / 2, N / 2), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "cylinders_altitude.ppm");
            Helper.SavePPM(canvas, file);
            return file;
        }
        
    }
}