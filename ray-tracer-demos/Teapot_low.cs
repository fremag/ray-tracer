using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class Teapot_low : AbstractScene
    {
        public Teapot_low()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                CameraX = 0, CameraY = 7, CameraZ = 13,
                LookX = 0, LookY = 1, LookZ = 0,
                FieldOfView = 1.5
            });
        }

        public override void InitWorld()
        {
            Light(50, 100, 20, White / 2);
            Light(2, 50, 100, White / 2);

            /* Floor */
            Material planesMaterial = new Material(new CheckerPattern(White * 0.35, White * 0.4))
            {
                Ambient = 1,
                Diffuse = 0,
                Specular = 0,
                Reflective = 0.1
            };

            Add(new Plane
            {
                Material = planesMaterial
            });
            Add(new Plane
            {
                Material = new Material(new CheckerPattern(White * 0.35, White * 0.4))
                {
                    Ambient = 1,
                    Diffuse = 0,
                    Specular = 0,
                }
            }).Rotate(rx: Pi / 2).Translate(0, 0, -10);

            Material lowPoly = new Material(new Color(1, 0.3, 0.2))
            {
                Shininess = 5,
                Specular = 0.4
            };

            ObjFileReader teapotReader = new ObjFileReader("teapot-low.obj", true);
            var teapot = teapotReader.Groups[1];
            teapot.Scale(0.3).Rotate(rx: -Pi / 2).Rotate(ry: Pi * 23 / 22).Translate(7, 0, 3);
            teapot.Material = lowPoly;
            Add(teapot);

            ObjFileReader teapotReader2 = new ObjFileReader("teapot-low.obj", true);
            var teapot2 = teapotReader2.Groups[1];
            teapot2.Scale(0.3).Rotate(rx: -Pi / 2).Rotate(ry: -Pi * 46 / 22).Translate(-7, 0, 3);
            teapot2.Material = lowPoly;
            Add(teapot2);

            Material highPoly = new Material(new Color(0.3, 1, 0.2))
            {
                Shininess = 5,
                Specular = 0.4,
                Reflective = 0.5
            };

            ObjFileReader teapotReader3 = new ObjFileReader("_teapot.obj", true);
            var teapot3 = teapotReader3.Groups[1];
            teapot3.Scale(0.4).Rotate(rx: -Pi / 2).Rotate(ry: -Pi).Translate(0, 0, -5);
            teapot3.Material = highPoly;
            Add(teapot3);
        }
    }
}