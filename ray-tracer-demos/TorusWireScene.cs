using System;
using ray_tracer;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos
{
    public class TorusWireScene : AbstractScene
    {
        public TorusWireScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 640, Height = 400,
                CameraX = -10, CameraY = 8, CameraZ = -10,
                LookX = 0, LookY = 1, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);
            const double r0 = 0.05;
            const double r1 = 3;
            const double r2 = 1;

            void CirclePath(double t, out double x, out double y, out double z)
            {
                x = r1 * Math.Cos(2 * Pi * t);
                y = 0;
                z = r1 * Math.Sin(2 * Pi * t);
            }

            void CircleCurve(double u, double v, out double x, out double y)
            {
                x = r2 * Math.Cos(2 * Pi * v);
                y = r2 * Math.Sin(2 * Pi * v);
            }

            CurveSweepMesh mesh = new CurveSweepMesh(40, 10, CirclePath, CircleCurve);
            WireFrameMeshFactory factory = new WireFrameMeshFactory(r0, 0*r0, true, true);
            var torus = factory.Build(mesh);
            TriangleMeshFactory triFactory = new TriangleMeshFactory();
            var torusTriangle = triFactory.Build(mesh);
            Add(torus).Translate(ty: r2+r0);
            Add(torusTriangle).Translate(ty: r2+r0);
            torusTriangle.Material = new Material(Blue) {Transparency = 0.9};
            DefaultFloor();
        }
    }
}