using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests.Patterns
{
    public class PerlinTests
    {
        [Fact]
        public void BasicTest()
        {
            Perlin perlin = new Perlin();
            var p1 = perlin.perlin(0,0,0);
            var p2 = perlin.perlin(1,0,0);
            var p3 = perlin.perlin(2,0,0);
            var p4 = perlin.perlin(3,0,0);
            
        }
    }
}