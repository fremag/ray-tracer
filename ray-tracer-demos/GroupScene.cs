using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class GroupScene : AbstractScene
    {
        public static IShape CreatePyramid(int n = 5)
        {
            var g = new Group();
            for (int i = 1; i <= n; i++)
            {
                var cube = new Cube().Scale(sx: (n - i + 1) * 1.0 / n, sy: 0.1, sz: (n - i + 1) * 1.0 / n).Translate(ty: i * 1.0 / n);
                cube.Material.Pattern = new SolidPattern(Color._Red * i / n);
                cube.Material.Ambient = 0.1;
                g.Add(cube);
            }

            return g;
        }

        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            Add(floor);

            const int n = 4;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                var pyramid = CreatePyramid(10).Translate(tx: 2 * i, tz: 2 * j);
                Add(pyramid);
            }

            var point = Helper.CreatePoint(1, 1, -1) * 6;
            Light(2 * point);
        }
    }
}