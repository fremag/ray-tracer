using System;

namespace ray_tracer
{
    public class Canvas
    {
        public int Width { get; }
        public int Height { get; }
        public Color[][] Pixels { get; }
        public bool[][] Computed { get; }
        
        public Canvas(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[Width][];
            Computed = new bool[Width][];
            for (int i = 0; i < Width; i++)
            {
                Pixels[i] = new Color[Height];
                Computed[i] = new bool[Height];
                Array.Fill(Computed[i], false);
            }
        }

        public Color[] this[int y]
        {
            get => Pixels[y];
        }

        public void SetPixel(int x, int y, Color c)
        {
            Pixels[x][y] = c;
            Computed[x][y] = true;
        }
        
        public Color GetPixel(int x, int y) => Pixels[x][y];
    }
}