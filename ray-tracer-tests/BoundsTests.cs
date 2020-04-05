using System.Linq;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class BoundsTests
    {
        [Theory]
        [InlineData(5, -2, 0, true)]
        [InlineData(11, 4, 7, true)]
        [InlineData(8, 1, 3, true )]
        [InlineData(3, 0, 3, false)]
        [InlineData(8, -4, 3, false)]
        [InlineData(8, 1, -1, false)]
        [InlineData(13, 1, 3, false)]
        [InlineData(8, 5, 3, false)]
        [InlineData(8, 1, 8, false)]
        public void ContainsTest(double x, double y, double z, bool isInside)
        {
            var box = new Bounds
            {
                PMin = Helper.CreatePoint(5, -2, 0),
                PMax = Helper.CreatePoint(11, 4, 7)
            };
            var p = Helper.CreatePoint(x, y, z);
            Check.That(box.Contains(ref p)).IsEqualTo(isInside);

        }
        
        [Fact]
        public void Split_PerfectCubeTest()
        {
            var box = new Bounds
            {
                PMin = Helper.CreatePoint(-1, -4, -5),
                PMax = Helper.CreatePoint(9, 6, 5)
            };

            var splits = box.Split().ToArray();
            Check.That(splits[0].PMin).IsEqualTo(Helper.CreatePoint(-1, -4, -5));
            Check.That(splits[0].PMax).IsEqualTo(Helper.CreatePoint(4, 6, 5));
            Check.That(splits[1].PMin).IsEqualTo(Helper.CreatePoint(4, -4, -5));
            Check.That(splits[1].PMax).IsEqualTo(Helper.CreatePoint(9, 6, 5));
        }

        [Fact]
        public void Split_XWideBoxTest()
        {
            var box = new Bounds
            {
                PMin = Helper.CreatePoint(-1, -2, -3),
                PMax = Helper.CreatePoint(9, 5.5, 3)
            };

            var splits = box.Split().ToArray();
            Check.That(splits[0].PMin).IsEqualTo(Helper.CreatePoint(-1, -2, -3));
            Check.That(splits[0].PMax).IsEqualTo(Helper.CreatePoint(4, 5.5, 3));
            Check.That(splits[1].PMin).IsEqualTo(Helper.CreatePoint(4, -2, -3));
            Check.That(splits[1].PMax).IsEqualTo(Helper.CreatePoint(9, 5.5, 3));
        }

        [Fact]
        public void Split_YWideBoxTest()
        {
            var box = new Bounds
            {
                PMin = Helper.CreatePoint(-1, -2, -3),
                PMax = Helper.CreatePoint(5, 8, 3)
            };

            var splits = box.Split().ToArray();
            Check.That(splits[0].PMin).IsEqualTo(Helper.CreatePoint(-1, -2, -3));
            Check.That(splits[0].PMax).IsEqualTo(Helper.CreatePoint(5, 3, 3));
            Check.That(splits[1].PMin).IsEqualTo(Helper.CreatePoint(-1, 3, -3));
            Check.That(splits[1].PMax).IsEqualTo(Helper.CreatePoint(5, 8, 3));
        }

        [Fact]
        public void Split_ZWideBoxTest()
        {
            var box = new Bounds
            {
                PMin = Helper.CreatePoint(-1, -2, -3),
                PMax = Helper.CreatePoint(5, 3, 7)
            };

            var splits = box.Split().ToArray();
            Check.That(splits[0].PMin).IsEqualTo(Helper.CreatePoint(-1, -2, -3));
            Check.That(splits[0].PMax).IsEqualTo(Helper.CreatePoint(5, 3, 2));
            Check.That(splits[1].PMin).IsEqualTo(Helper.CreatePoint(-1, -2, 2));
            Check.That(splits[1].PMax).IsEqualTo(Helper.CreatePoint(5, 3, 7));
        }
    }
}