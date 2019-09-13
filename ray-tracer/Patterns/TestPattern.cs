namespace ray_tracer.Patterns
{
    public class TestPattern : AbstractPattern
    {
        public override Color GetColor(Tuple point)
        {
            return new Color(point.X, point.Y, point.Z);
        }
    }
}