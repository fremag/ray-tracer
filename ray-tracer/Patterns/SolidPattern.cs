namespace ray_tracer.Patterns
{
    public class SolidPattern : AbstractPattern
    {
        public Color Color { get; }

        public SolidPattern(Color c)
        {
            Color = c;
        }

        public override Color GetColor(Tuple point) => Color;

        protected bool Equals(SolidPattern other)
        {
            return Equals(Color, other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SolidPattern) obj);
        }

        public override int GetHashCode()
        {
            return (Color != null ? Color.GetHashCode() : 0);
        }
    }
}