using NFluent;
using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests.Patterns
{
    public class TestPatternTests
    {
        [Fact]
        public void APatternWithAnObjectTransformationTest()
        {
            var shape = Helper.Sphere();
            shape.Transform = Helper.Scaling(2, 2, 2);
            var pattern = new TestPattern();
            var point = Helper.CreatePoint(2, 3, 4);
            var c = pattern.GetColorAtShape(shape, ref point);
            Check.That(c).IsEqualTo(new Color(1, 1.5, 2));
        }

        [Fact]
        public void APatternWithAPatternTransformation()
        {
            var shape = Helper.Sphere();
            var pattern = new TestPattern();
            pattern.Transform = Helper.Scaling(2, 2, 2);
            var point = Helper.CreatePoint(2, 3, 4);
            var c = pattern.GetColorAtShape(shape, ref  point);
            Check.That(c).IsEqualTo(new Color(1, 1.5, 2));
        }

        [Fact]
        public void APatternWithBothAnObjectAndAPatternTransformation()
        {
            var shape = Helper.Sphere();
            shape.Transform = Helper.Scaling(2, 2, 2);
            var pattern = new TestPattern();
            pattern.Transform = Helper.Translation(0.5, 1, 1.5);
            var point = Helper.CreatePoint(2.5, 3, 3.5);
            var c = pattern.GetColorAtShape(shape, ref point);
            Check.That(c).IsEqualTo(new Color(0.75, 0.5, 0.25));
        }
    }
}