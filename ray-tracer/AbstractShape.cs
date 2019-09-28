namespace ray_tracer
{
    public abstract class AbstractShape : IShape
    {
        public Matrix Transform { get; set; } = Helper.CreateIdentity();
        public IShape Parent { get; set; }
        public Material Material { get; set; } = new Material();

        public abstract Intersections IntersectLocal(Ray ray);
        public abstract Tuple NormalAtLocal(Tuple worldPoint);

        public Intersections Intersect(Ray ray)
        {
            var transformedRay = ray.Transform(Transform.Inverse());
            return IntersectLocal(transformedRay);
        }
        
        public Tuple NormalAt(Tuple worldPoint)
        {
            var localPoint = WorldToObject(worldPoint);
            var localNormal = NormalAtLocal(localPoint);
            return NormalToWorld(localNormal);
        }
        
        public Tuple WorldToObject(Tuple point)
        {
            var p = point;
            if (Parent != null)
            {
                p = Parent.WorldToObject(point);
            }

            return Transform.Inverse() * p;
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