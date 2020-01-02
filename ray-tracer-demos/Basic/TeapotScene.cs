using System.IO;
using System.Reflection;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class TeapotScene : AbstractScene
    {
        public TeapotScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 2, CameraY = 4, CameraZ = -8,
                LookX = 0, LookY = 2, LookZ = 0});
        }

        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            Add(floor);

            var assembly = typeof(TeapotScene).GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("ray_tracer_demos.Basic.teapot.obj");
            ObjFileReader teapotObj = new ObjFileReader(resource, false);
            var teapot = teapotObj.ObjToGroup();
            Add(teapot);
            Light(15, 15, -15);
        }
    }
}