namespace ray_tracer.Shapes
{
    public class HeightField : AbstractMesh
    {
        public HeightField(int n, int m, Altitude func) : base(n, m)
        {
            for(int i=0; i < n; i++)
            {
                double u = i * 1.0 / n;
                for (int j = 0; j < n; j++)
                {
                    double v = j * 1.0 / m;
                    double x = -0.5 + u;
                    double y = func(u, v);
                    double z = -0.5 + v;
                    Points[i][j] = Helper.CreatePoint(x, y, z);
                }
            }

            Build();
        }

        private void Build()
        {
            for (int i = 1; i < N; i++)
            {
                for (int j = 1; j < M; j++)
                {
                    var tri1 = new Triangle(Points[i][j], Points[i-1][j-1], Points[i-1][j]);
                    var tri2 = new Triangle(Points[i][j], Points[i-1][j-1], Points[i][j-1]);
                    Add(tri1);
                    Add(tri2);
                }
            }
        }
    }
}