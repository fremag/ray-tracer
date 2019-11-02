namespace ray_tracer.Shapes.Mesh
{
    public delegate double Func1D(double u, double v);

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

    public abstract class AbstractPath3D : IPath3D
    {
        public abstract void GetPoint(double t, out double x, out double y, out double z);
    }
}