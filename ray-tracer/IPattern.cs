namespace ray_tracer
{
    public interface IPattern : ITransformable
    {
        Color GetColor(Tuple point);
        Color GetColorAtShape(IShape shape, Tuple point);
    }
}