using System;

namespace ray_tracer.Patterns
{
    public class CheckerPattern : BiColorPattern
    {
        public CheckerPattern(Matrix transform, Color colorA, Color colorB) : base(transform, colorA, colorB)
        {
        }

        public CheckerPattern(Color colorA, Color colorB) : base(colorA, colorB)
        {
        }

        public override Color GetColor(Tuple point)
        {
            var d = Math.Floor(point.X) + Math.Floor(point.Y) + Math.Floor(point.Z);
            if (Math.Abs(d % 2) < double.Epsilon)
            {
                return ColorA;
            }

            return ColorB;
        }
    }
}