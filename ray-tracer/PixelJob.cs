namespace ray_tracer
{
    public class PixelJob
    {
        public int X { get; }
        public int Y { get; }
        public Canvas Canvas { get; }
        public World World { get; }
        public int MaxRecursion { get; }
        public Ray Ray { get; }
        public RenderStatistics RenderStatistics { get; }

        public PixelJob(in int x, in int y, Canvas canvas, World world, int maxRecursion, Ray ray, RenderStatistics renderStatistics)
        {
            X = x;
            Y = y;
            Canvas = canvas;
            World = world;
            MaxRecursion = maxRecursion;
            Ray = ray;
            RenderStatistics = renderStatistics;
        }

        public void DoWork()
        {
            var color = World.ColorAt(Ray, MaxRecursion);
            Canvas.SetPixel(X, Y, color);
            RenderStatistics.IncNbPixels();
        }
    }
}