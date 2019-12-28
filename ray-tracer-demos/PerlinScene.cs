using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class PerlinScene : AbstractScene
    {
        public PerlinScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 1, CameraZ = -5,
                LookX = 0, LookY = 1, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            ColorMap map1 = new ColorMap((0, White));
            IShape floor = new Plane();
            floor.Material.Pattern = new PerlinPattern(map1).Scale(1).Translate(-1000);

            ColorMap map2 = new ColorMap((0, Blue + Red), (0.5, Green + Red));
            var middle = Helper.Sphere().Translate(-0.5, 1, 0.5);
            middle.Material.Pattern = new PerlinPattern(map2).Scale(0.2).Translate(-1000).Rotate(Pi/4);

            ColorMap map3 = new ColorMap((0,  0.5, 1, 0.5), (1, 0, 0, 0));
            var right = new Cube().Scale(0.5, 0.5, 0.5).Translate(1.5, 0.5, -0.5);
            right.Material.Pattern = new PerlinPattern(map3).Scale(0.1).Translate(-1000);

            ColorMap map4 = new ColorMap((0, Red)).Add( (0.5, 1, 0.95, 0), (0.8, 1, 0.5, 0));
            var left = new Cone(-1, 0, true).Scale(0.8, 1, 0.8).Translate(-1.5, 1, -0.75);
            left.Material.Pattern = new PerlinPattern(map4).Scale(sx: 1).Translate(-1000);

            ColorMap map5 = new ColorMap(
                (0, 0, 0, 1), 
                (0.5, 0, 0, 1), 
                (0.6, 1,1,1), 
                (0.8, 0.5, 0.5, 0.5), 
                (1, 0, 0, 1));
            
            var sky = new Sphere().Scale(20);
            sky.Material.Pattern = new PerlinPattern(map5).Scale(0.05);

            Add(floor, middle, left, right);
            Add(sky);
            Light(-10, 10, -10);
        }
    }
}