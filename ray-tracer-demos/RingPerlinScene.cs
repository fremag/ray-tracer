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
                Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 1, CameraZ = -5,
                LookX = 0, LookY = 1, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);

            ColorMap map1 = new ColorMap(
            (0, White),
            (0.1, Black),
            (0.399, Black),
            (0.4, White),
            (0.5, Black),
            (0.9, Black),
            (1, White)
            );
            IShape floor = new Plane();
            floor.Material.Pattern = new PerlinPattern(map1).Scale(1).Translate(-1000);
            Add(floor);
        }
    }
}