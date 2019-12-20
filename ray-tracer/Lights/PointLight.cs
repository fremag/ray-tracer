namespace ray_tracer.Lights
{
    public class PointLight : ILight
    {
        public Color Intensity { get; }
        public double IntensityAt(Tuple point, World world)
        {
            
            var isShadowed = world.IsShadowed(point, this);
            return isShadowed ? 0 : 1;
        }

        public Tuple Position { get; }

        public PointLight(Tuple position, Color intensity)
        {
            Position = position;
            Intensity = intensity;
        }
    }
}