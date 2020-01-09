using System;

namespace ray_tracer.Patterns
{
    public class PerlinPattern : AbstractPattern
    {
        private readonly Perlin perlin = new Perlin();
        private readonly int octaves = 1; 
        private readonly double persistence = 1;
        
        public ColorMap ColorMap { get; }

        public PerlinPattern(int octaves = 1, double persistence = 1)
        {
            ColorMap = new ColorMap((1, Color.White));
            this.octaves = octaves;
            this.persistence = persistence;
        }

        public PerlinPattern(ColorMap colorMap, int octaves = 1, double persistence = 1)
        {
            ColorMap = colorMap;
            this.octaves = octaves;
            this.persistence = persistence;
        }

        public override Color GetColor(Tuple point)
        {
            var p = perlin.OctavePerlin(Math.Abs(point.X), Math.Abs(point.Y), Math.Abs(point.Z), octaves, persistence);
            var c =  ColorMap.GetColor(p);
            return c;
        }
    }
}