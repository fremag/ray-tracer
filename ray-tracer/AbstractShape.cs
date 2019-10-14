#define OPTIM_WORLD_TO_OBJECT
#define OPTIM_INTERSECT
namespace ray_tracer
{
    public abstract class AbstractShape : IShape
    {
        public Matrix Transform { get; set; } = Matrix.Identity;
        public IShape Parent { get; set; }
        public Material Material { get; set; } = new Material();

        public abstract Intersections IntersectLocal(Ray ray);
        public abstract Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null);
        public abstract Bounds Box { get; }

        public Intersections Intersect(Ray ray)
        {
            Ray transformedRay = ray;
#if OPTIM_INTERSECT            
            if (! ReferenceEquals(Transform, Matrix.Identity))
            {
#endif                
                transformedRay = ray.Transform(Transform.Inverse());
#if OPTIM_INTERSECT                
            }
#endif            
            return IntersectLocal(transformedRay);
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