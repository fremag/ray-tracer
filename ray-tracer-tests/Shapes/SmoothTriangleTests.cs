using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class SmoothTriangleTests
    {
        private Tuple p1;
        private Tuple p2;
        private Tuple p3;
        private Tuple n1;
        private Tuple n2;
        private Tuple n3;
        private SmoothTriangle tri;

        public SmoothTriangleTests()
        {
            p1 = Helper.CreatePoint(0, 1, 0);
             p2  = Helper.CreatePoint(-1, 0, 0);
             p3  = Helper.CreatePoint(1, 0, 0);
             n1 = Helper.CreateVector(0, 1, 0);
             n2 = Helper.CreateVector(-1, 0, 0);
             n3 = Helper.CreateVector(1, 0, 0);
             tri = new SmoothTriangle(p1, p2, p3, n1, n2, n3);
        }

        [Fact]
        public void ConstructingSmoothTriangleTest()
        {
            Check.That(tri.P1).IsEqualTo(p1);
            Check.That( tri.P2 ).IsEqualTo( p2);
            Check.That( tri.P3 ).IsEqualTo( p3);
            Check.That( tri.N1 ).IsEqualTo( n1);
            Check.That( tri.N2).IsEqualTo( n2);
            Check.That( tri.N3).IsEqualTo(n3);
        }

        [Fact]
        public void IntersectionCanEncapsulateUAndVTest()
        {
            var s = new Triangle(Helper.CreatePoint(0, 1, 0), Helper.CreatePoint(-1, 0, 0), Helper.CreatePoint(1, 0, 0));
            var i = new Intersection(3.5, s, 0.2, 0.4);
            Check.That(i.U).IsEqualTo(0.2);
            Check.That(i.V).IsEqualTo(0.4);
        }

        [Fact]
        public void AnIntersectionWithSmoothTriangleStoresUvTest()
        {
            var r = Helper.Ray(-0.2, 0.3, -2, 0, 0, 1);
            var xs = tri.IntersectLocal(ref r.Origin, ref r.Direction);
            Check.That(xs[0].U).IsCloseTo(0.45, Helper.Epsilon);
            Check.That(xs[0].V).IsCloseTo(0.25, Helper.Epsilon);
        }

        [Fact]
        public void SmoothTriangleUsesUvToInterpolateTheNormal()
        {
            var i = new Intersection(1, tri, 0.45, 0.25);

            var n = tri.NormalAt(Helper.CreatePoint(0, 0, 0), i);
            Check.That(n).IsEqualTo(Helper.CreateVector(-0.5547, 0.83205, 0));
        }
    }
}