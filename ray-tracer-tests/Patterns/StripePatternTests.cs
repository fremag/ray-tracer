using NFluent;
using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests.Patterns
{
    public class StripePatternTests
    {
        private StripePattern pattern;
        public StripePatternTests()
        {
            pattern = new StripePattern(Color.White, Color.Black);
        }

        [Fact]
        public void BasicTest()
        {
            Check.That(pattern.ColorA).IsEqualTo(Color.White);
            Check.That(pattern.ColorB).IsEqualTo(Color.Black);
        }

        [Theory]
        [InlineData(0, 0, 0, true)]
        [InlineData(0, 1, 0, true)]
        [InlineData(0, 2, 0, true)]
        [InlineData(0, 0, 1, true)]
        [InlineData(0, 0, 2, true)]
        [InlineData(0.9, 0, 0, true)]
        [InlineData(1, 0, 0, false)]
        [InlineData(-0.1, 0, 0, false)]
        [InlineData(-1, 0, 0, false)]
        [InlineData(-1.1, 0, 0, true)]
        public void GetColorTest(double x, double y, double z, bool isWhite)
        {
            var c = pattern.GetColor(Helper.CreatePoint(x, y, z));
            if (isWhite)
            {
                Check.That(c).IsEqualTo(Color.White);
            }
            else
            {
                Check.That(c).IsEqualTo(Color.Black);
            }
        }
    }
}