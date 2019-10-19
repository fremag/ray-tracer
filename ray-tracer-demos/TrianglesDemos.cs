using System;
using System.IO;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    internal static class TrianglesDemos
    {
        public static string RenderTeapot()
        {
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            world.Add(floor);

            ObjFileReader teapotObj = new ObjFileReader("teapot.obj");
            var teapot = teapotObj.ObjToGroup();
            
            world.Add(teapot);
            
            var point = Helper.CreatePoint(7, 5, -10);
            world.Lights.Add(new PointLight(Helper.CreatePoint(15, 15, -15), Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 1, 0)));
            camera.RowRendered += Program.OnRowRendered;
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "teapot.ppm");
            canvas.SavePPM(file);
            return file;
        }
    
        public static string RenderPikachu()
        {
            var world = new World();
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White).Scale(5))
            };
            world.Add(floor);

            ObjFileReader smoothPikachuObj = new ObjFileReader("pikachu.obj", true);
            var smoothPikachu = smoothPikachuObj.ObjToGroup();
            smoothPikachu.Rotate(ry: Math.PI).Translate(tx: 0.5);
            world.Add(smoothPikachu);

            ObjFileReader pikachuObj = new ObjFileReader("pikachu.obj", false);
            var pikachu = pikachuObj.ObjToGroup();
            pikachu.Rotate(ry: Math.PI).Translate(tx: -4);
            world.Add(pikachu);
            
            var point = Helper.CreatePoint(10, 10, -10)/2;
            world.Lights.Add(new PointLight(Helper.CreatePoint(100, 100, -100), Color.White));
            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(point, Helper.CreatePoint(0, 3, 0), Helper.CreateVector(0, 1, 0)));
            camera.RowRendered += Program.OnRowRendered;
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "pikachu.ppm");
            canvas.SavePPM(file);
            return file;
        }
    }
}