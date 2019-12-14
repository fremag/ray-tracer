using System;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class CylinderAltitudeScene : AbstractScene
    {
        const int N = 100;

        public CylinderAltitudeScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 1600, Height = 1200,
                CameraX = N / 2, CameraY = N / 2, CameraZ = -N / 2,
                LookX = 0, LookY = 1, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            for (int i = 0; i < N; i++)
            {
                var group = new Group();
                for (int j = 0; j < N; j++)
                {
                    double x = (double) i / N - 0.5;
                    double z = (double) j / N - 0.5;
                    double r = Math.Sqrt(x * x + z * z);
                    double y = (1 + Math.Sin(Math.PI * 2 * r * 8)) * 2;
                    var p1 = Helper.CreatePoint(x * N, -1, z * N);
                    var p2 = Helper.CreatePoint(x * N, y, z * N);
                    var cyl = Helper.Cylinder(p1, p2, 0.5);
                    cyl.Material.Pattern = new SolidPattern(new Color((double) i / N, y / 4, (double) j / N));
                    cyl.Material.Transparency = 0;
                    group.Add(cyl);
                }

                Add(group);
            }

            var d = 3 * Math.Sqrt(N);
            var point = Helper.CreatePoint(0, d, -d);
            Light(2 * point);
        }
    }
}