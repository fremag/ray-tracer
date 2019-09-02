using NFluent;
using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests.Patterns
{
    public class GradientPatternTests
    {
        [Fact]
        public void BasicTest()
        {
            var pattern = new GradientPattern(Color.White, Color.Black);
            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0, 0))).IsEqualTo(new Color(1, 1, 1));
            Check.That(pattern.GetColor(Helper.CreatePoint(0.25, 0, 0))).IsEqualTo(new Color(0.75, 0.75, 0.75));
            Check.That(pattern.GetColor(Helper.CreatePoint(0.5, 0, 0))).IsEqualTo(new Color(0.5, 0.5, 0.5));
            Check.That(pattern.GetColor(Helper.CreatePoint(0.75, 0, 0))).IsEqualTo(new Color(0.25, 0.25, 0.25));
        }
    }
}