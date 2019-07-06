using System;
using NFluent;
using Xunit;

namespace ray_tracer_tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Check.That(1 + 1).IsEqualTo(2);
        }
    }
}
