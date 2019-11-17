using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ray_tracer;
using ray_tracer_demos;
using Color = System.Drawing.Color;

namespace ray_tracer_ui.Data
{
    public class RayTracingService
    {
        private AbstractScene Scene { get; set; }

        public void Run(SceneParameters sceneParameters)
        {
            Scene = new ConeScene();
            Scene.InitWorld();
            Task.Run( () => Scene.Render(sceneParameters));
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
            using var bitmap = new Bitmap(canvas.Width, canvas.Height);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Black);

            for (int i = 0; i < canvas.Width; i++)
            {
                for (int j = 0; j < canvas.Height; j++)
                {
                    var c = canvas.Pixels[i][j];
                    var cRed = ray_tracer.Color.Normalize(c.Red);
                    var cGreen = ray_tracer.Color.Normalize(c.Green);
                    var cBlue = ray_tracer.Color.Normalize(c.Blue);
                    try
                    {
                        var color = Color.FromArgb(cRed, cGreen, cBlue);
                        var pen = new Pen(color, 1);
                        graphics.DrawRectangle(pen, i, j, 1, 1);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            bitmap.Save(stream, ImageFormat.Png);
        }
    }
}