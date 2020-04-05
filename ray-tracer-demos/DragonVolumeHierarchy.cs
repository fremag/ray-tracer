using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class DragonVolumeHierarchy : AbstractScene
    {
        public DragonVolumeHierarchy()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 1200*2, Height = 480*2,
                CameraX = 0, CameraY = 2.5, CameraZ = -10,
                LookX = 0, LookY = 1, LookZ = 0,
                FieldOfView = 1.2
            });
        }

        private IShape GetDragon()
        {
            ObjFileReader dragonReader = new ObjFileReader("dragon.obj", true);
            var dragon = dragonReader.ObjToGroup();
            dragon.Scale(0.268).Translate(0, 0.1217, 0);
            return dragon;
        }

        private IShape RawBox() => new Cube {HasShadow = true}.Translate(1, 1, 1).Scale(3.7335, 2.5845, 1.6283).Translate(-3.9863, -0.1217, -1.1820);
        private IShape BBox() => RawBox().Translate(0, 0.1217).Scale(0.30);
        private IShape Pedestal() => new Cylinder(-0.15, 0, true) {Material = new Material(Color.White * 0.2, ambient: 0, diffuse: 0.8, specular: 0, reflective: 0.2)};

        private IShape MakeGroup(double scale, double angleY, Color color, double diffuse, double transparency, bool addBox = true)
        {
            var dragon = GetDragon();
            dragon.Material = new Material(color, ambient: 0.1, diffuse: 0.6, specular: 0.3, shininess: 15);

            var subGroup = new Group();
            subGroup.Add(dragon);

            if (addBox)
            {
                var box = BBox();
                box.Material = new Material {Ambient = 0, Diffuse = diffuse, Specular = 0, Transparency = transparency, RefractiveIndex = 1};
                subGroup.Add(box);
            }

            subGroup.Scale(scale).Rotate(ry: angleY);
            var g = new Group().Add(Pedestal(), subGroup);
            return g;
        }

        public override void InitWorld()
        {
            Light(-10, 100, -100);
            Light(0, 100, 0, White * 0.1);
            Light(100, 10, -25, White * 0.2);
            Light(-100, 10, -25, White * 0.2);

            var dragon1 = MakeGroup(1, 0, new Color(1, 0, 0.1), 0.4, 0.6).Translate(0,2,0);
            var dragon2 = MakeGroup(0.75, 4, new Color(1, 0.5, 0.1), 0.2, 0.8).Translate(2,1,-1);
            var dragon3 = MakeGroup(0.75, -0.4, new Color(0.9, 0.5, 0.1), 0.2, 0.8).Translate(-2, 0.75,-1);
            var dragon4 = MakeGroup(0.5, -0.2, new Color(1, 0.9, 0.1), 0.1, 0.9).Translate(-4, 0,-2);
            var dragon5 = MakeGroup(0.5, 3.3, new Color(0.9, 1, 0.1), 0.1, 0.9).Translate(4, 0,-2);
            var dragon6 = MakeGroup(1, Pi, new Color(1, 1, 1), 0, 0, false).Translate(0, 0.5, -4);

            var g = new Group();
            g.Add(dragon1, dragon2, dragon3, dragon4, dragon5, dragon6);
            g.Divide(10);

            Add(g);
        }
    }
}