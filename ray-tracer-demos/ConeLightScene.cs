using ray_tracer;
using ray_tracer.Lights;

namespace ray_tracer_demos
{
    public class ConeLightScene : AbstractScene
    {
        public override void InitWorld()
        {
            Light(0, 10, 0, White/5);
            DefaultFloor(White, White/2);
            ConeLight light1 = new ConeLight(Helper.CreatePoint(0, 2, 0), Blue,  Helper.CreatePoint(-0.25, 1, 0), 0.1, 0.65);
            ConeLight light2 = new ConeLight(Helper.CreatePoint(0, 2, 0), Red,   Helper.CreatePoint(0.25, 1, 0), 0.1, 0.65);
            ConeLight light3 = new ConeLight(Helper.CreatePoint(0, 2, 0), Green, Helper.CreatePoint(0, 1, 0.25), 0.1, 0.65);
            Add(light1, light2, light3);
            
        }
    }
}