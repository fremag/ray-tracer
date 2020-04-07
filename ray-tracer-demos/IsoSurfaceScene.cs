using System;
using System.Threading.Tasks;
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

            const int n = 100;
            const double c = 3;
            IShape shape1 = null;
            IShape shape2 = null;
            IShape shape3 = null;
            IShape shape4 = null;

            Action[] actions = {
                () => shape1 = InitSurfaceSphereCube(true, n, c).Scale(0.75).Translate(tx: -1, ty: 1),
                () => shape2 = InitSurfaceSphereCube(false, n, c).Scale(0.75).Translate(tx: -1, ty: 2.5),
                () => shape3 = InitSurfaceConeCylinder(true, n, c).Scale(0.75).Translate(tx: 1.5),
                () => shape4 = InitSurfaceConeCylinder(false, n, c).Scale(0.75).Translate(tx: 1.5, ty: 2)

            };
            Parallel.ForEach(actions, action => action());
            
            SafeAdd(shape1.Divide(5));
            SafeAdd(shape2.Divide(5));
            SafeAdd(shape3.Divide(5));
            SafeAdd(shape4.Divide(5));
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