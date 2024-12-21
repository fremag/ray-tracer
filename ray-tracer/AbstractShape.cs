using System.Threading;

namespace ray_tracer
{
    public abstract class AbstractShape : IShape
    {
        private static int id = -1;
        public int Id { get; } = Interlocked.Increment(ref id); 
        public Matrix Transform { get; set; } = Matrix.Identity;
        public IShape Parent { get; set; }
        public virtual Material Material { get; set; } = new Material();

        public abstract void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections);
        public abstract Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null);
        public abstract Bounds Box { get; }
        private Bounds transformedBox;
        public Bounds TransformedBox {
            get {
                if (transformedBox == null)
                {
                    transformedBox = Box.Transform(Transform);
                }

                return transformedBox;
            }
        }
        
        public virtual bool Contains(IShape shape)
        {
            return ReferenceEquals(shape, this);
        }

        public bool HasShadow { get; set; } = true;

        public void Intersect(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            if (ReferenceEquals(Transform, Matrix.Identity))
            {
                IntersectLocal(ref origin, ref direction, intersections);
                return;
            }
            var invMatrix = Transform.Invert();
            var transformedOrigin = invMatrix.FastTransform(ref origin);
            var transformedDirection = invMatrix.FastTransform(ref direction);
            IntersectLocal(ref transformedOrigin, ref transformedDirection, intersections);
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

        public virtual IShape Divide(int threshold)
        {
            return this;
        }
    }
}