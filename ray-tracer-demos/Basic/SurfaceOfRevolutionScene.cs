using System;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos.Basic
{
    internal class SurfaceOfRevolutionScene : AbstractScene
    {
        public SurfaceOfRevolutionScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 0.5, CameraZ = -1.5,
                LookX = 0, LookY = 0.25, LookZ = 0});
        }

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