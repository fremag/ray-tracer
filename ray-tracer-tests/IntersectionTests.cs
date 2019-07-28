using System.Linq;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class IntersectionTests
    {
        [Fact]
        public void IntersectionTest()
        {
            var sphere = Helper.Sphere();
            var i = new Intersection(3.5, sphere);
            Check.That(i.T).IsEqualTo(3.5);
            Check.That(i.Object).IsEqualTo(sphere);
        }

        [Fact]
        public void IntersectionAggregationTest()
        {
            var sphere = Helper.Sphere();
            var i1 = new Intersection(1, sphere);
            var i2 = new Intersection(2, sphere);
            var intersections = Helper.Intersections(i1, i2);
            Check.That(intersections.Count).IsEqualTo(2);
            Check.That(intersections).ContainsExactly(i1, i2);
            Check.That(intersections.Select(i => i.T)).ContainsExactly(1, 2);
            Check.That(intersections.Select(i => i.Object)).ContainsExactly(sphere, sphere);
        }

        [Fact]
        public void HitAllIntersectionPositiveTest()
        {
            var s = Helper.Sphere();
            var i1 = Helper.Intersection(1, s);
            var i2 = Helper.Intersection(2, s);
            var xs = Helper.Intersections(i2, i1);
            var i = xs.Hit();
            Check.That(i).IsEqualTo(i1);
        }

        [Fact]
        public void HitIntersectionPositiveNegativeTest()
        {
            var s = Helper.Sphere();
            var i1 = Helper.Intersection(-1, s);
            var i2 = Helper.Intersection(1, s);
            var xs = Helper.Intersections(i2, i1);
            var i = xs.Hit();
            Check.That(i).IsEqualTo(i2);
        }

        [Fact]
        public void HitAllIntersectionNegativeTest()
        {
            var s = Helper.Sphere();
            var i1 = Helper.Intersection(-2, s);
            var i2 = Helper.Intersection(-1, s);
            var xs = Helper.Intersections(i2, i1);
            var i = xs.Hit();
            Check.That(i).IsNull();
        }

        [Fact]
        public void HitIntersectionLowestTest()
        {
            var s = Helper.Sphere();
            var i1 = Helper.Intersection(5, s);
            var i2 = Helper.Intersection(7, s);
            var i3 = Helper.Intersection(-3, s);
            var i4 = Helper.Intersection(2, s);
            var xs = Helper.Intersections(i1, i2, i3, i4);
            var i = xs.Hit();
            Check.That(i).IsEqualTo(i4);
        }

        [Fact]
        public void IntersectionDataTest()
        {
            var ray = new Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var intersection = Helper.Intersection(4, sphere);
            var intersectionData = intersection.Compute(ray);
            Check.That(intersectionData.T).IsEqualTo(4);
            Check.That(intersectionData.Object).IsEqualTo(sphere);
            Check.That(intersectionData.Point).IsEqualTo(Helper.CreatePoint(0, 0, -1));
            Check.That(intersectionData.EyeVector).IsEqualTo(Helper.CreateVector(0, 0, -1));
            Check.That(intersectionData.Normal).IsEqualTo(Helper.CreateVector(0, 0, -1));
        }

        [Fact]
        public void IntersectionData_NotInsideTest()
        {
            var ray = new Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var intersection = Helper.Intersection(4, sphere);
            var intersectionData = intersection.Compute(ray);
            Check.That(intersectionData.Inside).IsFalse();
        }

        [Fact]
        public void IntersectionData_Inside_Test()
        {
            var ray = new Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var intersection = Helper.Intersection(1, sphere);
            var intersectionData = intersection.Compute(ray);
            Check.That(intersectionData.Point).IsEqualTo(Helper.CreatePoint(0, 0, 1));
            Check.That(intersectionData.EyeVector).IsEqualTo(Helper.CreateVector(0, 0, -1));
            Check.That(intersectionData.Normal).IsEqualTo(Helper.CreateVector(0, 0, -1));
        }

        [Fact]
        public void TheHitShouldOffsetThePointTest()
        {
            var r = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var shape = Helper.Sphere();
            shape.Transform = Helper.Translation(0, 0, 1);
            var intersection = new Intersection(5, shape);
            var comps = new IntersectionData(intersection, r);
            Check.That(comps.OverPoint.Z).Not.IsStrictlyGreaterThan(-Helper.Epsilon / 2).And.Not
                .IsStrictlyGreaterThan(comps.Point.Z);
        }
    }
}