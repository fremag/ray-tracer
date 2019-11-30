using System;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class CylinderScene : AbstractScene
    {
        public CylinderScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 6, CameraY = 5, CameraZ = -8,
                LookX = 3, LookY = 2, LookZ = 0});
        }

        private void AddObject(double tx, double ty, double tz, int n)
        {
            var sphere = new Sphere().Scale(0.3).Translate(tx, ty, tz);
            var cyl1 = new Cylinder {Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(ry: Math.PI / 2).Translate(tx, ty, tz);
            var cyl2 = new Cylinder {Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(rx: Math.PI / 2).Translate(tx, ty, tz);
            var cyl3 = new Cylinder {Closed = true, Minimum = 0, Maximum = 1}.Scale(sx: 0.15, sz: 0.15).Rotate(rz: Math.PI / 2).Translate(tx, ty, tz);

            cyl1.Material.Pattern = new SolidPattern(Color._Red * (tx / n));
            cyl2.Material.Pattern = new SolidPattern(Color._Green * (ty / n));
            cyl3.Material.Pattern = new SolidPattern(Color._Blue * (tz / n));

            var red = tx / n;
            var green = ty / n;
            var blue = tz / n;
            var color = new Color(red, green, blue);
            sphere.Material.Pattern = new SolidPattern(color);
            Add(sphere, cyl1, cyl2, cyl3);
        }

        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White).Scale(1), reflective: 0.3, transparency: 0.9),
                Transform = Helper.Translation(0, -0.5, 0)
            };
            Add(floor);

            const int n = 5;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            for (int k = 0; k < n; k++)
                AddObject(i, j, k, n);

            var d = 3 * Math.Sqrt(n);
            var point = Helper.CreatePoint(d, d, -d);
            Light(2 * point);
        }
    }
}