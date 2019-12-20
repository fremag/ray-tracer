using System.IO;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class CanvasTests
    {
        [Fact]
        public void CanvasTest()
        {
            var canvas = new Canvas(20, 10);
            Check.That(canvas.Width).IsEqualTo(20);
            Check.That(canvas.Height).IsEqualTo(10);

            for (int i = 0; i < canvas.Height; i++)
            {
                foreach (var c in canvas[i])
                {
                    Check.That(c).IsDefaultValue();
                }
            }
        }

        [Fact]
        public void ReadWritePixel()
        {
            var canvas = new Canvas(20, 10);
            canvas[0][0] = new Color(5);
            Check.That(canvas[0][0].Red).IsEqualTo(5);
            Check.That(canvas[0][0].Green).IsEqualTo(5);
            Check.That(canvas[0][0].Blue).IsEqualTo(5);
        }

        [Fact]
        public void GetSetPixelTest()
        {
            var canvas = new Canvas(20, 10);
            canvas.SetPixel(3, 5, Color.White);
            Check.That(canvas.GetPixel(3, 5)).IsEqualTo(Color.White);
        }

        [Fact]
        public void ToPPM_Test()
        {
            Canvas canvas = new Canvas(5, 3);
            canvas.SetPixel(0, 0, new Color(1.5, 0, 0));
            canvas.SetPixel(2, 1, new Color(0, 0.5, 0));
            canvas.SetPixel(4, 2, new Color(-0.5, 0, 1));

            var txt = canvas.ToPPM();
            Check.That(txt).ContainsExactly("P3", "5 3", "255", "255 0 0 0 0 0 0 0 0 0 0 0 0 0 0", "0 0 0 0 0 0 0 128 0 0 0 0 0 0 0", "0 0 0 0 0 0 0 0 0 0 0 0 0 0 255");
        }

        [Fact]
        public void ToPPM_SplitLongLines_Test()
        {
            Canvas canvas = new Canvas(10, 2);
            var color = new Color(1, 0.8, 0.6);
            for(int i=0; i < 10; i++)
            {
                for(int j=0; j < 2; j++)
                {
                    canvas.SetPixel(i, j, color);
                }
            }

            var txt = canvas.ToPPM();
            Check.That(txt).ContainsExactly("P3", "10 2", "255", "255 204 153 255 204 153 255 204 153 255 204 153 255 204 153 255 204", "153 255 204 153 255 204 153 255 204 153 255 204 153", "255 204 153 255 204 153 255 204 153 255 204 153 255 204 153 255 204", "153 255 204 153 255 204 153 255 204 153 255 204 153");
        }

        internal class Projectile
        {
            public Projectile(Tuple position, Tuple velocity)
            {
                Position = position;
                Velocity = velocity;
            }

            internal Tuple Position { get; private set; }            
            internal Tuple Velocity { get; private set; }

            internal void Tick(Tuple wind, Tuple gravity)
            {
                Position += Velocity;
                Velocity = Velocity + gravity + wind;
            }
        }
        
        [Fact]
        public void VectorCanvasTest()
        {
            var start = Helper.CreatePoint(0, 1, 0);
            var velocity = Helper.CreateVector(1, 1.8, 0).Normalize() * 11.25;
            var projectile = new Projectile(start, velocity);
            
            var gravity = Helper.CreateVector(0, -0.1, 0);
            var wind = Helper.CreateVector(-0.01, 0, 0);
            var canvas = new Canvas(900, 550);

            while (projectile.Position.X < 900 && projectile.Position.Y < 550 && projectile.Position.Y > 0)
            {
                var positionX = (int)projectile.Position.X;
                var positionY = (int)projectile.Position.Y;
                canvas.SetPixel(positionX, 550-positionY, Color.White);
                projectile.Tick(wind, gravity);
            }

            var tmpFile = Path.GetTempFileName();
            var ppmFile = Path.ChangeExtension(tmpFile, "ppm");
            canvas.SavePPM(ppmFile);
            
            File.Delete(tmpFile);
            File.Delete(ppmFile);
        }
    }
}