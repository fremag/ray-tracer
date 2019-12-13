using System;
using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class GroupTests
    {
        [Fact]
        public void IntersectingRayWithAnEmptyGroupTest()
        {
            var g = new Group();
            var r = Helper.Ray(0, 0, 0, 0, 0, 1);
            var xs = g.IntersectLocal(ref r.Origin, ref r.Direction);
            Check.That(xs).IsEmpty();
        }

        [Fact]
        public void IntersectingRayWithNonEmptyGroupTest()
        {
            var g = new Group();
            var s1 = Helper.Sphere();
            var s2 = Helper.Sphere().Translate(tz: -3);
            var s3 = Helper.Sphere().Translate(tx: 5);
            g.Add(s1, s2, s3);

            var r = Helper.Ray(0, 0, -5, 0, 0, 1);
            var xs = g.IntersectLocal(ref r.Origin, ref r.Direction);
            Check.That(xs).CountIs(4);
            Check.That(xs[0].Object).IsEqualTo(s2);
            Check.That(xs[1].Object).IsEqualTo(s2);
            Check.That(xs[2].Object).IsEqualTo(s1);
            Check.That(xs[3].Object).IsEqualTo(s1);
        }

        [Fact]
        public void IntersectingTransformedGroupTest()
        {
            Group g = new Group();
            g.Scale(2);
            var s = Helper.Sphere().Translate(tx: 5);
            g.Add(s);
            var r = Helper.Ray(10, 0, -10, 0, 0, 1);
            var xs = g.Intersect(ref r.Origin, ref r.Direction);
            Check.That(xs).CountIs(2);
        }

        [Fact]
        public void ConvertingPointFromWorldToObjectSpaceTest()
        {
            var g1 = new Group().Rotate(ry: Math.PI / 2);
            var g2 = new Group().Scale(2);

            g1.Add(g2);

            var s = Helper.Sphere().Translate(tx: 5);
            g2.Add(s);
            var p = s.WorldToObject(Helper.CreatePoint(-2, 0, -10));
            Check.That(p).IsEqualTo(Helper.CreatePoint(0, 0, -1));
        }

        [Fact]
        public void ConvertingNormalFromObjectToWorldSpaceTest()
        {
            var g1 = new Group().Rotate(ry: Math.PI / 2);
            var g2 = new Group().Scale(1, 2, 3);
            g1.Add(g2);
            var s = Helper.Sphere().Translate(tx: 5);
            g2.Add(s);
            var n = s.NormalToWorld(Helper.CreateVector(Math.Sqrt(3) / 3, Math.Sqrt(3) / 3, Math.Sqrt(3) / 3));
            Check.That(n.X).IsCloseTo(0.2857, 1e-4);
            Check.That(n.Y).IsCloseTo(0.4286, 1e-4);
            Check.That(n.Z).IsCloseTo(-0.8571, 1e-4);
        }

        [Fact]
        public void FindingTheNormalOnAChildObjectTest()
        {
            var g1 = new Group().Rotate(ry: Math.PI / 2);
            var g2 = new Group().Scale(1, 2, 3);
            g1.Add(g2);

            var s = new Sphere().Translate(tx: 5);
            g2.Add(s);
            var n = s.NormalAt(Helper.CreatePoint(1.7321, 1.1547, -5.5774));
            Check.That(n.X).IsCloseTo(0.2857, 1e-4);
            Check.That(n.Y).IsCloseTo(0.4286, 1e-4);
            Check.That(n.Z).IsCloseTo(-0.8571, 1e-4);
        }
    }
}