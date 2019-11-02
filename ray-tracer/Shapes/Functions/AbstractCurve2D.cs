namespace ray_tracer.Shapes.Functions
{
    public abstract class AbstractCurve2D : ICurve2D
    {
        public abstract void GetPoint(double u, double v, out double x, out double y);
    }
}