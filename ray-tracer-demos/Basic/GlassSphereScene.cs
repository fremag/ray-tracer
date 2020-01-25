using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class GlassSphereScene : AbstractScene
    {
        public GlassSphereScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 0, CameraZ = -3,
                LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White) {Transform = Helper.Scaling(4, 4, 4)}, reflective: 0, specular: 0, diffuse: 0, shininess: 0, ambient: 1),
                Transform = Helper.Translation(0, -1, 0)
            };
            var s2 = new Sphere
            {
                Material = new Material(Color.White, transparency: 0.9, refractiveIndex: 1.5, reflective: 1, shininess: 300, specular: 0.9, ambient: 0, diffuse: 0.4)
            };

            Add(new[]
            {
                floor,
                s2
            });
            Light(-100, 0, -50);
        }
    }
}