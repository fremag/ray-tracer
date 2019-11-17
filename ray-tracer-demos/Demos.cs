using System;
using System.Diagnostics;
using System.IO;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class CubeScene : AbstractScene
    {
        public override void InitWorld()
        {
            const int n = 10;
            IShape floor = new Plane
            {
                Material = new Material(pattern: new CheckerPattern(colorA: Color.Black, colorB: Color.White).Scale(scale: n), reflective: 0.3),
                Transform = Helper.Translation(x: 0, y: -1, z: 0)
            };
            Add(floor);
            Random r = new Random(Seed: 0);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            for (int k = 0; k < n; k++)
            {
                var red = i / (double) n;
                var green = j / (double) n;
                var blue = k / (double) n;
                var color = new Color(red: red, green: green, blue: blue);
                double ambient = r.NextDouble() / 2 + 0.25;
                double diffuse = r.NextDouble() / 2 + 0.5;
                double specular = r.NextDouble() / 2 + 0.5;
                var cube = new Cube {Material = new Material(color: color, ambient: ambient, diffuse: diffuse, specular: specular)}.Scale(sx: 1 - r.NextDouble() + 0.5, sy: 1 - r.NextDouble() + 0.5, sz: 1 - r.NextDouble() + 0.5).Translate(tx: i, ty: j, tz: k);
                Add(cube);
            }

            var d = 5 * Math.Sqrt(d: n);
            var point = Helper.CreatePoint(x: d, y: d, z: -d);
            Light(2 * point);
        }
    }
}