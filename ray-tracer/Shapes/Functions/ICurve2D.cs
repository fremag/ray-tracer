namespace ray_tracer.Shapes.Functions
{
    public interface ICurve2D
    {
        void GetPoint(double u, double v, out double x, out double y);
    }
}