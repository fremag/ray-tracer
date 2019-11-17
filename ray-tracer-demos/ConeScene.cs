using System;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class ConeScene : AbstractScene
    {
        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            Add(floor);

            const int n = 200;
            for (int i = 1; i <= n; i++)
            {
                var t = (double) i / n;
                var x = 4 * t * Math.Cos(2 * Math.PI * t * 4);
                var z = 4 * t * Math.Sin(2 * Math.PI * t * 4);
                var cone = new Cone(-1, 0, true).Translate(ty: 1).Scale(sy: 1 - t + 0.5).Translate(tx: x, tz: z);
                cone.Material.Pattern = new SolidPattern(Color._Blue * t + Color._Green * (1 - t));
                Add(cone);
            }

            var point = Helper.CreatePoint(0, 5, -5);
            Light(2 * point);
        }
    }
}