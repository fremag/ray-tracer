using System;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class TransformationTests
    {
        [Fact]
        public void TransformationSequenceTest()
        {
            var p = Helper.CreatePoint(1, 0, 1);
            var r = Helper.RotationX(Math.PI / 2);
            var s = Helper.Scaling(5, 5, 5);
            var t = Helper.Translation(10, 5, 7);
            var p2 = r * p;
            Check.That(p2).IsEqualTo(Helper.CreatePoint(1, -1, 0));

            var p3 = s * p2;
            Check.That(p3).IsEqualTo(Helper.CreatePoint(5, -5, 0));

            var p4 = t * p3;
            Check.That(p4).IsEqualTo(Helper.CreatePoint(15, 0, 7));            
        }
    
        [Fact]
        public void ChainedTransformationTest()
        {
            var p = Helper.CreatePoint(1, 0, 1);
            var r = Helper.RotationX(Math.PI / 2);
            var s = Helper.Scaling(5, 5, 5);
            var t = Helper.Translation(10, 5, 7);
            var m = t * s * r; 
            var p2 = m * p;
            Check.That(p2).IsEqualTo(Helper.CreatePoint(15, 0, 7));            
        }
    }
}