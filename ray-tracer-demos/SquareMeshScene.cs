using System;
using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    internal class SquareMeshScene : Scene
    {
        public SquareMeshScene()
        {
            DefaultFloor();
            Light(1, 1, -1);

            double Altitude(double u, double v)
            {
                var r = 8*Helper.Radius(u-0.5, v-0.5);
                return 0.2 * Math.Exp(-r/4)*(1+Math.Cos(2 * Pi * r));
            }

            var mesh = new HeightField(50, 50, Altitude);
            Add(mesh);
        }
    }
}