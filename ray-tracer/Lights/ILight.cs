namespace ray_tracer.Lights
{
    public interface ILight
    {
        Tuple Position { get; }
        Color Intensity { get; }
        unsafe int GetPositions(double* x, double* y, double* z);
    }
}