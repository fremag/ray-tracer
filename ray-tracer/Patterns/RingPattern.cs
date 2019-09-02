using System;

namespace ray_tracer.Patterns
{
    public class RingPattern : BiColorPattern
    {
        public RingPattern(Matrix transform, Color colorA, Color colorB) : base(transform, colorA, colorB)
        {
        }

        public RingPattern(Color colorA, Color colorB) : base(colorA, colorB)
        {
        }

        public override Color GetColor(Tuple point)
        {
            var distance = Math.Sqrt(point.X * point.X + point.Z * point.Z);
            return Math.Abs(Math.Floor(distance % 2)) < double.Epsilon ? ColorA : ColorB;
        }
    }
}