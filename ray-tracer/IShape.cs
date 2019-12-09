using System.Numerics;

namespace ray_tracer
{
    public interface IShape : ITransformable
    {
        IShape Parent { get; set; }
        Material Material { get; set; }
        Intersections Intersect(Ray ray);
        Intersections Intersect(ref Vector4 origin, ref Vector4 direction);
        Tuple NormalAt(Tuple worldPoint, Intersection hit=null);
        Tuple WorldToObject(Tuple point);
        Tuple NormalToWorld(Tuple normal);
        Bounds Box { get; }
        bool Contains(IShape shape);
    }
}