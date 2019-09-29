using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class TriangleTests
    {
        [Fact]
        public void ConstructingATriangleTest()
        {
            var p1 = Helper.CreatePoint(0, 1, 0);
            var p2 = Helper.CreatePoint(-1, 0, 0);
            var p3 = Helper.CreatePoint(1, 0, 0);
            var t = new Triangle(p1, p2, p3);
            Check.That(t.P1).IsEqualTo(p1);
            Check.That(t.P2).IsEqualTo( p2);
            Check.That(t.P3).IsEqualTo(p3);
            Check.That(t.E1 ).IsEqualTo(Helper.CreateVector(-1, -1, 0));
            Check.That(t.E2).IsEqualTo(Helper.CreateVector(1, -1, 0));
            Check.That(t.N).IsEqualTo(Helper.CreateVector(0, 0, -1));
        }

        [Fact]
        public void FindingTheNormalOnATriangleTest()
        {
            var t = new Triangle(Helper.CreatePoint(0, 1, 0), Helper.CreatePoint(-1, 0, 0), Helper.CreatePoint(1, 0, 0));
            var n1 = t.NormalAtLocal(Helper.CreatePoint(0, 0.5, 0));
            var n2 = t.NormalAtLocal(Helper.CreatePoint(-0.5, 0.75, 0));
            var n3 = t.NormalAtLocal(Helper.CreatePoint(0.5, 0.25, 0));
            Check.That(n1).IsEqualTo(t.N);
            Check.That(n2).IsEqualTo(t.N);
            Check.That(n3).IsEqualTo(t.N);
        }

        [Fact]
        public void IntersectingARayParallelToTheTriangleTest()
        {
            var t = new Triangle(Helper.CreatePoint(0, 1, 0), Helper.CreatePoint(-1, 0, 0), Helper.CreatePoint(1, 0, 0));
            var r = Helper.Ray(Helper.CreatePoint(0, -1, -2), Helper.CreateVector(0, 1, 0));
            var xs = t.IntersectLocal(r);
            Check.That(xs).IsEmpty();
        }
        
        [Fact]
        public void ARayMissesTheP1P3EdgeTest()
        {
            var t = new Triangle(Helper.CreatePoint(0, 1, 0), Helper.CreatePoint(-1, 0, 0), Helper.CreatePoint(1, 0, 0));
            var r = Helper.Ray(Helper.CreatePoint(1, 1, -2), Helper.CreateVector(0, 0, 1));
            var xs = t.IntersectLocal(r);
            Check.That(xs).IsEmpty();
        }
        
        [Fact]
        public void ARayMissesTheP1P2EdgeTest()
        {
            var t = new Triangle(Helper.CreatePoint(0, 1, 0), Helper.CreatePoint(-1, 0, 0), Helper.CreatePoint(1, 0, 0));
            var r = Helper.Ray(Helper.CreatePoint(-1, 1, -2), Helper.CreateVector(0, 0, 1));
            var xs = t.IntersectLocal(r);
            Check.That(xs).IsEmpty();
        }
        
        [Fact]
        public void ARayMissesTheP2P3EdgeTest()
        {
            var t = new Triangle(Helper.CreatePoint(0, 1, 0), Helper.CreatePoint(-1, 0, 0), Helper.CreatePoint(1, 0, 0));
            var r = Helper.Ray(Helper.CreatePoint(0, -1, -2), Helper.CreateVector(0, 0, 1));
            var xs = t.IntersectLocal(r);
            Check.That(xs).IsEmpty();
        }

        [Fact]
        public void ARayStrikesATriangleTest()
        {
            var t = new Triangle(Helper.CreatePoint(0, 1, 0), Helper.CreatePoint(-1, 0, 0), Helper.CreatePoint(1, 0, 0));
            var r = Helper.Ray(Helper.CreatePoint(0, 0.5, -2), Helper.CreateVector(0, 0, 1));
            var xs = t.IntersectLocal(r);
            Check.That(xs).CountIs(1);
            Check.That(xs[0].T).IsEqualTo(2);
        }
    }
}