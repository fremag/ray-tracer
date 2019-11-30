using System;
using ray_tracer;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos
{
    internal class SquareMeshScene : AbstractScene
    {
        public SquareMeshScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 1, CameraZ = -7,
                LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            DefaultFloor();
            Light(1, 1, -1);

            double Altitude(double u, double v)
            {
                var r = 8*Helper.Radius(u-0.5, v-0.5);
                return 0.2 * Math.Exp(-r/4)*(1+Math.Cos(2 * Pi * r));
            }

            var mesh = new HeightField(50, 50, Altitude);
            var triangleMeshFactory = new TriangleMeshFactory(false, false);
            Add(triangleMeshFactory.Build(mesh).Translate(-0.5));
            var sphereMeshFactory = new SphereMeshFactory(0.05);
            Add(sphereMeshFactory.Build(mesh).Translate(0.5));
        }
    }
}