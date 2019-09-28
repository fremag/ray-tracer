using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer
{
    class Program
    {
        static void Main()
        {
            GCSettings.LatencyMode = GCLatencyMode.Batch;
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Start time: {DateTime.Now:HH:mm:ss}"); 
            var file = RenderLabyrinthScene();
            sw.Stop();
            Console.WriteLine();
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

            canvas.SavePPM(file);
            return file;
        }

        private static void OnRowRendered(int progress, int hSize)
        {
            var startTime = Process.GetCurrentProcess().StartTime;
            var now = DateTime.Now;
            var pct = (100.0 * progress) / hSize;
            var t = (now - startTime).TotalSeconds / pct;
            var endTime = startTime.AddSeconds(100 * t);
            
            Console.SetCursorPosition(0, Console.CursorLeft);
            Console.Write($"{startTime-now:hh\\:mm\\:ss}> {pct,5:n2} % => {endTime-startTime:hh\\:mm\\:ss}, endTime: {endTime:HH\\:mm\\:ss}");
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

            canvas.SavePPM(file);
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

            canvas.SavePPM(file);
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

            canvas.SavePPM(file);
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
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "world_reflection_refraction.ppm");

            canvas.SavePPM(file);
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
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "glass_sphere.ppm");
            canvas.SavePPM(file);
            return file;
        }

        public static string RenderCubeScene()
        {
            const int n = 10;
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(pattern: new CheckerPattern(colorA: Color.Black, colorB: Color.White).Scale(scale: n), reflective: 0.3),
                Transform = Helper.Translation(x: 0, y: -1, z: 0)
            };
            world.Add(floor);
            Random r = new Random(Seed: 0);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            for (int k = 0; k < n; k++)
            {
                var red = i / (double) n;
                var green = j / (double) n;
                var blue = k / (double) n;
                var color = new Color(red: red, green: green, blue: blue);
                double ambient = r.NextDouble() / 2 + 0.25;
                double diffuse = r.NextDouble() / 2 + 0.5;
                double specular = r.NextDouble() / 2 + 0.5;
                var cube = new Cube {Material = new Material(color: color, ambient: ambient, diffuse: diffuse, specular: specular)}.Scale(sx: 1 - r.NextDouble() + 0.5, sy: 1 - r.NextDouble() + 0.5, sz: 1 - r.NextDouble() + 0.5).Translate(tx: i, ty: j, tz: k);
                world.Add(cube);
            }

            var d = 5 * Math.Sqrt(d: n);
            var point = Helper.CreatePoint(x: d, y: d, z: -d);
            world.Lights.Add(item: new PointLight(position: 2 * point, intensity: Color.White));
            var camera = new Camera(hSize: 600, vSize: 400, fieldOfView: Math.PI / 3, transform: Helper.ViewTransform(@from: point, to: Helper.CreatePoint(x: n / 2.0, y: n / 2.0, z: n / 2.0), up: Helper.CreateVector(x: 0, y: 1, z: 0)));
            var canvas = camera.Render(world: world);
            string file = Path.Combine(path1: Path.GetTempPath(), path2: "cubes.ppm");
            canvas.SavePPM(filePath: file);
            return file;
        }

        private static void AddObject(World world, double tx, double ty, double tz, int n)
        {
            var sphere = new Sphere().Scale(0.3).Translate(tx, ty, tz);
            var cyl1 = new Cylinder {Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(ry: Math.PI / 2).Translate(tx, ty, tz);
            var cyl2 = new Cylinder {Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(rx: Math.PI / 2).Translate(tx, ty, tz);
            var cyl3 = new Cylinder {Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(rz: Math.PI / 2).Translate(tx, ty, tz);

            cyl1.Material.Pattern = new SolidPattern(Color._Red * (tx / n));
            cyl2.Material.Pattern = new SolidPattern(Color._Green * (ty / n));
            cyl3.Material.Pattern = new SolidPattern(Color._Blue * (tz / n));

            var red = tx / n;
            var green = ty / n;
            var blue = tz / n;
            var color = new Color(red, green, blue);
            sphere.Material.Pattern = new SolidPattern(color);
            world.Add(sphere, cyl1, cyl2, cyl3);
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

            const int n = 5;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            for (int k = 0; k < n; k++)
                AddObject(world, i, j, k, n);

            var d = 3 * Math.Sqrt(n);
            var point = Helper.CreatePoint(d, d, -d);
            world.Lights.Add(new PointLight(2 * point, Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(n / 2.0, n / 2.0, n / 2.0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "cylinders.ppm");
            canvas.SavePPM(file);
            return file;
        }

        public static string RenderCylinderAltitudeScene()
        {
            var world = new World();

            const int n = 70;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                double x = (double) i / n - 0.5;
                double z = (double) j / n - 0.5;
                double r = Math.Sqrt(x * x + z * z);
                double y = (1 + Math.Sin(Math.PI * 2 * r * 8)) * 2;
                var cyl = new Cylinder(minimum: 0, maximum: 1, closed: true).Scale(sx: 0.5, sy: y, sz: 0.5).Translate(tx: i, tz: j);
                cyl.Material.Pattern = new SolidPattern(Color.White * (1 - r));
                cyl.Material.Transparency = 0.9;
                world.Add(cyl);
            }

            var d = 3 * Math.Sqrt(n);
            var point = Helper.CreatePoint(0, d, -d);
            world.Lights.Add(new PointLight(2 * point, Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(n / 2.0, 0 * n / 2.0, n / 2.0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world, 2);
            string file = Path.Combine(Path.GetTempPath(), "cylinders_altitude.ppm");
            canvas.SavePPM(file);
            return file;
        }

        public static string RenderConesScene()
        {
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            world.Add(floor);

            const int n = 200;
            for (int i = 1; i <= n; i++)
            {
                var t = (double) i / n;
                var x = 4 * t * Math.Cos(2 * Math.PI * t * 4);
                var z = 4 * t * Math.Sin(2 * Math.PI * t * 4);
                var cone = new Cone(-1, 0, true).Translate(ty: 1).Scale(sy: 1 - t + 0.5).Translate(tx: x, tz: z);
                cone.Material.Pattern = new SolidPattern(Color._Blue * t + Color._Green * (1 - t));
                world.Add(cone);
            }

            var point = Helper.CreatePoint(0, 5, -5);
            world.Lights.Add(new PointLight(2 * point, Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(0, 0, -2), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "cones.ppm");
            canvas.SavePPM(file);
            return file;
        }

        public static IShape CreatePyramid(int n = 5)
        {
            var g = new Group();
            for (int i = 1; i <= n; i++)
            {
                var cube = new Cube().Scale(sx: (n - i + 1) * 1.0 / n, sy: 0.1, sz: (n - i + 1) * 1.0 / n).Translate(ty: i * 1.0 / n);
                cube.Material.Pattern = new SolidPattern(Color._Red * i / n);
                cube.Material.Ambient = 0.1;
                g.Add(cube);
            }

            return g;
        }

        public static string RenderGroupScene()
        {
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            world.Add(floor);

            const int n = 4;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                var pyramid = CreatePyramid(10).Translate(tx: 2 * i, tz: 2 * j);
                world.Add(pyramid);
            }

            var point = Helper.CreatePoint(1, 1, -1) * 6;
            world.Lights.Add(new PointLight(2 * point, Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "groups_pyramids.ppm");
            canvas.SavePPM(file);
            return file;
        }


        public static string RenderLabyrinthScene()
        {
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            world.Add(floor);

            var w = 60;
            var lab = ComputeLabyrinth(w, w);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (lab[i][j] == 0)
                    {
                        var cube = new Cube().Scale(0.5).Translate(tx: i-w/2, tz: j-w/2);
                        cube.Material.Pattern = new SolidPattern(Color.White *0.8);
                        world.Add(cube);
                    }
                }
            }

            var point = Helper.CreatePoint(0, 1, -0.75) * w*0.75;
            world.Lights.Add(new PointLight(Helper.CreatePoint(0, w/3.0, 0), Color.White));
            var camera = new Camera(1200, 800, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 1, 0)));
            camera.RowRendered += OnRowRendered;
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "labyrinth.ppm");
            canvas.SavePPM(file);
            return file;
        }

        public static int[][] ComputeLabyrinth(int w, int h)
        {
            int[][] laby = new int[h][];
            for (int i = 0; i < h; i++)
            {
                laby[i] = new int[w];
                for (int j = 0; j < w; j++)
                {
                    laby[i][j] = 1;
                }
            }

            Random rand = new Random(0);
            laby[0][0] = 0;
            CreateLabyrinth(laby, rand, 0, 0);

            return laby;
        }

        private static void CreateLabyrinth(int[][] maze, Random rand, int r, int c)
        {
            // 4 random directions
            int randDirs = rand.Next() % 4;
            // Examine each direction
            for (int i = 0; i < 4; i++)
            {
                int dir = (randDirs + i) % 4;
                switch (dir)
                {
                    case 0: // Up
                        //　Whether 2 cells up is out or not
                        if (r - 2 <= 0)
                            continue;
                        if (maze[r - 2][c] != 0)
                        {
                            maze[r - 2][c] = 0;
                            maze[r - 1][c] = 0;
                            CreateLabyrinth(maze, rand, r - 2, c);
                        }

                        break;
                    case 1: // Right
                        // Whether 2 cells to the right is out or not
                        if (c + 2 >= maze[0].Length - 1)
                            continue;
                        if (maze[r][c + 2] != 0)
                        {
                            maze[r][c + 2] = 0;
                            maze[r][c + 1] = 0;
                            CreateLabyrinth(maze, rand, r, c + 2);
                        }

                        break;
                    case 2: // Down
                        // Whether 2 cells down is out or not
                        if (r + 2 >= maze.Length - 1)
                            continue;
                        if (maze[r + 2][c] != 0)
                        {
                            maze[r + 2][c] = 0;
                            maze[r + 1][c] = 0;
                            CreateLabyrinth(maze, rand, r + 2, c);
                        }

                        break;
                    case 3: // Left
                        // Whether 2 cells to the left is out or not
                        if (c - 2 <= 0)
                            continue;
                        if (maze[r][c - 2] != 0)
                        {
                            maze[r][c - 2] = 0;
                            maze[r][c - 1] = 0;
                            CreateLabyrinth(maze, rand, r, c - 2);
                        }

                        break;
                }
            }
        }

        public static void PrintLabyrinth(int[][] laby)
        {
            for (int i = 0; i < laby.Length; i++)
            {
                for (int j = 0; j < laby[i].Length; j++)
                {
                    var c = laby[i][j] == 0 ? '█' : ' ';
                    Console.Write(c);
                }

                Console.WriteLine();
            }
        }
    }
}