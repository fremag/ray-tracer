using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class CsgBasicScene : Scene
    {
        public CsgBasicScene()
        {
            DefaultFloor();

            var cube = new Cube();
            var cyl1 = new Cylinder(-4, 4){Material = new Material(Red)}.Scale(sx: 0.75, sz: 0.75);
            var cyl2 = new Cylinder(-4, 4){Material = new Material(Green)}.Scale(sx: 0.75, sz: 0.75).Rotate(rx: Pi/2);
            var cyl3 = new Cylinder(-4, 4){Material = new Material(Blue)}.Scale(sx: 0.75, sz: 0.75).Rotate(rz: Pi/2);
            var csgUnion = new CsgUnion(cyl1,  new CsgUnion(cyl2, cyl3));
            var diff = new CsgDifference(cube, csgUnion);
            diff.Translate(ty: 1).Rotate(ry: Pi/4);
            Add(diff);
            Light(0, 1, 0);
            Light(5, 5, -5, Color.White);
        }
    }
}