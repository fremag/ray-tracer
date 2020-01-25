using System;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos.Basic
{
    internal class CurveSweepScene : AbstractScene
    {
        public CurveSweepScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 1.5, CameraZ = -1.5,
                LookX = 0, LookY = 0.5, LookZ = 0});
            
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

        public override void InitWorld()
        {
            DefaultFloor();
            Light(0, 5, -5);

            var curveSweepMesh = new CurveSweepMesh(36*10, 18, Path, Curve);
            var triangleFactory = new TriangleMeshFactory();
            var curveSweep = triangleFactory.Build(curveSweepMesh);
            Add(curveSweep.Rotate(rx: Pi/2).Translate(ty: 0.55));
        }
    }
}