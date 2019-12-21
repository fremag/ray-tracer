using ray_tracer;
using ray_tracer.Lights;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class SoftShadowScene : AbstractScene
    {
        public SoftShadowScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 1600, Height = 1200,
                CameraX = 0, CameraY = 3, CameraZ = -7,
                LookX = 0, LookY = 0, LookZ = -2});
        }

        public override void InitWorld()
        {
            DefaultFloor(Color.White, Color._Blue);
            var corner = Helper.CreatePoint(0, 4, 10);
            var v1 = Helper.CreatePoint(1,0,0);
            var v2 = Helper.CreatePoint(0,1,0);
            Add(new AreaLight(corner, Color.White, v1, v2, 8, 16));

            var material = new Material(Color.White / 2, ambient: 0.6);
            Add(new Cube   {Material = material}.Scale(sx: 0.5, sy: 1, sz: 0.1).Translate(tx: -2));
            Add(new Sphere {Material = material}.Scale(sx: 1.0, sy: 1, sz: 1.0).Translate(tx: 0));
            Add(new Cube   {Material = material}.Scale(sx: 0.5, sy: 1, sz: 0.1).Translate(tx: 2));
        }
    }
}