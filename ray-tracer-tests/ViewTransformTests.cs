using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class ViewTransformTests
    {

        [Fact]
        public void DefaultViewTransformTest()
        {
            var from = Helper.CreatePoint(0, 0, 0);
            var to = Helper.CreatePoint(0, 0, -1);
            var up = Helper.CreateVector(0, 1, 0);
            var t = Helper.ViewTransform(from, to, up);
            Check.That(t).IsEqualTo(Helper.CreateIdentity());
        }

        [Fact]
        public void ViewTransformation_LookingInPositiveZDirectionTest()
        {
            var from = Helper.CreatePoint(0, 0, 0);
            var to = Helper.CreatePoint(0, 0, 1);
            var up = Helper.CreateVector(0, 1, 0);
            var t = Helper.ViewTransform(from, to, up);
            Check.That(t).IsEqualTo(Helper.Scaling(-1, 1, -1));
        }
        
        [Fact]
        public void ViewTransformation_Translation()
        {
            var from = Helper.CreatePoint(0, 0, 8);
            var to = Helper.CreatePoint(0, 0, 0);
            var up = Helper.CreateVector(0, 1, 0);
            var t = Helper.ViewTransform(from, to, up);
            Check.That(t).IsEqualTo(Helper.Translation(0, 0, -8));
        }

        [Fact]
        public void ViewTransform_ArbitraryTransformationTest()
        {
            var from = Helper.CreatePoint(1, 3, 2);
            var to = Helper.CreatePoint(4, -2, 8);
            var up = Helper.CreateVector(1, 1, 0);
            var t = Helper.ViewTransform(from, to, up);
            Check.That(t).IsEqualTo(new Matrix(4,
                -0.50709, 0.50709, 0.67612, -2.36643,
                0.76772, 0.60609, 0.12122, -2.82843,
                -0.35857, 0.59761, -0.71714, 0.00000,
                0.00000, 0.00000, 0.00000, 1.00000
            ));
        }
    }
}