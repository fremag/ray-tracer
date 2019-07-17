using System.Linq;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class SphereTests
    {
        [Fact]
        public void IntersectTwoPointsTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(4, 6);
        }
        
        [Fact]
        public void IntersectTangentTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 1, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(5, 5);
        }
        
        [Fact]
        public void IntersectMissSphereTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 2, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs).IsEmpty();
        }
        
        [Fact]
        public void IntersectInsideSphereTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(-1, 1);
            Check.That(xs.Select(i => i.Object)).ContainsExactly(sphere, sphere);
        }

        [Fact]
        public void IntersectSphereBehindRayTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, 5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(-6, -4);
        }
    }
}