namespace ray_tracer
{
    public class PointLight
    {
        public Color Intensity { get; }
        public Tuple Position { get; }

        public PointLight(Tuple position, Color intensity)
        {
            Position = position;
            Intensity = intensity;
        }
    }
}