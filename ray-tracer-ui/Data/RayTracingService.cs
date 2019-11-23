using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ray_tracer;
using ray_tracer_demos;
using Color = System.Drawing.Color;

namespace ray_tracer_ui.Data
{
    public class RayTracingService
    {
        private AbstractScene Scene { get; set; }
        public Dictionary<string, Type> SceneTypes { get; set; }
        Bitmap bitmap ;
        private bool[][] pixels;
        
        public RayTracingService()
        {
            var types = Assembly.GetAssembly(typeof(AbstractScene))
                .GetTypes().ToArray();
            SceneTypes = types
                .Where(type => typeof(AbstractScene).IsAssignableFrom(type))
                .Where(type => !type.IsAbstract)
                .ToDictionary(type => type.Name);

        }

        public List<string> GetScenes() => SceneTypes.Keys.ToList();
        
        public void Run(SceneParameters sceneParameters, RenderParameters renderParameters)
        {
            SceneTypes.TryGetValue(sceneParameters.Scene, out var typeScene);
            if (typeScene == null)
            {
                return;
            }
            Scene = (AbstractScene)Activator.CreateInstance(typeScene);
            Scene.InitWorld();
            bitmap = new Bitmap(sceneParameters.Width, sceneParameters.Height);
            for(int i=0; i <bitmap.Height; i++)
            {
                for(int j=0; j <bitmap.Width; j++)
                {
                    bitmap.SetPixel(j, i, Color.Black);
                }
            }

            pixels = Enumerable.Range(0, sceneParameters.Height).Select(i => new bool[sceneParameters.Width]).ToArray();
            
            Task.Run( () => Scene.Render(sceneParameters, renderParameters));
        }

        public Task<string> GetImage()
        {
            using var memoryStream = new MemoryStream();

            if (Scene?.Image != null)
            {
                CreateImage(memoryStream, Scene.Image);
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

            for (int i = 0; i < canvas.Width; i++)
            {
                for (int j = 0; j < canvas.Height; j++)
                {
                    var c = canvas.Pixels[i][j];
                    if (c != null && ! pixels[j][i])
                    {
                        pixels[j][i] = true;
                        var cRed = ray_tracer.Color.Normalize(c.Red);
                        var cGreen = ray_tracer.Color.Normalize(c.Green);
                        var cBlue = ray_tracer.Color.Normalize(c.Blue);
                        var color = Color.FromArgb(cRed, cGreen, cBlue);
                        bitmap.SetPixel(i, j, color);
                    }
                }
            }
            Console.WriteLine($"Draw: {sw.ElapsedMilliseconds} ms");
            sw.Reset();
            bitmap.Save(stream, ImageFormat.Png);
            Console.WriteLine($"Save: {sw.ElapsedMilliseconds} ms");
        }
    }
}