using System;
using System.Collections.Generic;

namespace ray_tracer.Patterns
{
    public class ColorMapEntry
    {
        public double Key { get; }
        public Color Color { get; }

        public ColorMapEntry(double key, double r, double g, double b) 
            : this(key, new Color(r, g, b))
        {
            
        }
        
        public ColorMapEntry(double key, Color color)
        {
            Key = key;
            Color = color;
        }
    }
    
    public class ColorMap
    {
        private List<ColorMapEntry> Entries { get; } = new List<ColorMapEntry>();

        public void Add(double key, Color color)
        {
            Entries.Add(new ColorMapEntry(key, color));
        }

        public Color GetColor(double key)
        {
            if (key < Entries[0].Key)
            {
                var c = Interpolate(0, Entries[0].Key, key, Color.Black, Entries[0].Color);
                return c;
            }

            if (key > Entries[^1].Key)
            {
                var c = Interpolate(Entries[^1].Key, 1, key, Entries[^1].Color, Color.Black);
                return c;
            }

            for (int i = 0; i < Entries.Count - 1; i++)
            {
                var k1 = Entries[i].Key;
                var k2 = Entries[i+1].Key;

                if (key >= k1 && key <= k2)
                {
                    var c = Interpolate(k1, k2, key, Entries[i].Color, Entries[i+1].Color);
                    return c;
                }
            }
            return Color.Black;
        }

        static public double LinearInterp(double x, double x0, double x1, double y0, double y1)
        {
            var deltaX = (x1 - x0);
            if (Math.Abs(deltaX) < 1e-4)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / deltaX;
        }        
        
        private Color Interpolate(double key1, double key2, double key, Color c1, Color c2)
        {
            var delta = (key2 - key1);
            var r = LinearInterp(key, key1, key2, c1.Red, c2.Red);
            var g = LinearInterp(key, key1, key2, c1.Green, c2.Green);
            var b = LinearInterp(key, key1, key2, c1.Blue, c2.Blue);

            return new Color(r, g, b);
        }
    }
}