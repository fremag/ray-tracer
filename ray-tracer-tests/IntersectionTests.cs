using System;
using System.Linq;
using NFluent;
using ray_tracer.Shapes;
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
            Check.That(comps.OverPoint.Z).Not.IsStrictlyGreaterThan(Helper.Epsilon / 2).And.Not.IsStrictlyGreaterThan(comps.Point.Z);
        }

        [Fact]
        public void TheUnderPointIsOffsetBelowTheSurfaceTest()
        {
            var r = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var shape = Helper.CreateGlassSphere();
            shape.Transform = Helper.Translation(0, 0, 1);
            var intersection = new Intersection(5, shape);
            var intersections = new Intersections(new[] {intersection});
            var comps = new IntersectionData(intersection, r, intersections);
            Check.That(comps.UnderPoint.Z).IsStrictlyGreaterThan(Helper.Epsilon / 2).And.IsStrictlyGreaterThan(comps.Point.Z);
        }

        [Fact]
        public void ReflectNormalTest()
        {
            var shape = new Plane();
            var r = Helper.Ray(Helper.CreatePoint(0, 1, -1), Helper.CreateVector(0, -Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
            var intersection = new Intersection(Math.Sqrt(2), shape);
            var comps = intersection.Compute(r);
            Check.That(comps.ReflectionVector).IsEqualTo(Helper.CreateVector(0, Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
        }

        [Theory]
        [InlineData(0, 1.0, 1.5)]
        [InlineData(1, 1.5, 2.0)]
        [InlineData(2, 2.0, 2.5)]
        [InlineData(3, 2.5, 2.5)]
        [InlineData(4, 2.5, 1.5)]
        [InlineData(5, 1.5, 1.0)]
        public void ComputeN1N2Test(int i, double n1, double n2)
        {
            var a = Helper.CreateGlassSphere();
            a.Transform = Helper.Scaling(2, 2, 2);
            a.Material.RefractiveIndex = 1.5;

            var b = Helper.CreateGlassSphere();
            b.Transform = Helper.Translation(0, 0, -0.25);
            b.Material.RefractiveIndex = 2.0;

            var c = Helper.CreateGlassSphere();
            c.Transform = Helper.Translation(0, 0, 0.25);
            c.Material.RefractiveIndex = 2.5;

            var r = Helper.Ray(Helper.CreatePoint(0, 0, -4), Helper.CreateVector(0, 0, 1));
            var xs = new Intersections
            {
                new Intersection(2, a),
                new Intersection(2.75, b),
                new Intersection(3.25, c),
                new Intersection(4.75, b),
                new Intersection(5.25, c),
                new Intersection(6, a)
            };

            var intersectionData = xs[i].Compute(r, xs);
            Check.That(intersectionData.N1).IsEqualTo(n1);
            Check.That(intersectionData.N2).IsEqualTo(n2);
        }

        [Fact]
        public void SchlickApproximation_UnderTotalInternalReflectionTest()
        {
            var shape = Helper.CreateGlassSphere();
            double sqrt2 = Math.Sqrt(2);
            var r = Helper.Ray(Helper.CreatePoint(0, 0, sqrt2 / 2), Helper.CreateVector(0, 1, 0));
            var xs = new Intersections {new Intersection(-sqrt2 / 2, shape), new Intersection(sqrt2 / 2, shape)};
            var comps = xs[1].Compute(r, xs);
            var reflectance = comps.Schlick();
            Check.That( reflectance).IsEqualTo(1.0);
        }

        [Fact]
        public void SchlickApproximationUnder_PerpendicularViewingAngleTest()
        {
            var shape = Helper.CreateGlassSphere();
            var r = Helper.Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 1, 0));
            var xs = new Intersections {new Intersection(-1, shape), new Intersection(1, shape)};
            var comps = xs[1].Compute(r, xs);
            var reflectance = comps.Schlick();
            Check.That( reflectance).IsCloseTo(0.04, 1e-5);
        }
        
        [Fact]
        public void SchlickApproximationUnder_SmallAngleAndN2GreaterThanN1Test()
        {
            var shape = Helper.CreateGlassSphere();
            var r = Helper.Ray(Helper.CreatePoint(0, 0.99, -2), Helper.CreateVector(0, 0, 1));
            var xs = new Intersections {new Intersection(1.8589, shape)};
            var comps = xs[0].Compute(r, xs);
            var reflectance = comps.Schlick();
            Check.That( reflectance).IsCloseTo(0.48873, 1e-5);
        }
    }
}