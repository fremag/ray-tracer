using System;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class RotationTests
    {
        [Fact]
        public void RotationX()
        {
            var p = Helper.CreatePoint(0, 1, 0);
            var halfQ = Helper.RotationX(Math.PI / 4);
            var fullQ = Helper.RotationX(Math.PI / 2);
            Check.That(halfQ * p).IsEqualTo(Helper.CreatePoint(0, Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
            var value = fullQ * p;
            Check.That(value).IsEqualTo(Helper.CreatePoint(0, 0, 1));
        }
        [Fact]
        public void InverseRotationX()
        {
            var p = Helper.CreatePoint(0, 1, 0);
            var halfQ = Helper.RotationX(Math.PI / 4);
            var invHalfQ = halfQ.Inverse();
            Check.That(invHalfQ * p).IsEqualTo(Helper.CreatePoint(0, Math.Sqrt(2) / 2, - Math.Sqrt(2) / 2));
        }
        
        [Fact]
        public void RotationY()
        {
            var p = Helper.CreatePoint(0,  0, 1);
            var halfQ = Helper.RotationY(Math.PI / 4);
            var fullQ = Helper.RotationY(Math.PI / 2);
            Check.That(halfQ * p).IsEqualTo(Helper.CreatePoint(Math.Sqrt(2) / 2, 0, Math.Sqrt(2) / 2));
            var value = fullQ * p;
            Check.That(value).IsEqualTo(Helper.CreatePoint(1, 0, 0));
        }
        
        [Fact]
        public void RotationZ()
        {
            var p = Helper.CreatePoint(0, 1, 0);
            var halfQ = Helper.RotationZ(Math.PI / 4);
            var fullQ = Helper.RotationZ(Math.PI / 2);
            Check.That(halfQ * p).IsEqualTo(Helper.CreatePoint(-Math.Sqrt(2) / 2, Math.Sqrt(2) / 2, 0));
            var value = fullQ * p;
            Check.That(value).IsEqualTo(Helper.CreatePoint(-1, 0, 0));
        }
    }
}