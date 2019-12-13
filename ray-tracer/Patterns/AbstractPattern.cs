namespace ray_tracer.Patterns
{
    public abstract class AbstractPattern : IPattern
    {
        public Matrix Transform { get; set; } = Helper.CreateIdentity();

        public abstract Color GetColor(Tuple point);
        
        public Color GetColorAtShape(IShape shape, Tuple point)
        {
            var objectPoint = shape.WorldToObject(point);
            var patternPoint = Transform.Invert() * objectPoint;
            var color = GetColor(patternPoint);
            return color;
        }

        protected AbstractPattern()
        {
        }

        protected AbstractPattern(Matrix transform)
        {
            Transform = transform;
        }
    }
}