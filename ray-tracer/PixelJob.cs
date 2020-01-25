using System.Collections.Generic;
using ray_tracer.Cameras;

namespace ray_tracer
{
    public class PixelJob
    {
        public List<int> X { get; } = new List<int>();
        public List<int> Y { get; } = new List<int>();
        public Canvas Canvas { get; }
        public World World { get; }
        public int MaxRecursion { get; }
        public RenderStatistics RenderStatistics { get; }
        public ICamera Camera { get; }

        public PixelJob(Canvas canvas, World world, int maxRecursion, RenderStatistics renderStatistics, ICamera camera)
        {
            Canvas = canvas;
            World = world;
            MaxRecursion = maxRecursion;
            RenderStatistics = renderStatistics;
            Camera = camera;
        }

        public void DoWork()
        {
            for (int i = 0; i < X.Count; i++)
            {
                int x = X[i];
                int y = Y[i];
                var ray = Camera.RayForPixel(x, y);
                var color = World.ColorAt(ray, MaxRecursion);
                Canvas.SetPixel(x, y, color);
                RenderStatistics.IncNbPixels();
            }
        }
    }
}