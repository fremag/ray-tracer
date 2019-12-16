using NFluent;
using ray_tracer.Triangulation;
using Xunit;

namespace ray_tracer.tests.Triangulation
{
    public class Point2DTests
    {
        [Theory]
        [InlineData(0,1,0,0,1,0, true)]
        [InlineData(0,-1,0,0,1,0, false)]
        [InlineData(0,1,0,0,1,1, true)]
        [InlineData(0,-1,0,0,1,1, false)]
        [InlineData(-10,1,0,0,1,1, true)]
        [InlineData(-10,-11,0,0,1,1, false)]
        public void IsLeftTest(double x, double y, double x0, double y0, double x1, double y1, bool isLeft)
        {
            var p = new Point2D(x, y);
            var p0 = new Point2D(x0, y0);
            var p1 = new Point2D(x1, y1);

            var result = p.IsLeft(p0, p1);
            Check.That(result).IsEqualTo(isLeft);
        }
    }
}