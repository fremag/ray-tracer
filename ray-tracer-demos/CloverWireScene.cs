using System;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos
{
    public class CloverWireScene : AbstractScene
    {
        public CloverWireScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 1600, Height = 1200,
                CameraX = 0, CameraY = 0, CameraZ = -8,
                LookX = 0, LookY = 0, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);
            const double r0 = 0.02;
            const double r1 = 1;
            const double r2 = 0.25;

            void CloverPath(double t, out double x, out double y, out double z)
            {
                x = r1 * (Math.Cos(2 * Pi * t) + 2 * Math.Cos(2 * Pi * 2 * t));
                y = r1 * (Math.Sin(2 * Pi * t) - 2 * Math.Sin(2 * Pi * 2 * t));
                z = 2 * r2 * Math.Sin(2 * Pi * 3 * t);
            }

            void CircleCurve(double u, double v, out double x, out double y)
            {
                x = r2 * Math.Cos(2 * Pi * v);
                y = r2 * Math.Sin(2 * Pi * v);
            }

            CurveSweepMesh mesh = new CurveSweepMesh(80, 15, CloverPath, CircleCurve);
            WireFrameMeshFactory factory = new WireFrameMeshFactory(r0, true, true);
            var cloverWire = factory.Build(mesh);
            cloverWire.Material = new Material(Magenta);
            Add(cloverWire);
        }
    }
}