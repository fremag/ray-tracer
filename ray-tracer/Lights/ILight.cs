namespace ray_tracer.Lights
{
    public interface ILight
    {
        public const int MAX_SAMPLE = 16*16;
        Tuple Position { get; }
        Color Intensity { get; }
        unsafe int GetPositions(double* x, double* y, double* z);
    }
}