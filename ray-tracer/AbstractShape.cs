namespace ray_tracer
{
    public abstract class AbstractShape : IShape
    {
        public Matrix Transform { get; set; } = Matrix.Identity;
        public IShape Parent { get; set; }
        public Material Material { get; set; } = new Material();

        public abstract Intersections IntersectLocal(ref Tuple origin, ref Tuple direction);
        public abstract Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null);
        public abstract Bounds Box { get; }
        
        public virtual bool Contains(IShape shape)
        {
            return ReferenceEquals(shape, this);
        }

        public Intersections Intersect(ref Tuple origin, ref Tuple direction)
        {
            if (ReferenceEquals(Transform, Matrix.Identity))
            {
                return IntersectLocal(ref origin, ref direction);
            }
            var invMatrix = Transform.Invert();
            var transformedOrigin = invMatrix.FastTransform(ref origin);
            var transformedDirection = invMatrix.FastTransform(ref direction);
            return IntersectLocal(ref transformedOrigin, ref transformedDirection);
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
            if (ReferenceEquals(Transform, Matrix.Identity))
            {
                return p;
            }
            var transfoP =  Transform.Invert().FastTransform(ref p);
            return transfoP;
        }

        public Tuple NormalToWorld(Tuple normal)
        {
            var n = Transform.Invert().Transpose() * normal;
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