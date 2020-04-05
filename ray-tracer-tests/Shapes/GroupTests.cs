using System;
using System.Collections.Generic;
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
            var xs =  new Intersections();
            g.IntersectLocal(ref r.Origin, ref r.Direction, xs);
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
            var xs =  new Intersections();
            g.IntersectLocal(ref r.Origin, ref r.Direction, xs);
            xs.Sort();
            Check.That(xs).CountIs(4);
            Check.That(xs[0].Object).IsSameReferenceAs(s2);
            Check.That(xs[1].Object).IsSameReferenceAs(s2);
            Check.That(xs[2].Object).IsSameReferenceAs(s1);
            Check.That(xs[3].Object).IsSameReferenceAs(s1);
        }

        [Fact]
        public void IntersectingTransformedGroupTest()
        {
            Group g = new Group();
            g.Scale(2);
            var s = Helper.Sphere().Translate(tx: 5);
            g.Add(s);
            var r = Helper.Ray(10, 0, -10, 0, 0, 1);
            var xs =  new Intersections();
            g.Intersect(ref r.Origin, ref r.Direction, xs);
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

        [Fact]
        public void BoundsTest()
        {
            var s = new Sphere();
            s.Scale(2).Translate(2, 5, -3);
            
            var cyl = new Cylinder(-2, 2);
            cyl.Scale(0.5, 1, 0.5).Translate(-4, -1, 4);
            
            var g = new Group().Add(s).Add(cyl);
            var box = g.Box;
            Check.That(box.PMin).IsEqualTo(Helper.CreatePoint(-4.5, -3, -5));
            Check.That(box.PMax).IsEqualTo(Helper.CreatePoint(4, 7, 4.5));
        }

        [Fact]
        public void PartitionTest()
        {
            var s1 = new Sphere().Translate(-2);
            var s2 = new Sphere().Translate(2);
            var s3 = new Sphere();
            var g = new Group().Add(s1, s2, s3);
            var left = new List<IShape>();
            var right = new List<IShape>();
            g.Partition(left, right);
            Check.That(g.Count).IsEqualTo(1);
            Check.That(g[0]).IsSameReferenceAs(s3);
            Check.That(left).ContainsExactly(s1);
            Check.That(right).ContainsExactly(s2);
        }
        
        [Fact]
        public void DivideTest()
        {
            var s1 = new Sphere().Translate(-2);
            var s2 = new Sphere().Translate(2);
            var s3 = new Sphere();
            var g = new Group().Add(s1, s2, s3);
            g.Divide(1);
            Check.That(g.Count).IsEqualTo(3);
            Check.That(g[0]).IsSameReferenceAs(s3);
            var left = g[1] as Group;
            Check.That(left).IsNotNull();
            var right = g[2] as Group;
            Check.That(right).IsNotNull();
            Check.That(left.Count).IsEqualTo(1);
            Check.That(right.Count).IsEqualTo(1);
            Check.That(left[0]).IsSameReferenceAs(s1);
            Check.That(right[0]).IsSameReferenceAs(s2);
        }

        
        [Fact]
        public void Divide_TooFewChildrenTest()
        {
            var s1 = new Sphere().Translate(-2);
            var s2 = new Sphere().Translate(2, 1);
            var s3 = new Sphere().Translate(2, -1);
            var subGroup = new Group().Add(s1, s2, s3);
            var s4 = new Sphere();
            var g = new Group();
            g.Add(subGroup, s4);
            g.Divide(3);
            Check.That(g[0]).IsSameReferenceAs(subGroup);
            Check.That(g[1]).IsSameReferenceAs(s4);
            Check.That(subGroup.Count).IsEqualTo(2);
            var subG1 = subGroup[0] as Group;
            var subG2 = subGroup[1] as Group;
            Check.That(subG1[0]).IsSameReferenceAs(s1);
            Check.That(subG2[0]).IsSameReferenceAs(s2);
            Check.That(subG2[1]).IsSameReferenceAs(s3);
        }
    }
}