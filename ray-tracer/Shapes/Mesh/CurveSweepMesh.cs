namespace ray_tracer.Shapes.Mesh
{
    public class CurveSweepMesh : AbstractMesh
    {
        public CurveSweepMesh(int n, int m, Path3D path, Curve2D curve) : this(n, m, new Path3DAdapter(path), new Curve2DAdapter(curve))
        {
            
        } 
        
        public CurveSweepMesh(int n, int m, IPath3D path, ICurve2D curve) : base(n, m)
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
        }
    }
}