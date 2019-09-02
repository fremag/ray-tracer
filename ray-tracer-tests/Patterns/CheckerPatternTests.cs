using NFluent;
using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests.Patterns
{
    public class CheckerPatternTests
    {
        [Fact]
        public void BasicTest()
        {
            var pattern = new CheckerPattern(Color.White, Color.Black);
            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0, 0))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(0.99, 0, 0))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(1.01, 0, 0))).IsEqualTo(Color.Black);

            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0, 0))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0.99, 0))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(0, 1.01, 0))).IsEqualTo(Color.Black);

            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0, 0))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0, 0.99))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0, 1.01))).IsEqualTo(Color.Black);
            
            Check.That(pattern.GetColor(Helper.CreatePoint(0, 0, 0))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(0.99, 0, 0.99))).IsEqualTo(Color.White);
            Check.That(pattern.GetColor(Helper.CreatePoint(1.01, 1.01, 1.01))).IsEqualTo(Color.Black);
        }
    }
}