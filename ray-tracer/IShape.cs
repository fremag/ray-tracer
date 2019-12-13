namespace ray_tracer
{
    public interface IShape : ITransformable
    {
        IShape Parent { get; set; }
        Material Material { get; set; }
        Intersections Intersect(ref Tuple origin, ref Tuple direction);
        Tuple NormalAt(Tuple worldPoint, Intersection hit=null);
        Tuple WorldToObject(Tuple point);
        Tuple NormalToWorld(Tuple normal);
        Bounds Box { get; }
        bool Contains(IShape shape);
    }
}