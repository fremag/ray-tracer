using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class ScalingTests
    {
        [Fact]
        public void ScalingTest()
        {
            var s = Helper.Scaling(2, 3, 4);
            var p = Helper.CreatePoint(-4, 6, 8);
            var scaled = s * p;
            Check.That(scaled).IsEqualTo(Helper.CreatePoint(-8, 18, 32));
        }

        [Fact]
        public void InverseScalingTest()
        {
            var s = Helper.Scaling(2, 3, 4);

            var invS = s.Invert();
            var p = Helper.CreatePoint(-4, 6, 8);
            var scaled = invS * p;
            Check.That(scaled).IsEqualTo(Helper.CreatePoint(-2, 2, 2));
        }

        [Fact]
        public void ScalingVectorTest()
        {
            var s = Helper.Scaling(2, 3, 4);
            var v = Helper.CreateVector(-4, 6, 8);
            var scaled = s * v;
            Check.That(scaled).IsEqualTo(Helper.CreateVector(-8, 18, 32));
        }
    }
}