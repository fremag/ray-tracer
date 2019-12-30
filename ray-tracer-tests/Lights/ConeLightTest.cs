using NFluent;
using ray_tracer.Lights;
using Xunit;

namespace ray_tracer.tests.Lights
{
    public class ConeLightTest
    {
        [Fact]
        public void BasicTest()
        {
            var p1 = Helper.CreatePoint(0, 2, 0);
            var p2 = Helper.CreatePoint(0, 1, 0);
            
            ConeLight coneLight = new ConeLight(p1, Color.White, p2, 1, 1);

            var p3 = Helper.CreatePoint(0, 0, 0);
            var c= coneLight.GetIntensityAt(ref p3);
            Check.That(c).IsEqualTo(Color.White);
            
            var p4 = Helper.CreatePoint(-5, 0, 0);
            c= coneLight.GetIntensityAt(ref p4);
            Check.That(c).IsEqualTo(Color.Black);
        }
    }
}