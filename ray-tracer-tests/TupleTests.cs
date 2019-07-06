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

        [Fact]
        public void EqualsTest()
        {
            var v1 = new Tuple(3, -2, 5, 1);
            var v2 = new Tuple(-2, 3, 1, 0);
            Check.That(v1).IsEqualTo(v1);
            Check.That(v2).IsEqualTo(v2);
            Check.That(v1).IsEqualTo(new Tuple(3, -2, 5, 1));
            Check.That(v1).IsNotEqualTo(v2);
        }

        [Fact]
        public void AddTest()
        {
            var v1 = new Tuple(3, -2, 5, 1);
            var v2 = new Tuple(-2, 3, 1, 0);
            var v3 = v1.Add(v2);
            Check.That(v3).IsEqualTo(new Tuple(1, 1, 6, 1));
        }

        [Fact]
        public void AddOperatorTest()
        {
            var v1 = new Tuple(3, -2, 5, 1);
            var v2 = new Tuple(-2, 3, 1, 0);
            var v3 = v1 + v2;
            Check.That(v3).IsEqualTo(new Tuple(1, 1, 6, 1));
        }

        [Fact]
        public void Sub_PointPoint_Test()
        {
            var p1 = Helper.CreatePoint(3, 2, 1);
            var p2 = Helper.CreatePoint(5, 6, 7);

            var v3 = p1.Sub(p2);
            Check.That(v3).IsEqualTo(Helper.CreateVector(-2, -4, -6));
        }

        [Fact]
        public void SubOperator_PointPoint_Test()
        {
            var p1 = Helper.CreatePoint(3, 2, 1);
            var p2 = Helper.CreatePoint(5, 6, 7);

            var v3 = p1 - p2;
            Check.That(v3).IsEqualTo(Helper.CreateVector(-2, -4, -6));
        }

        [Fact]
        public void Sub_PointVector_Test()
        {
            var p1 = Helper.CreatePoint(3, 2, 1);
            var v2 = Helper.CreateVector(5, 6, 7);

            var v3 = p1.Sub(v2);
            Check.That(v3).IsEqualTo(Helper.CreatePoint(-2, -4, -6));
        }

        [Fact]
        public void SubOperator_PointVector_Test()
        {
            var p1 = Helper.CreatePoint(3, 2, 1);
            var v2 = Helper.CreateVector(5, 6, 7);

            var v3 = p1 - v2;
            Check.That(v3).IsEqualTo(Helper.CreatePoint(-2, -4, -6));
        }


        [Fact]
        public void Sub_VectorVector_Test()
        {
            var v1 = Helper.CreateVector(3, 2, 1);
            var v2 = Helper.CreateVector(5, 6, 7);

            var v3 = v1.Sub(v2);
            Check.That(v3).IsEqualTo(Helper.CreateVector(-2, -4, -6));
        }

        [Fact]
        public void SubOperator_VectorVector_Test()
        {
            var v1 = Helper.CreateVector(3, 2, 1);
            var v2 = Helper.CreateVector(5, 6, 7);

            var v3 = v1 - v2;
            Check.That(v3).IsEqualTo(Helper.CreateVector(-2, -4, -6));
        }
    }
}
