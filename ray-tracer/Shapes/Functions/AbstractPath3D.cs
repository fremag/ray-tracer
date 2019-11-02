namespace ray_tracer.Shapes.Functions
{
    public abstract class AbstractPath3D : IPath3D
    {
        public abstract void GetPoint(double t, out double x, out double y, out double z);
    }
}