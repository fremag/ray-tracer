namespace ray_tracer
{
    public class PointLight
    {
        public Color Color { get; }
        public Tuple Position { get; }

        public PointLight(Tuple position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}