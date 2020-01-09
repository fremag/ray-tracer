using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class RingPerlinScene : AbstractScene
    {
        public RingPerlinScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 800, Height = 600,
                CameraX = -0, CameraY = -0, CameraZ = -4,
                LookX = 0, LookY = 0, LookZ = 1
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);

            StarField();
            Ring();
            RingSpace();
        }

        private void Ring()
        {
            var csgRing = new CsgDifference().Rotate(rx: Pi/2);;
            var outterRing = new Cylinder(-0.01, 0.01, true);
            double  ringThickness = 0.05;
            var innerRing = new Cylinder(-0.011, 0.011, true).Scale(sx: 1-ringThickness, sz: 1-ringThickness);
            csgRing.Init(outterRing, innerRing);

            double delta = 0.005;
            ColorMap ringMap = new ColorMap(
                (0, White/8),
                (0.3-delta, White/8),
                (0.3, Cyan/4),
                (0.3+delta, White/8),
                (0.5-delta, White/8),
                (0.5, Blue/2),
                (0.5+delta, White/8),
                (1, Blue/2)
            );
            double perlinScale = 15;
            var ringPattern = new PerlinPattern(ringMap, 8)
                .Translate(tx: 1 / perlinScale, tz: 1 / perlinScale)
                .Scale(sx: perlinScale, sz: 10*perlinScale, sy: perlinScale);
            csgRing.Material = new Material(ringPattern, ambient: 1, diffuse: 2, reflective: 1);
            
            Add(csgRing);
        }

        private void RingSpace()
        {
            var ring = new Cylinder(-0.01, 0.01, true).Rotate(rx: Pi/2);
            double delta = 0.025;
            var c1 = Blue/3;
            var c2 = Cyan/4;
            var c3 = Cyan/2+Yellow/8;
            ColorMap ringMap = new ColorMap(
                (0, c1),
                (0.50-delta, c2),
                (0.50, c3),
                (0.50+delta, c2),
                (1, c1)
                );
            double perlinScale = 15;
            ring.Material.Pattern = new PerlinPattern(ringMap, 8)
                .Translate(tx:1/ perlinScale, tz: 1/perlinScale)
                .Scale(perlinScale);
            ring.Material.Diffuse = 0;
            ring.Material.Specular = 0;
            ring.Material.Shininess = 0;
            ring.Material.Ambient = 0.5;
            
            Add(ring);
        }

        private void StarField()
        {
            ColorMap starFieldMap = new ColorMap(
            (0, White),
            (0.2, Black),
            (0.41999, Black),
            (0.42, White),
            (0.421, Black),
            (0.51999, Black),
            (0.52, White*0.8),
            (0.521, Black),
            (0.61999, Black),
            (0.62, White),
            (0.621, Black),
            (0.81999, Black),
            (0.82, White),
            (0.821, Black),
            (1, Black)
            );
            IShape floor = new Plane();
            floor.Rotate(rx:- Pi / 2).Translate(tz: 100);
            floor.Material.Pattern = new PerlinPattern(starFieldMap).Rotate(ry: Pi/3);
            floor.Material.Transparency = 0.5;
            floor.Material.Diffuse = 0;
            floor.Material.Specular = 0;
            floor.Material.Ambient = 1;
            Add(floor);
        }
    }
}