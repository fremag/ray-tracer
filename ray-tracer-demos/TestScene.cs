using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using ray_tracer.Shapes.IsoSurface;

namespace ray_tracer_demos
{
    public class TestScene : AbstractScene
    {
        public TestScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 0, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-3, 3, -3);

            var wall = new Plane().Rotate(rx: Pi/2).Translate(tz:2);
            wall.Material = new Material(new CheckerPattern(White, Black).Scale(1.0/2.5));
            Add(wall);
            var sphere = new Sphere
            {
                Material = new Material(new SolidPattern(White))
                {
                    Ambient = 0.1,
                    Diffuse = 0.9,
                    Specular = 0.9,
                    Shininess = 200,
                    Reflective = 0.1,
                    Transparency = 0.9,
                    RefractiveIndex = 1.5
                }
            };

            Add(sphere);
        }
    }
}