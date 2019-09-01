using System;
using NFluent;
using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests
{
    public class MaterialTests
    {
        Material material = new Material();
        Tuple position = Helper.CreatePoint(0, 0, 0);

        [Fact]
        public void DefaultMaterialTest()
        {
            Material m = new Material();
            Check.That(m.Pattern).IsEqualTo(new SolidPattern(new Color(1, 1, 1)));
            Check.That(m.Ambient).IsEqualTo(0.1);
            Check.That(m.Diffuse).IsEqualTo(0.9);
            Check.That(m.Specular).IsEqualTo(0.9);
            Check.That(m.Shininess).IsEqualTo(200);
        }

        [Fact]
        public void MaterialTest()
        {
            var c = new Color(1, 1, 1);
            Material m = new Material(c, 1, 2, 3, 4);
            Check.That(m.Pattern).IsEqualTo(new SolidPattern(new Color(1, 1, 1)));
            Check.That(m.Ambient).IsEqualTo(1);
            Check.That(m.Diffuse).IsEqualTo(2);
            Check.That(m.Specular).IsEqualTo(3);
            Check.That(m.Shininess).IsEqualTo(4);
        }

        [Fact]
        public void LightingWithTheEyeBetweenTheLightAndTheSurfaceTest()
        {
            var eyev = Helper.CreateVector(0, 0, -1);
            var normalv = Helper.CreateVector(0, 0, -1);
            var light = new PointLight(Helper.CreatePoint(0, 0, -10), new Color(1, 1, 1));
            var inShadow = false;
            var result = material.Lighting(light, position, eyev, normalv, inShadow);
            Check.That(result).IsEqualTo(new Color(1.9, 1.9, 1.9));
        }
        
        [Fact]
        public void  LightingWithTheEyeBetweenLightAndSurfaceEyeOffset45DegreesTest()
        {
            var eyev = Helper.CreateVector(0, Math.Sqrt(2)/2, -Math.Sqrt(2)/2);
            var normalv = Helper.CreateVector(0, 0, -1);
            var light = new PointLight(Helper.CreatePoint(0, 0, -10), new Color(1, 1, 1));
            var inShadow = false;
            var result = material.Lighting(light, position, eyev, normalv, inShadow);
            Check.That(result).IsEqualTo(new Color(1.0, 1.0, 1.0));
        }

        [Fact]
        public void  LightingWithEyeOppositeSurfaceLightOffset45DegreesTest()
        {
            var eyev = Helper.CreateVector(0, 0, -1);
            var normalv = Helper.CreateVector(0, 0, -1);
            var light = new PointLight(Helper.CreatePoint(0, 10, -10), new Color(1, 1, 1));
            var inShadow = false;
            var result = material.Lighting(light, position, eyev, normalv, inShadow);
            var c = 0.1+0.9*Math.Sqrt(2)/2;
            Check.That(result.Red).IsCloseTo(c, 1e-5);
            Check.That(result.Green).IsCloseTo(c, 1e-5);
            Check.That(result.Blue).IsCloseTo(c, 1e-5);
        }

        [Fact]
        public void LightingWithEyeInThePathOfTheReflectionVectorTest()
        {
            var eyev = Helper.CreateVector(0, -Math.Sqrt(2)/2, -Math.Sqrt(2)/2);
            var normalv = Helper.CreateVector(0, 0, -1);
            var light = new PointLight(Helper.CreatePoint(0, 10, -10), new Color(1, 1, 1));
            var inShadow = false;
            var result = material.Lighting(light, position, eyev, normalv, inShadow);
            var c = 0.1+0.9*Math.Sqrt(2)/2 + 0.9;
            Check.That(result).IsEqualTo(new Color(c, c, c));
        }

        [Fact]
        public void LightingWithTheLightBehindTheSurfaceTest()
        {
            var eyev = Helper.CreateVector(0, 0, -1);
            var normalv = Helper.CreateVector(0, 0, -1);
            var light = new PointLight(Helper.CreatePoint(0, 0, 10), new Color(1, 1, 1));
            var inShadow = false;
            var result = material.Lighting(light, position, eyev, normalv, inShadow);
            Check.That(result).IsEqualTo(new Color(0.1, 0.1, 0.1));
        }

        [Fact]
        public void LightingWithTheSurfaceInShadowTest()
        {
            var eyev = Helper.CreateVector(0, 0, -1);
            var normalv = Helper.CreateVector(0, 0, -1);
            var light = new PointLight(Helper.CreatePoint(0, 0, -10), new Color(1, 1, 1));
            var inShadow = true;
            var result = material.Lighting(light, position, eyev, normalv, inShadow);
            Check.That(result).IsEqualTo(new Color(0.1, 0.1, 0.1));
        }

        [Fact]
        public void LightingWithStripePatternApplied()
        {
            material.Pattern = new StripePattern(new Color(1, 1, 1), new Color(0, 0, 0));
            material.Ambient = 1;
            material.Diffuse = 0;
            material.Specular = 0;
            var eyev = Helper.CreateVector(0, 0, -1);
            var normalv = Helper.CreateVector(0, 0, -1);
            var light = new PointLight(Helper.CreatePoint(0, 0, -10), new Color(1, 1, 1));
            var c1 = material.Lighting(light, Helper.CreatePoint(0.9, 0, 0), eyev, normalv, false);
            var c2 = material.Lighting(light, Helper.CreatePoint(1.1, 0, 0), eyev, normalv, false);
            Check.That(c1).IsEqualTo(Color.White);
            Check.That(c2).IsEqualTo(Color.Black);
        }
    }
}