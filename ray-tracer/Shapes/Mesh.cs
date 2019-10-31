namespace ray_tracer.Shapes
{
    public delegate double Altitude(double u, double v);

    public delegate void Curve2D(double u, double v, out double x, out double y);
    public delegate void Path3D(double t, out double x, out double y, out double z);

    public interface ICurve2D
    {
        void GetPoint(double u, double v, out double x, out double y);
    } 
    public interface IPath3D
    {
        void GetPoint(double t, out double x, out double y, out double z);
    }
    
    public abstract class AbstractCurve2D : ICurve2D
    {
        public abstract void GetPoint(double u, double v, out double x, out double y);
    }
    
    public class Curve2DAdapter : AbstractCurve2D
    {
        private Curve2D Func { get; }
        public Curve2DAdapter(Curve2D func)
        {
            Func = func;
        }

        public override void GetPoint(double u, double v, out double x, out double y)
        {
            Func(u, v, out x, out y);
        }
    }

    public abstract class AbstractPath3D : IPath3D
    {
        public abstract void GetPoint(double t, out double x, out double y, out double z);
    }
    
    public class Path3DAdapter : AbstractPath3D
    {
        private Path3D Func { get; }
        public Path3DAdapter(Path3D func)
        {
            Func = func;
        }

        public override void GetPoint(double t, out double x, out double y, out double z)
        {
            Func(t, out x, out y, out z);
        }
    }

    public abstract class AbstractMesh : Group
    {
        public int N { get; }
        public int M { get; }
        public Tuple[][] Points { get; }

        public AbstractMesh(int n, int m)
        {
            N = n;
            M = m;
            Points = new Tuple[n][];
            for (int i = 0; i < n; i++)
            {
                Points[i] = new Tuple[m];
            }
        }
    }
}