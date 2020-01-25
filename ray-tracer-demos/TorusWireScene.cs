using System;
using ray_tracer;
using ray_tracer.Cameras;
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
                Name = "Default", Width = 1600, Height = 1200,
                CameraX = -6, CameraY = 6, CameraZ = -6,
                LookX = 0, LookY = 0, LookZ = 0
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
            WireFrameMeshFactory factory = new WireFrameMeshFactory(r0, true, true);
            var torusWire = factory.Build(mesh);
            torusWire.Material = new Material(Magenta);
            Add(torusWire).Translate(ty: r2+r0);

            TriangleMeshFactory triFactory = new TriangleMeshFactory();
            var torusTriangle = triFactory.Build(mesh);
            torusTriangle.Material = new Material(Cyan) {Transparency = 0.99};
            Add(torusTriangle).Translate(ty: r2+r0);
            
            SphereMeshFactory sphereFactory = new SphereMeshFactory(2*r0);
            var torusSphere = sphereFactory.Build(mesh);
            torusSphere.Material = new Material(Yellow);
            Add(torusSphere).Translate(ty: r2+r0);
            
            DefaultFloor();
        }
    }
}