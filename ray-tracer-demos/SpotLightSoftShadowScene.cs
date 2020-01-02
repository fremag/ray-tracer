using ray_tracer;
using ray_tracer.Lights;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class SpotLightSoftShadowScene : AbstractScene
    {
        public SpotLightSoftShadowScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400, CameraX = 0, CameraY = 1, CameraZ = -2, LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            Material material = new Material(Color.White, ambient: 0.025, diffuse: 0.67, specular: 0);
            DefaultFloor(White, Blue);
            Light(0, 10, -1000, White/2);
            SpotLight spotLight1 = new SpotLight(Helper.CreatePoint(0.5, 3, 0), White,  Helper.CreatePoint(0.5,2,0), 0.25, 0.4, ILight.MAX_SAMPLE);
            SpotLight spotLight2 = new SpotLight(Helper.CreatePoint(-0.5, 3, 0), White,  Helper.CreatePoint(-0.5,2,0), 0.25, 0.4, 1);
            Add(spotLight1);
            Add(spotLight2);

            Add(new Sphere {Material = material}.Scale(0.25).Translate(tx: 0.5, ty: 0.5));
            Add(new Sphere {Material = material}.Scale(0.25).Translate(tx: -0.5, ty: 0.5));
        }
    }
}