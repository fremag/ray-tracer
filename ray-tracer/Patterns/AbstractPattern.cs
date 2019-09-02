namespace ray_tracer.Patterns
{
    public abstract class AbstractPattern : IPattern
    {
        public Matrix Transform { get; set; } = Helper.CreateIdentity();

        public abstract Color GetColor(Tuple point);

        protected AbstractPattern()
        {
        }

        protected AbstractPattern(Matrix transform)
        {
            Transform = transform;
        }
    }
}