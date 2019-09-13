namespace ray_tracer
{
    public interface IPattern
    {
        Matrix Transform { get; set; }
        Color GetColor(Tuple point);
        Color GetColorAtShape(IShape shape, Tuple point);
    }
}