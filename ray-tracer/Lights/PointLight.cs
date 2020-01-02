namespace ray_tracer.Lights
{
    public class PointLight : ILight
    {
        public Color Intensity { get; }
        public Tuple Position { get; }

        public PointLight(Tuple position, Color intensity)
        {
            Position = position;
            Intensity = intensity;
        }

        public Color GetIntensityAt(double x, double y, double z, ref Tuple point) => Intensity;
        
        public unsafe int GetPositions(double* x, double* y, double* z)
        {
            x[0] = Position.X;
            y[0] = Position.Y;
            z[0] = Position.Z;
            return 1;
        }
    }
}