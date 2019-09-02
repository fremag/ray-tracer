using System;

namespace ray_tracer.Patterns
{
    public class StripePattern : BiColorPattern
    {
        public StripePattern(Matrix transform, Color colorA, Color colorB) : base(transform, colorA, colorB)
        {
        }

        public StripePattern(Color colorA, Color colorB) : base(colorA, colorB)
        {
        }

        public override Color GetColor(Tuple point)
        {
            if (Math.Abs(Math.Floor(point.X) % 2) < double.Epsilon)
            {
                return ColorA;
            }

            return ColorB;
        }
    }
}