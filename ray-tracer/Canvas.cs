namespace ray_tracer
{
    public class Canvas
    {
        public int Width { get; }
        public int Height { get; }
        public Color[][] Pixels { get; }

        public Canvas(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[Width][];
            for (int i = 0; i < Width; i++)
            {
                Pixels[i] = new Color[Height];
                for (int j = 0; j < Height; j++)
                {
                    Pixels[i][j] = null;
                }
            }
        }

        public Color[] this[int y]
        {
            get => Pixels[y];
        }

        public void SetPixel(int x, int y, Color c)
        {
            Pixels[x][y] = c;
        }
        public Color GetPixel(int x, int y) => Pixels[x][y];
    }
}