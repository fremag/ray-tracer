namespace ray_tracer
{
    public abstract class AbstractShape : IShape
    {
        public Matrix Transform { get; set; } = Helper.CreateIdentity();
        public Material Material { get; set; } = new Material();

        public Intersections Intersect(Ray ray)
        {
            var transformedRay = ray.Transform(Transform.Inverse());
            return IntersectLocal(transformedRay);
        }
        
        public abstract Intersections IntersectLocal(Ray ray);

        public Tuple NormalAt(Tuple worldPoint)
        {
            var inverseTransform = Transform.Inverse();
            var objectPoint = inverseTransform * worldPoint;
            var objectNormal = NormalAtLocal(objectPoint);
            var worldNormal = inverseTransform.Transpose() * objectNormal;
            worldNormal = Helper.CreateVector(worldNormal.X, worldNormal.Y, worldNormal.Z);
            return worldNormal.Normalize();
        }
        
        public abstract Tuple NormalAtLocal(Tuple worldPoint);
    }
}