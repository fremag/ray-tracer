using System.IO;
using System.Reflection;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class TeapotScene : AbstractScene
    {
        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            Add(floor);

            var assembly = typeof(TeapotScene).GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("ray_tracer_demos.teapot.obj");
            ObjFileReader teapotObj = new ObjFileReader(resource, false);
            var teapot = teapotObj.ObjToGroup();
            Add(teapot);
            Light(15, 15, -15);
        }
    }
}