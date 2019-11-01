using System;
using ray_tracer;
using ray_tracer.Shapes;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos
{
    internal class CurveSweepScene : Scene
    {
        public CurveSweepScene()
        {
            DefaultFloor();
            Light(0, 5, -5);

            var curveSweepMesh = new CurveSweepMesh(36*10, 18, Path, Curve);
            var triangleFactory = new TriangleMeshFactory();
            var curveSweep = triangleFactory.Build(curveSweepMesh);
            Add(curveSweep.Rotate(rx: Pi/2).Translate(ty: 0.55));
        }

        private void Curve(double u, double v,  out double x, out double y)
        {
            double r = 0.05+0*Math.Cos(2*Pi*v);
            x = r*Math.Cos(2 * Pi * v);
            y = r*Math.Sin(2 * Pi * v);
        }

        private void Path(double t, out double x, out double y, out double z)
        {
            x = 0.5*Math.Cos(2 * Pi * 3*t);
            y = 0.5*Math.Cos(2 * Pi * t);
            z = 0.5*Math.Sin(2 * Pi *5* t);
        }
    }
}