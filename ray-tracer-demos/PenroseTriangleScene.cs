using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class PenroseTriangleScene : AbstractScene
    {
// https://github.com/POV-Ray/povray/blob/2d35fd4b36ad19e3c17273101f8f71a2ce7924aa/source/core/render/tracepixel.cpp#L375
// http://www.cs.cornell.edu/courses/cs4620/2013fa/lectures/02view-ray.pdf
// http://www.f-lohmueller.de/pov_tut/x_sam/sam_430f.htm
// http://www.f-lohmueller.de/pov_tut/camera_light/camera_f1.htm
        public PenroseTriangleScene()
        {
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 400, Height = 400,
                CameraX = 0, CameraY = 0, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 0
            });
            CameraParameters.Clear();
            CameraParameters.Add(new OrthographicCameraParameters(0,4,0, 4,0,0)
            {
                Name = "Default", Width = 400, Height = 400,
                CameraX = -2, CameraY = -2, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 1
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);
            Add(new Cube().Rotate(rz: Pi/4, ry: Pi/4));
        }
    }
}