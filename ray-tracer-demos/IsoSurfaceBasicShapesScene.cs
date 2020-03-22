using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes.IsoSurface;

namespace ray_tracer_demos
{
    public class IsoSurfaceBasicShapesScene : AbstractScene
    {
        public IsoSurfaceBasicShapesScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 800, Height = 600,
                CameraX = 0, CameraY = 2, CameraZ = -5,
                LookX = 0, LookY = 2, LookZ = 0
            });
        }

        private readonly Material material = new Material(White)
        {
            Reflective = 0,
            Ambient = 0.3,
            Diffuse = 0.4,
            Specular = 0.5,
            Transparency = 0,
            RefractiveIndex = 0.9
        };
        
        public override void InitWorld()
        {
            Light(-10, 10, -10);

            DefaultFloor();

            const int n = 4;
            const double c = 2;
            var sphere = InitSurface(true, n, c, new SphereField());
            Add(sphere.Translate(1, 1));
        }

        private IShape InitSurface(bool smooth, in int n, in double c, IScalarField field)
        {
            var isoSurface = new IsoSurface
            {
                Threshold = 1,
                XMin = -c, YMin = -c, ZMin = -c,
                XMax = c, YMax = c, ZMax = c,
                Width = n, Height = n, Depth = n,
            };

            isoSurface.Init(field);
            var shape = isoSurface.GetShape(smooth, 1e-9);
            shape.Material = material;
            return shape;
        }
    }
}