namespace ray_tracer
{
    public class Color
    {
        public double Red { get; }
        public double Green { get; }
        public double Blue { get; }

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
    }
}