using System;
using System.IO;
using System.Reflection;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class PikachuScene : AbstractScene
    {
        public PikachuScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 5, CameraY = 5, CameraZ = -10,
                LookX = 0, LookY = 2, LookZ = 0});
        }

        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White).Scale(5))
            };

            Add(floor);

            var assembly = typeof(PikachuScene).GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("ray_tracer_demos.Basic.Pikachu.obj");
            ObjFileReader smoothPikachuObj = new ObjFileReader(resource, true);
            var smoothPikachu = smoothPikachuObj.ObjToGroup();
            smoothPikachu.Rotate(ry: Math.PI).Translate(tx: 0.5);
            Add(smoothPikachu);

            resource = assembly.GetManifestResourceStream("ray_tracer_demos.Basic.Pikachu.obj");
            ObjFileReader pikachuObj = new ObjFileReader(resource, false);
            var pikachu = pikachuObj.ObjToGroup();
            pikachu.Rotate(ry: Math.PI).Translate(tx: -4);
            Add(pikachu);

            var point = Helper.CreatePoint(10, 10, -10) / 2;
            Light(100, 100, -100);
        }
    }
}