namespace ray_tracer
{
    public class IntersectionData
    {
        public double T { get; }
        public Sphere Object { get; }
        public Tuple Point { get; }
        public Tuple EyeVector { get; }
        public Tuple Normal { get; }
        public bool Inside { get; }

        public IntersectionData(Intersection intersection, Ray ray)
        {
            T = intersection.T;
            Object = intersection.Object;
            Point = ray.Position(T);
            EyeVector = -ray.Direction;
            Normal = Object.NormalAt(Point);

            if (Normal.DotProduct(EyeVector) < 0)
            {
                Inside = true;
                Normal = - Object.NormalAt(Point);
            } 
            else
            {
                Inside = false;
            }            
        }
    }
}