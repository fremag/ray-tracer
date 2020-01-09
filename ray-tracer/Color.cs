using System;

namespace ray_tracer
{
    public struct Color
    {
        public static readonly Color Black = new Color(0);
        public static readonly Color White = new Color(1);
        public static readonly Color _Red = new Color(1,0,0);
        public static readonly Color _Green = new Color(0,1,0);
        public static readonly Color _Blue = new Color(0,0,1);

        public double Red { get; }
        public double Green { get; }
        public double Blue { get; }
        public static readonly Color Yellow = _Green + _Red;
        public static readonly Color Magenta = _Blue + _Red;
        public static readonly Color Cyan = _Blue + _Green;

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

        public bool Equals(Color other)
        {
            return Helper.AreEquals(Red, other.Red) && Helper.AreEquals(Green, other.Green) && Helper.AreEquals(Blue, other.Blue);
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

        public static Color Rgb(int r, int g, int b) => new Color(r / 255.0, g / 255.0, b / 255.0);
    }
}