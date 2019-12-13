namespace ray_tracer
{
    public struct Ray
    {
        public Tuple Origin;
        public Tuple Direction;

        public Ray(Tuple origin, Tuple direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Tuple Position(double t) => Origin + t * Direction;

        public Ray Transform(Matrix m)
        {
            return new Ray(Origin * m, Direction * m);
        }
    }
}