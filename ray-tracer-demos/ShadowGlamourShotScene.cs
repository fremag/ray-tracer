using ray_tracer;
using ray_tracer.Lights;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class ShadowGlamourShotScene : AbstractScene
    {
        public ShadowGlamourShotScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 1200*2, Height = 480*2,
                CameraX = -3, CameraY = 1, CameraZ = 2.5,
                LookX = 0, LookY = 0.5, LookZ = 0});
        }

        public override void InitWorld()
        {
            var corner = Helper.CreatePoint(-1, 2, 4);
            var v1 = Helper.CreatePoint(2, 0, 0);
            var v2 = Helper.CreatePoint(0, 2, 0);
            Add(new AreaLight(corner, Color.White * 1.5, v1, v2, 8, 8));

            Add(new Cube {Material = new Material(Color.White * 1.5, ambient: 1, diffuse: 0, specular: 0), HasShadow = false}.Scale(sx: 1, sy: 1, sz: 0.01).Translate(tx: 0, 3, 4));
            Add(new Plane {Material = new Material(Color.White, ambient: 0.025, diffuse: 0.67, specular: 0)});
            Add(new Sphere {Material = new Material(Color._Red, ambient: 0.1, specular: 0, diffuse: 0.6, reflective: 0.3)}.Scale(0.5).Translate(0.5, 0.5, 0));
            Add(new Sphere {Material = new Material(new Color(0.5, 0.5, 1), ambient: 0.1, specular: 0, diffuse: 0.6, reflective: 0.3)}.Scale(0.33).Translate(-0.25, 0.33, 0));
        }
    }
}