using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class MaterialTests
    {
        [Fact]
        public void DefaultMaterialTest()
        {
            Material m = new Material();
            Check.That(m.Color).IsEqualTo(new Color(1,1,1));
            Check.That(m.Ambient).IsEqualTo(0.1);
            Check.That(m.Diffuse).IsEqualTo(0.9);
            Check.That(m.Specular).IsEqualTo(0.9);
            Check.That(m.Shininess).IsEqualTo(200);
        }

        [Fact]
        public void MaterialTest()
        {
            var c = new Color(1, 1, 1);
            Material m = new Material(c, 1,2,3,4);
            Check.That(m.Color).IsEqualTo(c);
            Check.That(m.Ambient).IsEqualTo(1);
            Check.That(m.Diffuse).IsEqualTo(2);
            Check.That(m.Specular).IsEqualTo(3);
            Check.That(m.Shininess).IsEqualTo(4);
        }
    }
}