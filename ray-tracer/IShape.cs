namespace ray_tracer
{
    public interface IShape
    {
        Matrix Transform { get; set; }
        Material Material { get; set; }
        Intersections Intersect(Ray ray);
        Tuple NormalAt(Tuple worldPoint);
    }
}