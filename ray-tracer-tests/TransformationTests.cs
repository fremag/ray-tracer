using System;
using System.IO;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class TransformationTests
    {
        [Fact]
        public void TransformationSequenceTest()
        {
            var p = Helper.CreatePoint(1, 0, 1);
            var r = Helper.RotationX(Math.PI / 2);
            var s = Helper.Scaling(5, 5, 5);
            var t = Helper.Translation(10, 5, 7);
            var p2 = r * p;
            Check.That(p2).IsEqualTo(Helper.CreatePoint(1, -1, 0));

            var p3 = s * p2;
            Check.That(p3).IsEqualTo(Helper.CreatePoint(5, -5, 0));

            var p4 = t * p3;
            Check.That(p4).IsEqualTo(Helper.CreatePoint(15, 0, 7));            
        }
    
        [Fact]
        public void ChainedTransformationTest()
        {
            var p = Helper.CreatePoint(1, 0, 1);
            var r = Helper.RotationX(Math.PI / 2);
            var s = Helper.Scaling(5, 5, 5);
            var t = Helper.Translation(10, 5, 7);
            var m = t * s * r; 
            var p2 = m * p;
            Check.That(p2).IsEqualTo(Helper.CreatePoint(15, 0, 7));            
        }

        [Fact]
        public void ClockImageTest()
        {
            const int N = 40;
            var size = 200;
            var canvas = new Canvas(size, size);

            var p = Helper.CreatePoint(0, 0, 0);
            var translate = Helper.Translation(0, 1, 0);
            var scaling = Helper.Scaling(size/3, size/3, 0);
            p = translate * p;
            p = scaling * p;
            
            var rotation = Helper.RotationZ(2 * Math.PI / N);
            Color color = new Color(1, 1, 1);
            
            for (int i = 0; i < N; i++)
            {
                p = rotation * p;
                var pX = (int)(p.X+size/2);
                var pY = (int)(p.Y+size/2);
                canvas.SetPixel(pX, pY, color);
            }

            var tmpFile = Path.GetTempFileName();
            var ppmFile = Path.ChangeExtension(tmpFile, "ppm");
            canvas.SavePPM(ppmFile);
            
            File.Delete(tmpFile);
            File.Delete(ppmFile);
        }
    }
}