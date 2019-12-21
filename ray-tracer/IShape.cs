namespace ray_tracer
{
    public interface IShape : ITransformable
    {
        int Id { get; }
        IShape Parent { get; set; }
        Material Material { get; set; }
        void Intersect(ref Tuple origin, ref Tuple direction, Intersections intersections);
        Tuple NormalAt(Tuple worldPoint, Intersection hit=null);
        Tuple WorldToObject(Tuple point);
        Tuple NormalToWorld(Tuple normal);
        Bounds Box { get; }
        bool Contains(IShape shape);
        bool HasShadow { get; }
    }
}