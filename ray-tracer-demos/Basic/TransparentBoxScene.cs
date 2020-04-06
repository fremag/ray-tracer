using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class TransparentBoxScene : AbstractScene
    {
        public TransparentBoxScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 2, CameraZ = -7,
                LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            Light(-4.9, 4.9, -1);

            DefaultFloor().Material = new Material(new CheckerPattern(Color.White * 0.35, Color.White * 0.65)
                {
                    //Transform = Helper.RotationY(45)
                }, reflective: 0.4, specular: 0);

            var sky = new Plane{Material = new Material(new SolidPattern(Blue)
            {
                Transform = Helper.RotationY(45)
            }, reflective: 0.4, specular: 0)}.Translate(ty: 10);
            Add(sky);
            
            Material material = new Material(new Color(1, 0, 0.2), ambient: 0, diffuse: 0.4, specular: 0.9, shininess: 300, reflective: 0.9, transparency: 0.9, refractiveIndex: 1);
            var c1 = new Cube {Material = material, HasShadow = false};
            Add(c1.Translate(-2));
            var s1 = new Sphere().Scale(0.2).Translate(tx: -2, ty: 0.2);
            Add(s1);

            var g = new Group() {HasShadow = true};
            var c2 = new Cube {Material = material, HasShadow = false};
            g.Add(c2);
            var s2 = new Sphere().Scale(0.2).Translate(ty: 0.2);
            g.Add(s2);
            Add(g.Translate(tx: 2));
        }
    }
}