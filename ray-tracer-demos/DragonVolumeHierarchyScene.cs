using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;
using ray_tracer.Shapes.TriangleGroup;

namespace ray_tracer_demos
{
    public class DragonVolumeHierarchyScene : AbstractScene
    {
        public DragonVolumeHierarchyScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 1200, Height = 480,
                CameraX = 0, CameraY = 2.5, CameraZ = -10,
                LookX = 0, LookY = 1, LookZ = 0,
                FieldOfView = 1.2
            });
        }

        private IShape GetDragon(bool basicGroup = true)
        {
            ObjFileReader dragonReader = new ObjFileReader("dragon.obj", true);
            Group rawTriangles = dragonReader.DefaultGroup;
            IShape dragon = rawTriangles;
            if (!basicGroup)
            {
                //dragon = new TriangleGroupAvx(rawTriangles);
                dragon = new TriangleGroupGpu(rawTriangles);
            }

            dragon.Divide(16);
            dragon.Scale(0.268);
            return dragon;
        }

        private IShape RawBox() => new Cube {HasShadow = false}.Translate(1, 1, 1).Scale(3.7335, 2.5845, 1.6283).Translate(-3.9863, -0.1217, -1.1820);
        private IShape BBox() => RawBox().Translate(0, 0).Scale(0.268);
        private IShape Pedestal() => new Cylinder(-0.15, 0, true) {Material = new Material(Color.White * 0.2, ambient: 0, diffuse: 0.8, specular: 0, reflective: 0.2)};

        private void AddDragon(double scale, double angleY, Color color, double diffuse, double transparency, double tx, double ty, double tz, bool addBox = true)
        {
            var dragon = GetDragon(!false);
            dragon.Material = new Material(color, ambient: 0.1, diffuse: 0.6, specular: 0.3, shininess: 15);
            dragon.Scale(scale).Rotate(ry: angleY);
            Add(dragon.Translate(tx, ty, tz));
            if (addBox)
            {
                var box = BBox();
                box.Material = new Material {Ambient = 0, Diffuse = diffuse, Specular = 0, Transparency = transparency, RefractiveIndex = 1};
                box.Scale(scale).Rotate(ry: angleY);
                Add(box.Translate(tx, ty, tz));
            }

            Add(Pedestal().Translate(tx, ty, tz));
        }

        public override void InitWorld()
        {
            Light(-10, 100, -100);
            Light(0, 100, 0, White * 0.1);
            Light(100, 10, -25, White * 0.2);
            Light(-100, 10, -25, White * 0.2);

            AddDragon(1, 0, new Color(1, 0, 0.1), 0.4, 0.6, 0,2,0);
            AddDragon(0.75, 4, new Color(1, 0.5, 0.1), 0.2, 0.8, 2,1,-1);
            AddDragon(0.75, -0.4, new Color(0.9, 0.5, 0.1), 0.2, 0.8, -2, 0.75,-1);
            AddDragon(0.5, -0.2, new Color(1, 0.9, 0.1), 0.1, 0.9, -4, 0,-2);
            AddDragon(0.5, 3.3, new Color(0.9, 1, 0.1), 0.1, 0.9, 4, 0,-2);
            AddDragon(1, Pi, new Color(1, 1, 1), 0, 0, 0, 0.5, -4, false);
        }
    }
}