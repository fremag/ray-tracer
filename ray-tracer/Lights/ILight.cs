namespace ray_tracer.Lights
{
    public interface ILight
    {
        public const int MAX_SAMPLE = 16*16;
        Tuple Position { get; }
        Color GetIntensityAt(ref Tuple point);
        unsafe int GetPositions(double* x, double* y, double* z);
    }
}