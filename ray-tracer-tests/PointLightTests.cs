using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class PointLightTests
    {
        [Fact]
        public void PointLightTest()
        {
            var intensity = new Color(0.1, 0.2, 0.3);
            var p = Helper.CreatePoint(1, 2, 3);

            var light = new PointLight(p, intensity);
            Check.That(light.Intensity).IsEqualTo(intensity);
            Check.That(light.Position).IsEqualTo(p);
        }
    }
}