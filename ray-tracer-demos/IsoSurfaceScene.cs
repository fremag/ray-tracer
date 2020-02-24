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
                Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 0, CameraZ = -10,
                LookX = 0, LookY = 0, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            Light(-10, 10, -10);

            const int n = 50;
            const double c = 2;
            var isoSurface = new IsoSurface
            {
                Threshold = 3,
                XMin = -c, YMin = -c, ZMin = -c,
                XMax = c, YMax = c,  ZMax = c,
                Width = n, Height = n, Depth = n
            };

            double SqrDist(double x0, double y0, double z0, double x1, double y1, double z1)
            {
                var x2 = (x0 - x1) * (x0 - x1);
                var y2 = (y0 - y1) * (y0 - y1);
                var z2 = (z0 - z1) * (z0 - z1);
                var r2 = x2 + y2 + z2;
                return r2;
            }
            
            double Func(double x, double y, double z)
            {
                var v=  1/SqrDist(x, y, z, -1, 0, 0);
                v +=  1/SqrDist(x, y, z, 1, 0, 0);
                v +=  1/SqrDist(x, y, z, 0, 1, 0);
                return v;
            }

            isoSurface.Init(Func);

            Add(isoSurface);
        }
    }
}