using System;
using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class MengerSpongeScene : Scene
    {
        public MengerSpongeScene()
        {
            DefaultFloor().Translate(ty: -1);
            Light(5, 15, -5, Color.White);
            
            var cube = new Cube();
            var diff = new CsgDifference(cube, MakeCross());
            Add(diff);
        }

        private IShape MakeCross()
        {
            var c1 = new Cube().Scale(1.0 / 3, 1.1, 1.0 / 3);
            var c2 = new Cube().Scale(1.1, 1.0 / 3, 1.0 / 3);
            var c3 = new Cube().Scale(1.0 / 3, 1.0 / 3, 1.1);
            var union = new CsgUnion(c1, new CsgUnion(c2, c3));
            return union;
        }
    }
}