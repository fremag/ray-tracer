namespace ray_tracer.Lights
{
    public interface ILight
    {
        Tuple Position { get; }
        Color Intensity { get; }
        double IntensityAt(Tuple point, World world);
    }
}