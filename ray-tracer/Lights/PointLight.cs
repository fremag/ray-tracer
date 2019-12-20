namespace ray_tracer.Lights
{
    public interface ILight
    {
        Tuple Position { get; }
        Color Intensity { get; }
    }
    
    
    
    public class PointLight : ILight
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