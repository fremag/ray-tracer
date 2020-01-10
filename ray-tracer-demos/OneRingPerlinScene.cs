using System;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos
{
    public class OneRingPerlinScene : AbstractScene
    {
        public OneRingPerlinScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 800, Height = 600,
                CameraX = -0, CameraY = 4, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);

            LavaField();
            TheOneRing();
        }

        private void TheOneRing()
        {
            const double r1 = 1;
            const double r2 = 0.2;

            void CirclePath(double t, out double x, out double y, out double z)
            {
                x = r1 * Math.Cos(2 * Pi * t);
                y = 0;
                z = r1 * Math.Sin(2 * Pi * t);
            }

            void CircleCurve(double u, double v, out double x, out double y)
            {
                x = r2 * 0.5* Math.Cos(2 * Pi * v);
                y = r2 * Math.Sin(2 * Pi * v);
            }

            CurveSweepMesh mesh = new CurveSweepMesh(100, 40, CirclePath, CircleCurve);

            TriangleMeshFactory triFactory = new TriangleMeshFactory();
            var torusTriangle = triFactory.Build(mesh);
            torusTriangle.Material = new Material((Yellow+Brown)/2, reflective: 1, diffuse: 1, specular: 2 ,ambient: 0);
            Add(torusTriangle).Translate(ty: r2);
        }

        private void LavaField()
        {
            ColorMap lavaFieldMap = new ColorMap(
                (0, Yellow * 0.8),
                (0.31, Yellow * 0.9),
                (0.32, White * 0.9),
                (0.33, Yellow),
                (0.7, Brown),
                (1, Brown / 4)
            );
            var lavaPattern = new PerlinPattern(lavaFieldMap, 3).Rotate(ry: Pi / 3).Translate(tz: -10);
            IShape floor = new Plane
            {
                Material = new Material(lavaPattern) {Diffuse = 0, Specular = 0, Ambient = 1}
            };
            Add(floor);
        }
    }
}