using System;

namespace ray_tracer.Patterns
{
    public class StripePattern : AbstractPattern
    {
        public Color A { get;  }
        public Color B { get;  }

        public StripePattern(Color a, Color b)
        {
            A = a;
            B = b;
        }

        public StripePattern() : this(Color.White, Color.Black)
        {}

        public override Color GetColor(Tuple point)
        {
            if (Math.Abs(Math.Floor(point.X) % 2) < double.Epsilon)
            {
                return A;
            }

            return B;
        }
    }
}