using System;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class CylinderAltitudeScene : AbstractScene
    {
        public CylinderAltitudeScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 35, CameraY = 35, CameraZ = -35,
                LookX = 35, LookY = 1, LookZ = 35});

        }

        public override void InitWorld()
        {
            const int n = 70;
            for (int i = 0; i < n; i++)
            {
                var group = new Group(); 
                for (int j = 0; j < n; j++)
                {
                    double x = (double) i / n - 0.5;
                    double z = (double) j / n - 0.5;
                    double r = Math.Sqrt(x * x + z * z);
                    double y = (1 + Math.Sin(Math.PI * 2 * r * 8)) * 2;
                    var cyl = new Cylinder(minimum: 0, maximum: 1, closed: true).Scale(sx: 0.5, sy: y, sz: 0.5).Translate(tx: i, tz: j);
                    cyl.Material.Pattern = new SolidPattern(new Color((double) i / n, y / 4, (double) j / n));
                    cyl.Material.Transparency = 0.9;
                    group.Add(cyl);
                }

                Add(group);
            }

            var d = 3 * Math.Sqrt(n);
            var point = Helper.CreatePoint(0, d, -d);
            Light(2 * point);
        }
    }
}