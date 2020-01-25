using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer_demos.Basic;
using Color = System.Drawing.Color;

namespace ray_tracer_ui.Data
{
    public class RayTracingService
    {
        public event Action<DateTime> Clock;
        private AbstractScene Scene { get; set; }
        public Dictionary<string, Type> SceneTypes { get; }
        Bitmap bitmap;
        private bool[][] pixels;
        private readonly Timer timer;
        private RenderManager RenderManager { get; }

        public RayTracingService()
        {
            
            SceneTypes = Helper.GetScenes<IcosahedronScene>();
            RenderManager = new RenderManager();
            timer = new Timer {Interval = 1000, AutoReset = true};
            timer.Elapsed += OnClock;
        }

        private void OnClock(object sender, ElapsedEventArgs e)
        {
            Clock?.Invoke(DateTime.Now);
        }

        public void Stop()
        {
            RenderManager.Stop();
            timer.Stop();
        }

        public RenderStatistics GetStatistics() => RenderManager.RenderStatistics;

        public List<CameraParameters> CameraParameters(string sceneName)
        {
            var scene = CreateScene(sceneName);
            return scene == null ? new List<CameraParameters>() : scene.CameraParameters;
        }

        public List<string> GetScenes() => SceneTypes.Keys.ToList();

        public void Run(string sceneName, CameraParameters cameraParameters, RenderParameters renderParameters)
        {
            timer.Start();
            Scene = CreateScene(sceneName);
            if (Scene == null)
            {
                return;
            }

            Scene.InitWorld();

            bitmap = new Bitmap(cameraParameters.Width, cameraParameters.Height);
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    bitmap.SetPixel(j, i, Color.Black);
                }
            }

            pixels = Enumerable.Range(0, cameraParameters.Height).Select(i => new bool[cameraParameters.Width]).ToArray();
            RenderManager.Render(cameraParameters, renderParameters, Scene.World);
        }

        private AbstractScene CreateScene(string sceneName)
        {
            SceneTypes.TryGetValue(sceneName, out var typeScene);
            if (typeScene == null)
            {
                return null;
            }

            var scene = (AbstractScene) Activator.CreateInstance(typeScene);
            return scene;
        }

        public Task<string> GetImage()
        {
            using var memoryStream = new MemoryStream();

            if (RenderManager.Image != null)
            {
                lock (bitmap)
                {
                    CreateImage(memoryStream, RenderManager.Image);
                }
            }
            else
            {
                CreateDefaultImage(memoryStream);
            }

            var bytes = memoryStream.ToArray();
            var img = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
            return Task.FromResult(img);
        }

        private void CreateDefaultImage(MemoryStream memoryStream)
        {
            using Bitmap b = new Bitmap(50, 50);
            using Graphics g = Graphics.FromImage(b);

            g.Clear(Color.Black);
            b.Save(memoryStream, ImageFormat.Png);
        }

        private void CreateImage(Stream stream, Canvas canvas)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int i=-1;
            int j=-1;
            ray_tracer.Color c = default;
            try
            {
                for (i = 0; i < canvas.Width; i++)
                {
                    for (j = 0; j < canvas.Height; j++)
                    {
                        c = canvas.Pixels[i][j];
                        if (!canvas.Computed[i][j] || pixels[j][i])
                        {
                            continue;
                        }

                        pixels[j][i] = true;
                        var cRed = ray_tracer.Color.Normalize(c.Red);
                        var cGreen = ray_tracer.Color.Normalize(c.Green);
                        var cBlue = ray_tracer.Color.Normalize(c.Blue);
                        var color = Color.FromArgb(cRed, cGreen, cBlue);
                        bitmap.SetPixel(i, j, color);
                    }
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"i: {i}, j: {j}, Red: {c.Red}, Green: {c.Green}, Blue: {c.Blue}");
            }

            Console.WriteLine($"Draw: {sw.ElapsedMilliseconds} ms");
            sw.Reset();
            bitmap.Save(stream, ImageFormat.Png);

            Console.WriteLine($"Save: {sw.ElapsedMilliseconds} ms");
        }
    }
}