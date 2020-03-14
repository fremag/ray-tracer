using System;
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

        public override void InitWorld()
        {
            Light(-10, 10, -10);
            
            DefaultFloor();
            
            const int n = 40;
            const double c = 2;
            const int nbSubGroup = 100;
            var isoSurface = new IsoSurface
            {
                Threshold = 3,
                XMin = -c, YMin = -c, ZMin = -c,
                XMax = c, YMax = c,  ZMax = c,
                Width = n, Height = n, Depth = n,
            };

            double SqrDist(double x0, double y0, double z0, double x1, double y1, double z1)
            {
                var x2 = (x0 - x1) * (x0 - x1);
                var y2 = (y0 - y1) * (y0 - y1);
                var z2 = (z0 - z1) * (z0 - z1);
                var r2 = x2 + y2 + z2;
                return r2;
            }
            
            double Cubic(double x0, double y0, double z0, double x1, double y1, double z1)
            {
                var x = Math.Abs(x0 - x1);
                var y = Math.Abs(y0 - y1);
                var z = Math.Abs(z0 - z1);
                var v = Math.Max(x, Math.Max(y, z));
                return v*v;
            }

            double Func(double x, double y, double z)
            {
                var v=  1/SqrDist(x, y, z, -1, 0, 0);
                v +=  1/Cubic(x, y, z, 1, 0, 0);
                v +=  1/SqrDist(x, y, z, 0, 1, 0);
                return v;
            }

            isoSurface.Init(Func);
            
            var material = new Material(White)
            {
                Reflective = 0,
                Ambient = 0.3,
                Diffuse = 0.4,
                Specular = 0.5,
                Transparency = 0,
                RefractiveIndex = 0.9
            };

            var shape1 = isoSurface.GetShape(true, 1e-9, nbSubGroup);
            shape1.Material = material;
            Add(shape1.Scale(0.75).Rotate(ry: -Pi/4).Translate(ty: 2, tx: -1));
            
            var shape2 = isoSurface.GetShape(false, 1e-7, nbSubGroup);
            shape2.Material = material;
            Add(shape2.Scale(0.75).Rotate(ry: -Pi/4).Translate(ty: 1, tx: 1));
        }
    }
}