namespace ray_tracer.Shapes.Functions
{
    public interface IPath3D
    {
        void GetPoint(double t, out double x, out double y, out double z);
    }
}