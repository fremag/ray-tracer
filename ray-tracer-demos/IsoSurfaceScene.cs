using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes.IsoSurface;

namespace ray_tracer_demos
{
    public class IsoSurfaceScene : AbstractScene
    {
        public IsoSurfaceScene()
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

            const int n = 50;
            const double c = 3;
            var shape1 = InitSurfaceSphereCube(true, n, c);
            Add(shape1.Scale(0.75).Translate(tx: -1, ty: 1));

            var shape2 = InitSurfaceSphereCube(false, n, c);
            Add(shape2.Scale(0.75).Translate(tx: -1, ty: 2.5));

            var shape3 = InitSurfaceConeCylinder(true, n, c);
            Add(shape3.Scale(0.75).Translate(tx: 1.5));

            var shape4 = InitSurfaceConeCylinder(false, n, c);
            Add(shape4.Scale(0.75).Translate(tx: 1.5, ty: 2));
        }

        private IShape InitSurfaceConeCylinder(bool smooth, in int n, in double c)
        {
            var isoSurface = new IsoSurface
            {
                Threshold = 3,
                XMin = -c, YMin = -c, ZMin = -c,
                XMax = c, YMax = c, ZMax = c,
                Width = n, Height = n, Depth = n,
            };

            IScalarField field = new InvSqrSumField(
                new ConeField().Translate(0,2,0)
                , new CylinderField().Translate(0,1,0)
            );
            isoSurface.Init(field);
            var shape = isoSurface.GetShape(smooth, 1e-9);
            shape.Material = material;
            return shape;
        }

        private IShape InitSurfaceSphereCube(bool smooth, in int n, in double c)
        {
            var isoSurface = new IsoSurface
            {
                Threshold = 3,
                XMin = -c, YMin = -c, ZMin = -c,
                XMax = c, YMax = c, ZMax = c,
                Width = n, Height = n, Depth = n,
            };

            IScalarField field = new InvSqrSumField(
                new SphereField().Translate(0,1,0)
                , new CubeField().Translate(-1,0,0)
                , new SphereField().Translate(1,0,0)
            );
            isoSurface.Init(field);
            var shape = isoSurface.GetShape(smooth, 1e-10);
            shape.Material = material;
            return shape;
        }
    }
}