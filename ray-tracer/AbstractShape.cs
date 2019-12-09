#define OPTIM_WORLD_TO_OBJECT
#define OPTIM_INTERSECT
using System.Numerics;

namespace ray_tracer
{
    public abstract class AbstractShape : IShape
    {
        public Matrix Transform { get; set; } = Matrix.Identity;
        public IShape Parent { get; set; }
        public Material Material { get; set; } = new Material();

        public abstract Intersections IntersectLocal(Ray ray);
        public virtual Intersections IntersectLocal(ref Vector4 origin, ref Vector4 direction)
        {
            var o = Helper.CreatePoint(origin.X, origin.Y, origin.Z);
            var d = Helper.CreateVector(direction.X, direction.Y, direction.Z);
            var ray = new Ray(o, d);
            return IntersectLocal(ray);
        }
        
        public abstract Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null);
        public abstract Bounds Box { get; }
        
        public virtual bool Contains(IShape shape)
        {
            return ReferenceEquals(shape, this);
        }

        public Intersections Intersect(Ray ray)
        {
            return Intersect(ref ray.Origin.vector, ref ray.Direction.vector);
        }

        public Intersections Intersect(ref Vector4 origin, ref Vector4 direction)
        {
            Intersections intersections;
            if (! ReferenceEquals(Transform, Matrix.Identity))
            {
                var invTransform = Transform.Inverse().matrix;
                var transformedOrigin = Vector4.Transform(origin, invTransform);
                var transformedDirection = Vector4.Transform(direction, invTransform);
                intersections=  IntersectLocal(ref transformedOrigin, ref transformedDirection);
            }
            else
            {
                intersections = IntersectLocal(ref origin, ref direction);
            }

            return intersections;
        }
        
        public Tuple NormalAt(Tuple worldPoint, Intersection hit=null)
        {
            var localPoint = WorldToObject(worldPoint);
            var localNormal = NormalAtLocal(localPoint, hit);
            return NormalToWorld(localNormal);
        }
        
        public Tuple WorldToObject(Tuple point)
        {
            var p = point;
            if (Parent != null)
            {
                p = Parent.WorldToObject(point);
            }
#if OPTIM_WORLD_TO_OBJECT
            if (ReferenceEquals(Transform, Matrix.Identity))
            {
                return p;
            }
#endif            
            var transfoP =  Transform.Inverse() * p;
            return transfoP;
        }

        public Tuple NormalToWorld(Tuple normal)
        {
            var n = Transform.Inverse().Transpose() * normal;
            n = Helper.CreateVector(n.X, n.Y, n.Z);
            n = n.Normalize();
            if (Parent != null)
            {
                n = Parent.NormalToWorld(n);
            }

            return n;
        }
    }
}