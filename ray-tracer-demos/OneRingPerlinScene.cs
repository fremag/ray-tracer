using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class OneRingPerlinScene : AbstractScene
    {
        public OneRingPerlinScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 800, Height = 600,
                CameraX = -0, CameraY = 5, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);

            LavaField();
        }

        private void LavaField()
        {
            ColorMap lavaFieldMap = new ColorMap(
            (0, Yellow),
            (0.215, Yellow),
            (0.22, White),
            (0.225, Yellow),
            (0.8, Brown),
            (1, Brown/2)
            );
            IShape floor = new Plane();
            floor.Material.Pattern = new PerlinPattern(lavaFieldMap, 2).Rotate(ry: Pi/3).Translate(tz: -10);
            floor.Material.Transparency = 0;
            floor.Material.Diffuse = 0;
            floor.Material.Specular = 0;
            floor.Material.Ambient = 1;
            Add(floor);
        }
    }
}