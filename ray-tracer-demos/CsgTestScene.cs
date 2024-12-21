using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class CsgTestScene : AbstractScene
    {
        public CsgTestScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 1600, Height = 1200,
                CameraX = 0, CameraY = 4, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);
            IShape cyl = new Cylinder(0, 1, true);
            cyl.Scale(2, 1, 2);
            
            IShape cube1 = new Cube();
            cube1.Scale(1, 2, 1).Translate(-1.1, 0, 0.2);
            
            IShape cube2 = new Cube();
            cube2.Scale(1, 2, 1);
            
            IShape cube3 = new Cube();
            cube3.Scale(1.0, 2.0, 1.0).Translate(1.1, 0.0, -0.2 );

            IShape csg = new CsgDifference(cyl, new CsgUnion(new CsgUnion(cube1, cube2), cube3));
            
            Add(csg);
        }
    }
}