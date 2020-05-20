using System;

namespace ray_tracer.Shapes.Mesh
{
    public class SurfaceMesh : AbstractMesh
    {
        public SurfaceMesh(int n, int m) : base(n, m)
        {
        }

        public void Build(Func<double, double, double> funcX, Func<double, double, double> funcY, Func<double, double, double> funcZ)
        {
            for(int i=0; i < N; i++)
            {
                double u = i * 1.0 / N;
                for (int j = 0; j < N; j++)
                {
                    double v = j * 1.0 / M;
                    double x = funcX(u, v);
                    double y = funcY(u, v);
                    double z = funcZ(u, v);
                    Points[i][j] = Helper.CreatePoint(x, y, z);
                }
            }
        }
    }
}