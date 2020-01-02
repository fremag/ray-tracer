using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class CsgBasicScene : AbstractScene
    {
        public CsgBasicScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 3, CameraZ = -4,
                LookX = 0, LookY = 1, LookZ = 0});
        }

        public override void InitWorld()
        {
            DefaultFloor();
            Light(0, 4, 0);
            Light(5, 15, -5, Color.White);

            var cube = new Cube();
            var sphere = new Sphere().Scale(1.4);
            var csgIntersection = new CsgIntersection(sphere, cube);
            
            var cyl1 = new Cylinder(-4, 4){Material = new Material(Red)}.Scale(sx: 0.75, sz: 0.75);
            var cyl2 = new Cylinder(-4, 4){Material = new Material(Green)}.Scale(sx: 0.75, sz: 0.75).Rotate(rx: Pi/2);
            var cyl3 = new Cylinder(-4, 4){Material = new Material(Blue)}.Scale(sx: 0.75, sz: 0.75).Rotate(rz: Pi/2);
            var csgUnion = new CsgUnion(cyl1,  new CsgUnion(cyl2, cyl3));

            var csgDifference = new CsgDifference(csgIntersection, csgUnion);
            csgDifference.Translate(ty: 1).Rotate(ry: Pi/4);
            Add(csgDifference);
        }
    }
}