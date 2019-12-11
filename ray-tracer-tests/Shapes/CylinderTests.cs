using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class CylinderTests
    {
        [Theory]
        [InlineData(1, 0, 0, 0, 1, 0)]
        [InlineData(0, 0, 0, 0, 1, 0)]
        [InlineData(0, 0, -5, 1, 1, 1)]
        public void ARayMissesACylinderTest(double x, double y, double z, double dx, double dy, double dz)
        {
            var cyl = new Cylinder();
            var direction = Helper.CreateVector(dx, dy, dz).Normalize();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), direction);
            var xs = cyl.IntersectLocal(ref r.Origin.vector, ref r.Direction.vector);
            Check.That(xs).CountIs(0);
        }

        [Theory]
        [InlineData(1, 0, -5, 0, 0, 1, 5, 5)]
        [InlineData(0, 0, -5, 0, 0, 1, 4, 6)]
        [InlineData(0.5, 0, -5, 0.1, 1, 1, 6.80798, 7.08872)]
        public void ARayStrikesACylinderTest(double x, double y, double z, double dx, double dy, double dz, double t0, double t1)
        {
            var cyl = new Cylinder();
            var direction = Helper.CreateVector(dx, dy, dz).Normalize();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), direction);
            var xs = cyl.IntersectLocal(ref r.Origin.vector, ref r.Direction.vector);
            Check.That(xs).CountIs(2);
            Check.That(xs[0].T).IsCloseTo(t0, 1e-4);
            Check.That(xs[1].T).IsCloseTo(t1, 1e-4);
        }

        [Theory]
        [InlineData(1, 0, 0, 1, 0, 0)]
        [InlineData(0, 5, -1, 0, 0, -1)]
        [InlineData(0, -2, 1, 0, 0, 1)]
        [InlineData(-1, 1, 0, -1, 0, 0)]
        public void NormalAtTest(double x, double y, double z, double dx, double dy, double dz)
        {
            var cyl = new Cylinder();
            var normal = Helper.CreateVector(dx, dy, dz).Normalize();
            var p = Helper.CreatePoint(x, y, z);
            var n = cyl.NormalAtLocal(p);
            Check.That(n).IsEqualTo(normal);
        }

        [Theory]
        [InlineData(0, 1.5, 0, 0.1, 1, 0, 0)]
        [InlineData(0, 3, -5, 0, 0, 1, 0)]
        [InlineData(0, 0, -5, 0, 0, 1, 0)]
        [InlineData(0, 2, -5, 0, 0, 1, 0)]
        [InlineData(0, 1, -5, 0, 0, 1, 0)]
        [InlineData(0, 1.5, -2, 0, 0, 1, 2)]
        public void IntersectingAConstrainedCylinderTest(double x, double y, double z, double dx, double dy, double dz, int c)
        {
            var cyl = new Cylinder {Maximum = 2, Minimum = 1};
            var direction = Helper.CreateVector(dx, dy, dz).Normalize();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), direction);
            var xs = cyl.IntersectLocal(ref r.Origin.vector, ref r.Direction.vector);
            Check.That(xs).CountIs(c);
        }

        [Theory]
        [InlineData(0, 3, 0, 0, -1, 0, 2)]
        [InlineData(0, 3, -2, 0, -1, 2, 2)]
        [InlineData(0, 4, -2, 0, -1, 1, 2)]
        [InlineData(0, 0, -2, 0, 1, 2, 2)]
        [InlineData(0, -1, -2, 0, 1, 1, 2)]
        public void IntersectingTheCapsOfAClosedCylinderTest(double x, double y, double z, double dx, double dy, double dz, int c)
        {
            var cyl = new Cylinder {Minimum = 1, Maximum = 2, Closed = true};
            var direction = Helper.CreateVector(dx, dy, dz).Normalize();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), direction);
            var xs = cyl.IntersectLocal(ref r.Origin.vector, ref r.Direction.vector);
            Check.That(xs).CountIs(c);
        }

        [Fact]
        public void MinMaxTest()
        {
            var cyl = new Cylinder();
            Check.That(double.IsNegativeInfinity(cyl.Minimum)).IsTrue();
            Check.That(double.IsPositiveInfinity(cyl.Maximum)).IsTrue();
        }

        [Theory]
        [InlineData(0, 1, 0, 0, -1, 0)]
        [InlineData(0.5, 1, 0, 0, -1, 0)]
        [InlineData(0, 1, 0.5, 0, -1, 0)]
        [InlineData(0, 2, 0, 0, 1, 0)]
        [InlineData(0.5, 2, 0, 0, 1, 0)]
        [InlineData(0, 2, 0.5, 0, 1, 0)]
        public void TheNormalVectorOnACylinderEndCapsTest(double x, double y, double z, double dx, double dy, double dz)
        {
            var cyl = new Cylinder {Minimum = 1, Maximum = 2, Closed = true};
            var n = cyl.NormalAtLocal(Helper.CreatePoint(x, y, z));
            Check.That(n).IsEqualTo(Helper.CreateVector(dx, dy, dz));
        }
    }
}