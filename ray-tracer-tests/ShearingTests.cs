using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class ShearingTests
    {
        [Theory]
        [InlineData(5, 3, 4, 1, 0, 0, 0, 0, 0)]
        [InlineData(6, 3, 4, 0, 1, 0, 0, 0, 0)]
        [InlineData(2, 5, 4, 0, 0, 1, 0, 0, 0)]
        [InlineData(2, 7, 4, 0, 0, 0, 1, 0, 0)]
        [InlineData(2, 3, 6, 0, 0, 0, 0, 1, 0)]
        [InlineData(2, 3, 7, 0, 0, 0, 0, 0, 1)]
        public void ShearingTest(double x, double y, double z, double xy, double xz, double yx, double yz, double zx, double zy)
        {
            var m = Helper.Shearing(xy, xz, yx, yz, zx, zy);
            var p = Helper.CreatePoint(2, 3, 4);
            Check.That(m * p ).IsEqualTo(Helper.CreatePoint(x, y, z));
        }
    }
}