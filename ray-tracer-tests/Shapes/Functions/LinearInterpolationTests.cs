using System.Linq;
using NFluent;
using ray_tracer.Shapes.Functions;
using Xunit;

namespace ray_tracer.tests.Shapes.Functions
{
    public class LinearInterpolationTests
    {
        private readonly LinearInterpolation interpolation;

        public LinearInterpolationTests()
        {
            InterpolationData[] points = Enumerable.Range(1, 10).Select(i => new InterpolationData(i, Helper.CreateVector(i, 2 * i, 3 * i))).ToArray();
            interpolation = new LinearInterpolation(points);
        }

        [Fact]
        public void BasicTest()
        {
            interpolation.GetPoint(-5, out var x, out var y, out var z);
            Check.That(x).IsEqualTo(1);
            Check.That(y).IsEqualTo(2);
            Check.That(z).IsEqualTo(3);
            interpolation.GetPoint(-0, out x, out y, out z);
            Check.That(x).IsEqualTo(1);
            Check.That(y).IsEqualTo(2);
            Check.That(z).IsEqualTo(3);
            interpolation.GetPoint(0.5, out x, out y, out z);
            Check.That(x).IsEqualTo(1);
            Check.That(y).IsEqualTo(2);
            Check.That(z).IsEqualTo(3);
            interpolation.GetPoint(1, out x, out y, out z);
            Check.That(x).IsEqualTo(1);
            Check.That(y).IsEqualTo(2);
            Check.That(z).IsEqualTo(3);
            interpolation.GetPoint(1.5, out x, out y, out z);
            Check.That(x).IsEqualTo(1.5);
            Check.That(y).IsEqualTo(3);
            Check.That(z).IsEqualTo(4.5);
            interpolation.GetPoint(15, out x, out y, out z);
            Check.That(x).IsEqualTo(10);
            Check.That(y).IsEqualTo(20);
            Check.That(z).IsEqualTo(30);
        }
    }
}