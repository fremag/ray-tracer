using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class PlaneTests
    {
        [Fact]
        public void TheNormalOfAPlaneIsConstantEverywhereTest()
        {
            var p = new Plane();
            Check.That(p.NormalAt(Helper.CreatePoint(0, 0, 0))).IsEqualTo(Helper.CreateVector(0, 1, 0));
            Check.That(p.NormalAt(Helper.CreatePoint(10, 0, -10))).IsEqualTo(Helper.CreateVector(0, 1, 0));
            Check.That(p.NormalAt(Helper.CreatePoint(-5, 0, 150))).IsEqualTo(Helper.CreateVector(0, 1, 0));
        }

        [Fact]
        public void IntersectWithARayParallelToThePlaneTest()
        {
            var p = new Plane();
            var r = Helper.Ray(Helper.CreatePoint(0, 10, 0), Helper.CreateVector(0, 0, 1));
            var xs = p.Intersect(ref r.Origin, ref r.Direction);
            Check.That(xs).IsEmpty();
        }

        [Fact]
        public void IntersectWithAXoplanarRayTest()
        {
            var p = new Plane();
            var r = Helper.Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var xs = p.Intersect(ref r.Origin, ref r.Direction);
            Check.That(xs).IsEmpty();
        }

        [Fact]
        public void ARayIntersectingAPlaneFromAboveTest()
        {
            var p = new Plane();
            var r = Helper.Ray(Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, -1, 0));
            var xs = p.Intersect(ref r.Origin, ref r.Direction);
            Check.That(xs.Count).IsEqualTo(1);
            Check.That(xs[0].Object).IsEqualTo(p);
            Check.That(xs[0].T).IsEqualTo(1);
        }

        [Fact]
        public void ARayIntersectingAPlaneFromBelowTest()
        {
            var p = new Plane();
            var r = Helper.Ray(Helper.CreatePoint(0, -1, 0), Helper.CreateVector(0, 1, 0));
            var xs = p.Intersect(ref r.Origin, ref r.Direction);
            Check.That(xs.Count).IsEqualTo(1);
            Check.That(xs[0].Object).IsEqualTo(p);
            Check.That(xs[0].T).IsEqualTo(1);
        }
    }
}