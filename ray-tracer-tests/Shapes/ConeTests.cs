using System;
using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class ConeTests
    {
        [Theory]
        [InlineData(0, 0, -5, 0, 0, 1, 5 , 5 )]
        [InlineData(0, 0, -5, 1, 1, 1, 8.66025 , 8.66025 )]
        [InlineData(1, 1, -5, -0.5, -1, 1, 4.55006 , 49.44994 )]
        public void IntersectingConeWithRayTest(double x, double y, double z, double dx, double dy, double dz, double t0, double t1)
        {
            var shape = new Cone();
            var direction = Helper.CreateVector(dx, dy, dz).Normalize();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), direction);
            var xs = shape.IntersectLocal(r);
            
            Check.That(xs).CountIs(2);

            Check.That(xs[0].T).IsCloseTo(t0, Helper.Epsilon);
            Check.That(xs[1].T).IsCloseTo(t1, Helper.Epsilon);
        }

        [Fact]
        public void IntersectingConeWithRayParallelToOneOfItsHalvesTest()
        {
            var shape = new Cone();
            var direction = Helper.CreateVector(0, 1, 1).Normalize();
            var r = Helper.Ray(Helper.CreatePoint(0, 0, -1), direction);
            var xs = shape.IntersectLocal(r);
            Check.That(xs).CountIs(1);
            Check.That(xs[0].T).IsCloseTo(0.35355, Helper.Epsilon);
        }

        [Theory]
        [InlineData(0, 0, -5, 0, 1, 0, 0)]
        [InlineData(0, 0, -0.25, 0, 1, 1, 2)]
        [InlineData(0, 0, -0.25, 0, 1, 0, 4)]
        public void IntersectingConeEndCapsTest(double x, double y, double z, double dx, double dy, double dz, int c)
        {
            var shape = new Cone(-0.5, 0.5, true);
            var direction = Helper.CreateVector(dx, dy, dz).Normalize();
            var r = Helper.Ray(Helper.CreatePoint(x, y, z), direction);
            var xs = shape.IntersectLocal(r);
            Check.That(xs).CountIs(c);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(-1, -1, 0, -1, 1, 0)]
        public void ComputingNormalVectorTest(double x, double y, double z, double dx, double dy, double dz)
        {
            var shape = new Cone();
            var n = shape.NormalAtLocal(Helper.CreatePoint(x, y, z));
            Check.That(n).IsEqualTo(Helper.CreateVector(dx, dy, dz));
        }
        
        [Fact]
        public void ComputingNormalVector_Negative_Y_Test()
        {
           ComputingNormalVectorTest(1, 1, 1, 1, -Math.Sqrt(2), 1); 
        }
    }
}