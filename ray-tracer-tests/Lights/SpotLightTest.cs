using NFluent;
using ray_tracer.Lights;
using Xunit;

namespace ray_tracer.tests.Lights
{
    public class SpotLightTest
    {
        [Fact]
        public void BasicTest()
        {
            var p1 = Helper.CreatePoint(0, 2, 0);
            var p2 = Helper.CreatePoint(0, 1, 0);
            
            SpotLight spotLight = new SpotLight(p1, Color.White, p2, 1, 1);

            var p3 = Helper.CreatePoint(0, 0, 0);
            var c= spotLight.GetIntensityAt(p1.X, p1.Y, p1.Z, ref p3);
            Check.That(c).IsEqualTo(Color.White);
            
            var p4 = Helper.CreatePoint(-5, 0, 0);
            c= spotLight.GetIntensityAt(p1.X, p1.Y, p1.Z, ref p4);
            Check.That(c).IsEqualTo(Color.Black);
        }
    }
}