using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class MengerSpongeScene : AbstractScene
    {
        public MengerSpongeScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400, CameraX = 3, CameraY = 2, CameraZ = -12, LookX = -1.5, LookY = 1, LookZ = 0});
        }

        public override void InitWorld()
        {
            DefaultFloor().Translate(ty: -1.5);
            DefaultFloor().Translate(ty: -1.5);
            Light(0, 0, 0, Color.White);
            Light(0, 5, -20, Color.White/2);
            Light(10, 5, 0, Color.White/4);
            Light(0, 20, 0, Color.White/3);

            var sponge0 = new MengerSponge(0).Translate(tx: -6);
            var sponge1 = new MengerSponge(1).Translate(tx: -2);
            var sponge2 =  new MengerSponge(2).Translate(tx: 2);
            Add(sponge0, sponge1, sponge2);
        }
   }
}