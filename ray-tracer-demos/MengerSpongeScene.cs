using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class MengerSpongeScene : AbstractScene
    {
        public MengerSpongeScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 1280, Height = 1024, CameraX = 0, CameraY = 2, CameraZ = -2, LookX = 0, LookY = 0, LookZ = -1});
        }

        public override void InitWorld()
        {
            DefaultFloor().Translate(ty: -1.5);
            Light(-1, 5, -5, Color.White/2);
            Light(-5, 5, -1, Color.White/2);

            var sponge2 = new MengerSponge(2);
            var cube = new Cube().Scale(sx: 2, sz: 2).Translate(ty: 1);
            var diff = new CsgDifference(sponge2, cube);
            Add(diff);
        }
   }
}