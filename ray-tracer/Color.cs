using System;

namespace ray_tracer
{
    public class Color
    {
        public static readonly Color Black = new Color(0);
        public static readonly Color White = new Color(1);
        public static readonly Color _Red = new Color(1,0,0);
        public static readonly Color _Green = new Color(0,1,0);
        public static readonly Color _Blue = new Color(0,0,1);

        public double Red { get; }
        public double Green { get; }
        public double Blue { get; }

        public Color(double c) : this(c, c, c)
        {

        }
        public Color(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public static Color operator +(Color c1, Color c2) => new Color(c1.Red + c2.Red, c1.Green + c2.Green, c1.Blue + c2.Blue);
        public static Color operator -(Color c1, Color c2) => new Color(c1.Red - c2.Red, c1.Green - c2.Green, c1.Blue - c2.Blue);
        public static Color operator *(Color c1, double c) => new Color(c1.Red * c, c1.Green * c, c1.Blue * c);
        public static Color operator *(double c, Color c1) => new Color(c1.Red * c, c1.Green * c, c1.Blue * c);
        public static Color operator /(Color c1, double c) => new Color(c1.Red / c, c1.Green / c, c1.Blue / c);
        public static Color operator /(double c, Color c1) => new Color(c1.Red / c, c1.Green / c, c1.Blue / c);

        public static Color operator *(Color c1, Color c2) => new Color(c1.Red * c2.Red, c1.Green * c2.Green, c1.Blue * c2.Blue);

        public static int Normalize(double c)
        {
            var d = Math.Min(c, 1);
            d = Math.Max(d, 0);

            return (int)Math.Round(d * 255, 0);
        }

        protected bool Equals(Color other)
        {
            return Red.Equals(other.Red) && Green.Equals(other.Green) && Blue.Equals(other.Blue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Color) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Red.GetHashCode();
                hashCode = (hashCode * 397) ^ Green.GetHashCode();
                hashCode = (hashCode * 397) ^ Blue.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"R: {Red} G: {Green} B: {Blue}";
    }
}