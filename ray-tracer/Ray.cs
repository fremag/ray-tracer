namespace ray_tracer
{
    public class Ray
    {
        public Tuple Origin { get; }
        public Tuple Direction { get; }

        public Ray(Tuple origin, Tuple direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Tuple Position(double t) => Origin + t * Direction;
    }
}