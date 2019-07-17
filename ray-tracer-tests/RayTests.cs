using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class RayTests
    {
        [Fact]
        public void RayTest()
        {
            var origin = Helper.CreatePoint(1, 2, 3);
            var dir = Helper.CreateVector(4, 5, 6);
            var ray = new Ray(origin, dir);

            Check.That(ray.Origin).IsEqualTo(origin);
            Check.That(ray.Direction).IsEqualTo(dir);
        }

        [Fact]
        public void PositionTest()
        {
            var origin = Helper.CreatePoint(2,3,4);
            var dir = Helper.CreateVector(1,0,0);
            var ray = new Ray(origin, dir);

            Check.That(ray.Position(0)).IsEqualTo(Helper.CreatePoint(2, 3, 4));
            Check.That(ray.Position(1)).IsEqualTo(Helper.CreatePoint(3, 3, 4));
            Check.That(ray.Position(-1)).IsEqualTo(Helper.CreatePoint(1, 3, 4));
            Check.That(ray.Position(2.5)).IsEqualTo(Helper.CreatePoint(4.5, 3, 4));
        }
    }
}