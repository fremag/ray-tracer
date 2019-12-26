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
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 2, CameraZ = -5,
                LookX = 0, LookY = 1, LookZ = 0});

        }

        public override void InitWorld()
        {
            IShape floor = new Plane();
            ColorMap map1 = new ColorMap();
            map1.Add(0, Color._Blue);
            ColorMap map2 = new ColorMap();
            map2.Add(0, Color._Blue+Color._Red);
            map2.Add(0.5, Color._Green+Color._Red);
            ColorMap map3 = new ColorMap();
            map3.Add(0, new Color(0.5, 0.5, 1));
            map3.Add(1, Color.Black);
            ColorMap map4 = new ColorMap();
            map4.Add(0, Color._Red);
            map4.Add(0.5, new Color(1, 0.95, 0));
            map4.Add(0.8, new Color(1, 0.5, 0));
            
            floor.Material = new Material
            {
                Pattern = new PerlinPattern(map1).Scale(3).Translate(-1000)
            };

            var middle = Helper.Sphere().Translate(-0.5, 1, 0.5);
            middle.Material = new Material
            {
                Pattern = new PerlinPattern(map2).Scale(0.2).Translate(-1000)
            };

            var right = Helper.Sphere().Scale(0.5, 0.5, 0.5).Translate(1.5, 0.5, -0.5);
            right.Material = new Material
            {
                Pattern = new PerlinPattern(map3).Scale(0.1).Translate(-1000)
            };

            var left = Helper.Sphere().Scale(1.0/3).Translate(-1.5, 0.33, -0.75);
            left.Material = new Material()
            {
                Pattern = new PerlinPattern(map4).Scale(sx: 0.25).Translate(-1000)
            };

            Add(floor, middle, left, right);
            Light(-10, 10, -10);
        }
    }
}