using System;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class PikachuScene : AbstractScene
    {
        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White).Scale(5))
            };

            Add(floor);

            ObjFileReader smoothPikachuObj = new ObjFileReader("pikachu.obj", true);
            var smoothPikachu = smoothPikachuObj.ObjToGroup();
            smoothPikachu.Rotate(ry: Math.PI).Translate(tx: 0.5);
            Add(smoothPikachu);

            ObjFileReader pikachuObj = new ObjFileReader("pikachu.obj", false);
            var pikachu = pikachuObj.ObjToGroup();
            pikachu.Rotate(ry: Math.PI).Translate(tx: -4);
            Add(pikachu);

            var point = Helper.CreatePoint(10, 10, -10) / 2;
            Light(100, 100, -100);
        }
    }
}