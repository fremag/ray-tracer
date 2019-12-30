using System;
using System.Diagnostics;

namespace ray_tracer.Patterns
{
    public class PerlinPattern : AbstractPattern
    {
        public ColorMap ColorMap { get; }

        public PerlinPattern()
        {
            ColorMap = new ColorMap((1, Color.White));
        }

        public PerlinPattern(ColorMap colorMap)
        {
            ColorMap = colorMap;
        }

        Perlin perlin = new Perlin();
        public override Color GetColor(Tuple point)
        {
            var p = perlin.perlin(Math.Abs(point.X), Math.Abs(point.Y), Math.Abs(point.Z));
            var c =  ColorMap.GetColor(p);
            return c;
        }
    }
}