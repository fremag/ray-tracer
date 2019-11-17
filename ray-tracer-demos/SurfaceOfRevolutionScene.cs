using System;
using ray_tracer;
using ray_tracer.Shapes;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos
{
    internal class SurfaceOfRevolutionScene : AbstractScene
    {
        public override void InitWorld()
        {
            DefaultFloor();
            Light(1, 1, -1);

            double Radius(double u, double v)
            {
                double r = Math.Exp(-u);
                double m = (1+Math.Abs(Math.Cos(2 * Math.PI * 1.5*u)));
                return r * m;
            }

            var mesh = new SurfaceOfRevolution(50, 50, Radius);
            var triangleMeshFactory = new TriangleMeshFactory(true, false);
            Add(triangleMeshFactory.Build(mesh).Scale(sy: 0.5, sx: 0.1, sz: 0.1).Translate(tx: 0, tz: 0));
        }
    }
}