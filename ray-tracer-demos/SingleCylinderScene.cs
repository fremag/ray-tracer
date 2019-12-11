using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class SingleCylinderScene : AbstractScene
    {
        public SingleCylinderScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                CameraX = 0, CameraY = 1, CameraZ = -15,
                Height = 400, Width = 640,
                Name = "Default"
            });
        }

        public override void InitWorld()
        {
            Light(0, 5, -5);
            Add(new Cylinder(0, 1));
        }
    }
}