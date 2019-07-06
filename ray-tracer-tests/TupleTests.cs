using NFluent;
using raytracer;
using Xunit;

namespace ray_tracer_tests
{
    public class TupleTests
    {
        [Fact]
        public void TuplePointTest()
        {
            Tuple tuple = new Tuple(1, 2, 3, 1);
            Check.That(tuple.IsPoint()).IsTrue();
            Check.That(tuple.IsVector()).IsFalse();
        }
        [Fact]
        public void TupleVectorTest()
        {
            Tuple tuple = new Tuple(1, 2, 3, 0);
            Check.That(tuple.IsPoint()).IsFalse();
            Check.That(tuple.IsVector()).IsTrue();
        }

        [Fact]
        public void CreatePointTest()
        {
            var v = Helper.CreatePoint(1, 2, 3);
            Check.That(v.IsPoint()).IsTrue();
        }

        [Fact]
        public void CreateVectorTest()
        {
            var v = Helper.CreateVector(1, 2, 3);
            Check.That(v.IsVector()).IsTrue();
        }
    }
}
