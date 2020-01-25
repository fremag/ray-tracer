using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class IcosahedronScene : AbstractScene
    {
        public IcosahedronScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 480,
                CameraX = 4, CameraY = 1, CameraZ = -7,
                LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            Light(-10, 10, 0);
            Light(0, 0, 0);

            Add(new Icosahedron(1, 0.1).Rotate(ry: Pi/2));
            Add(new Icosahedron(3, 0.1));
        }
    }
}