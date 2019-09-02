using System;

namespace ray_tracer.Patterns
{
    public class GradientPattern : BiColorPattern
    {
        private Color Distance { get; }

        public GradientPattern(Color colorA, Color colorB) : base(colorA, colorB)
        {
            Distance = colorB - colorA;
        }

        public GradientPattern(Matrix transform, Color colorA, Color colorB) : base(transform, colorA, colorB)
        {
        }

        public override Color GetColor(Tuple point)
        {
            var fraction = point.X - Math.Floor(point.X);
            return ColorA + Distance * fraction;
        }
    }
}