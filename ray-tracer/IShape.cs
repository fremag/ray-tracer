namespace ray_tracer
{
    public interface IShape : ITransformable
    {
        Material Material { get; set; }
        Intersections Intersect(Ray ray);
        Tuple NormalAt(Tuple worldPoint);
    }
}