using NFluent;
using ray_tracer.Triangulation;
using Xunit;

namespace ray_tracer.tests.Triangulation
{
    public class Triangle2DTest
    {
        private Triangle2D triangle = new Triangle2D(new Point2D(0, 0), new Point2D(1, 0), new Point2D(0, 1));

        [Theory]
        [InlineData(-1, 0, false)]
        [InlineData(0.25, 0.25, true)]
        public void IsInsideTest(double x, double y, bool isInside)
        {
            var p = new Point2D(x, y);
            var result = triangle.IsInside(p);
            Check.That(result).IsEqualTo(isInside);
        }
    }
}