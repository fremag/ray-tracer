using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class TranslationTests
    {
        [Fact]
        public void TranslationTest()
        {
            var t = Helper.Translation(5, -3, 2);
            var p = Helper.CreatePoint(-3, 4, 5);
            var translated = t * p;
            Check.That(translated).IsEqualTo( Helper.CreatePoint(2, 1, 7));
        }

        [Fact]
        public void InverseTranslationTest()
        {
            var t = Helper.Translation(5, -3, 2);

            var invT = t.Inverse();
            var p = Helper.CreatePoint(-3, 4, 5);
            var translated = invT * p;
            Check.That(translated).IsEqualTo(Helper.CreatePoint(-8, 7, 3));
        }

        [Fact]
        public void TranslationVectorTest()
        {
            var t = Helper.Translation(5, -3, 2);
            var v = Helper.CreateVector(-3, 4, 5);
            var translated = t * v;
            Check.That(translated).IsEqualTo( v );
        }
    }
}