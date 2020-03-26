using System;
using System.Threading.Tasks;
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

            var lambda1 = new Action(() =>
            {
                var sphere = InitSurface(true, 60, 2, new SphereField());
                SafeAdd(sphere.Scale(0.5).Translate(1, 1));
            });

            var lambda2 = new Action(() =>
            {
                var cone = InitSurface(true, 60, 2, new ConeField());
                SafeAdd(cone.Scale(0.5).Translate(-1, 1));
            });
            var lambda3 = new Action(() =>
            {
                var cube = InitSurface(true, 60, 2, new CubeField());
                SafeAdd(cube.Scale(0.5).Translate(-1, 2));
            });
            var lambda4 = new Action(() =>
            {
                var cylinder = InitSurface(true, 60, 2, new CylinderField());
                SafeAdd(cylinder.Scale(0.5).Translate(1, 2));
            });
            var actions = new[] {lambda1, lambda2, lambda3, lambda4};
            Parallel.ForEach(actions, action => action());
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