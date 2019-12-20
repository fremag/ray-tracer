using System;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class SoftShadowScene : AbstractScene
    {
        public SoftShadowScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 800, Height = 600,
                CameraX = 0, CameraY = 3, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = -2});
        }

        public override void InitWorld()
        {
            DefaultFloor();
            Light(0,5,10);

            Add(new Cube {Material = new Material(Color.White / 2, ambient: 0.6)}.Scale(0.5,1,0.1).Translate(-2));
            Add(new Cube {Material = new Material(Color.White / 2, ambient: 0.6)}.Scale(0.5,1,0.1).Translate(0));
            Add(new Sphere {Material = new Material(Color.White / 2, ambient: 0.6)}.Translate(2));
        }
    }
}