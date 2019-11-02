using System;

namespace ray_tracer.Shapes.Mesh
{
    public class SurfaceOfRevolution : AbstractMesh
    {
        public SurfaceOfRevolution(int n, int m, Func1D radius) : base(n, m)
        {
            for(int i=0; i < n; i++)
            {
                double u = i * 1.0 / n;
                for (int j = 0; j < n; j++)
                {
                    double v = j * 1.0 / m;
                    var r = radius(u, v);
                    double x = r * Math.Cos(2 * Math.PI * v);
                    double y = u;
                    double z = r * Math.Sin(2 * Math.PI * v);
                    Points[i][j] = Helper.CreatePoint(x, y, z);
                }
            }
        }
    }
}