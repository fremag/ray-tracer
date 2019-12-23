using NFluent;
using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests.Patterns
{
    public class ColorMapTests
    {
        [Fact]
        public void BasicTest()
        {
            ColorMap map = new ColorMap();
            map.Add(0.5, Color.White);
            map.Add(0.75, Color._Red);

            var c = map.GetColor(0);
            Check.That(c).IsEqualTo(new Color(0,0,0));
            
            c = map.GetColor(0.1);
            Check.That(c).IsEqualTo(new Color(0.2,0.2,0.2));

            c = map.GetColor(0.3);
            Check.That(c).IsEqualTo(new Color(0.6,0.6,0.6));
            
            c = map.GetColor(0.5);
            Check.That(c).IsEqualTo(new Color(1,1,1));
            
            c = map.GetColor(0.65);
            Check.That(c).IsEqualTo(new Color(1,0.4,0.4));
            
            c = map.GetColor(0.75);
            Check.That(c).IsEqualTo(new Color(1,0,0));
            
            c = map.GetColor(0.85);
            Check.That(c).IsEqualTo(new Color(0.6,0,0));

            c = map.GetColor(1);
            Check.That(c).IsEqualTo(new Color(0,0,0));
        }
    }
}