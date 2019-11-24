using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ray_tracer;

namespace ray_tracer_demos
{
    public static class Program
    {
        static void Main()
        {
            Stopwatch sw = Stopwatch.StartNew();
            RenderManager renderMgr = new RenderManager();
            Console.WriteLine($"Start time: {DateTime.Now:HH:mm:ss}");
            var scene = new MengerSpongeScene();
            scene.InitWorld();
            renderMgr.Render(scene.World, 2, 3, -10, lookX: -1.5, lookY: 0);
            while (renderMgr.PixelsLeft > 0)
            {
                Print(renderMgr);
                Thread.Sleep(500);
            }

            Print(renderMgr);
            Console.WriteLine();

            var file = renderMgr.Save("prism.ppm");
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds:###,###,##0} ms");
            Helper.Display(file);
            File.Delete(file);
        }

        private static void Print(RenderManager renderMgr)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.Write(renderMgr.RenderStatistics);
        }

        internal static void OnRowRendered(int progress, int hSize)
        {
            var startTime = Process.GetCurrentProcess().StartTime;
            var now = DateTime.Now;
            var pct = (100.0 * progress) / hSize;
            var t = (now - startTime).TotalSeconds / pct;
            var endTime = startTime.AddSeconds(100 * t);

            Console.SetCursorPosition(0, Console.CursorLeft);
            Console.Write($"{startTime - now:hh\\:mm\\:ss}> {pct,5:n2} % => {endTime - startTime:hh\\:mm\\:ss}, endTime: {endTime:HH\\:mm\\:ss}");
        }
    }
}