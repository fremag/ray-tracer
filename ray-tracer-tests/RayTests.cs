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

        [Fact]
        public void TranslateTest()
        {
            var r = Helper.Ray(Helper.CreatePoint(1, 2, 3), Helper.CreateVector(0, 1, 0));
            var m = Helper.Translation(3, 4, 5);
            var r2 = r.Transform(m);

            Check.That(r2.Origin).IsEqualTo(Helper.CreatePoint(4, 6, 8));
            Check.That(r2.Direction).IsEqualTo(Helper.CreateVector(0, 1, 0));
        }
        
        [Fact]
        public void ScaleTest()
        {
            var r = Helper.Ray(Helper.CreatePoint(1, 2, 3), Helper.CreateVector(0, 1, 0));
            var m = Helper.Scaling(2, 3, 4);
            var r2 = r.Transform(m);

            Check.That(r2.Origin).IsEqualTo(Helper.CreatePoint(2, 6, 12));
            Check.That(r2.Direction).IsEqualTo(Helper.CreateVector(0, 3, 0));
        }
    }
}