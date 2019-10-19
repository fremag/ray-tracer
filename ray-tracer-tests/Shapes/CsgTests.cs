using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class CsgTests
    {
        [Theory]
        [InlineData( true , true , true , false )]
        [InlineData( true , true , false , true )]
        [InlineData( true , false , true , false )]
        [InlineData( true , false , false , true )]
        [InlineData( false , true , true , false )]
        [InlineData( false , true , false , false )]
        [InlineData( false , false , true , true )]
        [InlineData( false , false , false , true )]
        public void Union_IntersectionAllowedTest(bool leftHit, bool insideLeft, bool insideRight, bool expected)
        {
            var unionCsg = new CsgUnion(new Sphere(), new Sphere());
            Check.That(unionCsg.IntersectionAllowed(leftHit, insideLeft, insideRight)).IsEqualTo(expected);
        } 
        
        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, true, true)]
        [InlineData(true, false, false, false)]
        [InlineData(false, true, true, true)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, false)]
        public void IntersectionAllowedTest(bool leftHit, bool insideLeft, bool insideRight, bool expected)
        {
            var intersectionCsg = new CsgIntersection(new Sphere(), new Sphere());
            Check.That(intersectionCsg.IntersectionAllowed(leftHit, insideLeft, insideRight)).IsEqualTo(expected);
        } 
        
        [Theory]
        [InlineData( true , true , true , false )]
        [InlineData( true , true , false , true )]
        [InlineData( true , false , true , false )]
        [InlineData( true , false , false , true )]
        [InlineData( false , true , true , true )]
        [InlineData( false , true , false , true )]
        [InlineData( false , false , true , false )]
        [InlineData( false , false , false , false )]
        public void DifferenceAllowedTest(bool leftHit, bool insideLeft, bool insideRight, bool expected)
        {
            var diffCsg = new CsgDifference(new Sphere(), new Sphere());
            Check.That(diffCsg.IntersectionAllowed(leftHit, insideLeft, insideRight)).IsEqualTo(expected);
        }

        private readonly IShape s1 = new Sphere();
        private readonly IShape s2 = new Cube();
        
        private void FilteringIntersectionsTest(AbstractCsg csg, int x0, int x1)
        {
            var xs = new Intersections(new Intersection(1, s1), new Intersection(2, s2), new Intersection(3, s1), new Intersection(4, s2));
            var result = csg.Filter(xs);
            Check.That(result).CountIs(2);
            Check.That(result[0]).IsEqualTo(xs[x0]);
            Check.That(result[1]).IsEqualTo(xs[x1]);
        }

        [Fact]
        public void FilteringIntersections_Union_Test()
        {
            var csg = new CsgUnion(s1, s2);
            FilteringIntersectionsTest(csg, 0, 3); 
        }
        [Fact]
        public void FilteringIntersections_Intersection_Test()
        {
            var csg = new CsgIntersection(s1, s2);
            FilteringIntersectionsTest(csg, 1, 2); 
        }
        [Fact]
        public void FilteringIntersections_Difference_Test()
        {
            var csg = new CsgDifference(s1, s2);
            FilteringIntersectionsTest(csg, 0, 1); 
        }

        [Fact]
        public void RayMissesCsgObjectTest()
        {
            var c = new CsgUnion(new Sphere(), new Cube());
            var r = Helper.Ray(0, 2, -5, 0, 0, 1);
            var xs = c.IntersectLocal(r);
            Check.That(xs).IsEmpty();
        }

        [Fact]
        public void RayHitsCsgObject()
        {
            var sphere1 = new Sphere();
            var sphere2 = new Sphere();
            sphere2.Translate(0, 0, 0.5);

            var csg = new CsgUnion(sphere1, sphere2);
            var r = Helper.Ray(0, 0, -5, 0, 0, 1);
            var xs = csg.IntersectLocal(r);
            Check.That(xs).CountIs(2);
            Check.That(xs[0].T).IsEqualTo(4);
            Check.That(xs[0].Object).IsEqualTo(sphere1);
            Check.That(xs[1].T).IsEqualTo(6.5);
            Check.That(xs[1].Object).IsEqualTo(sphere2);
        }
    }
}