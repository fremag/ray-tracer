namespace ray_tracer
{
    public interface IShape : ITransformable
    {
        IShape Parent { get; set; }
        Material Material { get; set; }
        Intersections Intersect(Ray ray);
        Tuple NormalAt(Tuple worldPoint, Intersection hit=null);
        Tuple WorldToObject(Tuple point);
        Tuple NormalToWorld(Tuple normal);
        Bounds Box { get; }
    }
}