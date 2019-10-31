namespace ray_tracer.Shapes
{
    public class CurveSweep : AbstractMesh
    {
        public CurveSweep(int n, int m, Path3D path, Curve2D curve, bool triangle = true) : this(n, m, new Path3DAdapter(path), new Curve2DAdapter(curve), triangle)
        {
            
        } 
        
        public CurveSweep(int n, int m, IPath3D path, ICurve2D curve, bool triangle=true) : base(n, m)
        {
            var vectorY = Helper.CreateVector(0, 1, 0);
            for(int i=0; i < n; i++)
            {
                double u = i * 1.0 / n;
                path.GetPoint(u, out var x0, out var y0, out var z0);
                path.GetPoint(u+0.001, out var x1, out var y1, out var z1);
                var tgt = Helper.CreateVector(x1-x0, y1-y0, z1-z0);
                var rotation = Helper.Rotation(vectorY, tgt.Normalize());
                
                for (int j = 0; j < m; j++)
                {
                    double v = j * 1.0 / m;
                    curve.GetPoint(u, v, out double cx, out double cy);
                    var transformPoint = rotation * Helper.CreateVector(cx, 0, cy);
                    double x = x0 + transformPoint.X;
                    double y = y0 + transformPoint.Y;
                    double z = z0 + transformPoint.Z;
                    
                    Points[i][j] = Helper.CreatePoint(x, y, z);
                }
            }

            Build(triangle);
        }

        private void Build(bool triangle)
        {
            for (int i = 0; i < N; i++)
            {
                var g = new Group();
                for (int j = 0; j < M; j++)
                {
                    var p0 = Points[i][j];
                    if (triangle)
                    {
                        var nextI = (i + 1) % N;
                        var nextJ = (j + 1) % M;
                        var p1 = Points[nextI][nextJ];
                        var tri1 = new Triangle(p0, p1, Points[i][nextJ]);
                        var tri2 = new Triangle(p0, p1, Points[nextI][j]);
                        g.Add(tri1);
                        g.Add(tri2);
                    } 
                    else
                    {
                        var sphere = new Sphere().Scale(0.01);
                        g.Add(sphere.Translate(p0));
                    }
                }

                Add(g);
            }
        }
    }
}