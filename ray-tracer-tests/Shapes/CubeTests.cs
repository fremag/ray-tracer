using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class CubeTests
    {
        [Theory]
        [InlineData(5, 0.5, 0, -1, 0, 0, 4, 6)]
        [InlineData(-5, 0.5, 0, 1, 0, 0, 4, 6)]
        [InlineData(0.5, 5, 0, 0, -1, 0, 4, 6)]
        [InlineData(0.5, -5, 0, 0, 1, 0, 4, 6)]
        [InlineData(0.5, 0, 5, 0, 0, -1, 4, 6)]
        [InlineData(0.5, 0, -5, 0, 0, 1, 4, 6)]
        [InlineData(0, 0.5, 0, 0, 0, 1, -1, 1)]
        public void ARayIntersectsACubeTest(double x, double y, double z, double dx, double dy, double dz, double t1, double t2)
        {
            var c = new Cube();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), Helper.CreateVector(dx, dy, dz));
            var xs = c.IntersectLocal(r);
            Check.That(xs.Count).IsEqualTo(2);

            Check.That(xs[0].T).IsEqualTo(t1);
            Check.That(xs[1].T).IsEqualTo(t2);
        }

        [Theory]
        [InlineData(-2, 0, 0, 0.2673, 0.5345, 0.8018)]
        [InlineData(0, -2, 0, 0.8018, 0.2673, 0.5345)]
        [InlineData(0, 0, -2, 0.5345, 0.8018, 0.2673)]
        [InlineData(2, 0, 2, 0, 0, -1)]
        [InlineData(0, 2, 2, 0, -1, 0)]
        [InlineData(2, 2, 0, -1, 0, 0)]
        public void ARayMissesACubeTest(double x, double y, double z, double dx, double dy, double dz)
        {
            var c = new Cube();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), Helper.CreateVector(dx, dy, dz));
            var xs = c.IntersectLocal(r);
            Check.That(xs).IsEmpty();
        }

        [Theory]
        [InlineData(1, 0.5, -0.8, 1, 0, 0)]
        [InlineData(-1, -0.2, 0.9, -1, 0, 0)]
        [InlineData(-0.4, 1, -0.1, 0, 1, 0)]
        [InlineData(0.3, -1, -0.7, 0, -1, 0)]
        [InlineData(-0.6, 0.3, 1, 0, 0, 1)]
        [InlineData(0.4, 0.4, -1, 0, 0, -1)]
        [InlineData(1, 1, 1, 1, 0, 0)]
        [InlineData(-1, -1, -1, -1, 0, 0)]
        public void TheNormalOnTheSurfaceOfACubeTest(double x, double y, double z, double nx, double ny, double nz)
        {
            var c = new Cube();
            var p = Helper.CreatePoint(x, y, z);
            var normal = c.NormalAtLocal(p);
            Check.That(normal.X).IsCloseTo(nx, 1e-5);
            Check.That(normal.Y).IsCloseTo(ny, 1e-5);
            Check.That(normal.Z).IsCloseTo(nz, 1e-5);
        }
    }
}