using NFluent;
using Xunit;
using Color = ray_tracer.Color;

namespace ray_tracer_tests
{
    public class ColorTests
    {
        [Fact]
        public void ColorTest()
        {
            var c = new Color(1, 2, 3);
            Check.That(c.Red).IsEqualTo(1);
            Check.That(c.Green).IsEqualTo(2);
            Check.That(c.Blue).IsEqualTo(3);
        }

        [Fact]
        public void ColorBasicTest()
        {
            var c = new Color(1);
            Check.That(c.Red).IsEqualTo(1);
            Check.That(c.Green).IsEqualTo(1);
            Check.That(c.Blue).IsEqualTo(1);
        }

        [Fact]
        public void BlackTest()
        {
            var black = Color.Black;
            Check.That(black.Red).IsEqualTo(0);
            Check.That(black.Green).IsEqualTo(0);
            Check.That(black.Blue).IsEqualTo(0);
        }

        [Fact]
        public void AddTest()
        {
            var c1 = new Color(1, 2, 3);
            var c2 = new Color(2, 3, 4);
            var c = c1 + c2;

            Check.That(c.Red).IsEqualTo(3);
            Check.That(c.Green).IsEqualTo(5);
            Check.That(c.Blue).IsEqualTo(7);
        }

        [Fact]
        public void SubTest()
        {
            var c1 = new Color(1, 2, 3);
            var c2 = new Color(2, 3, 4);
            var c = c2 - c1;

            Check.That(c.Red).IsEqualTo(1);
            Check.That(c.Green).IsEqualTo(1);
            Check.That(c.Blue).IsEqualTo(1);
        }


        [Fact]
        public void ProductTest()
        {
            var c1 = new Color(1, 2, 3);
            var c2 = new Color(2, 3, 4);
            var c = c2 * c1;

            Check.That(c.Red).IsEqualTo(2);
            Check.That(c.Green).IsEqualTo(6);
            Check.That(c.Blue).IsEqualTo(12);
        }


        [Fact]
        public void MulTest()
        {
            var c1 = new Color(1, 2, 3);
            var c = c1 * 2;

            Check.That(c.Red).IsEqualTo(2);
            Check.That(c.Green).IsEqualTo(4);
            Check.That(c.Blue).IsEqualTo(6);

            c = 2 * c1;

            Check.That(c.Red).IsEqualTo(2);
            Check.That(c.Green).IsEqualTo(4);
            Check.That(c.Blue).IsEqualTo(6);
        }
    }
}