using System.Linq;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class WorldTests
    {
        [Fact]
        public void EmptyWorldTest()
        {
            var world = new World();
            Check.That(world.Spheres).IsEmpty();
            Check.That(world.Lights).IsEmpty();
        }

        Sphere s1 = Helper.Sphere();
        Sphere s2 = Helper.Sphere();
        PointLight l = new PointLight(Helper.CreatePoint(-10, 10, -10), Color.White);

        private World GetDefaultWorld()
        {
            var world = new World();
            s1.Material = new Material(new Color(0.8, 1.0, 0.6), diffuse: 0.7, specular: 0.2);
            s2.Transform = Helper.Scaling(0.5, 0.5, 0.5);

            world.Spheres.Add(s1);
            world.Spheres.Add(s2);
            world.Lights.Add(l);
            return world;
        }

        [Fact]
        public void BasicWorldTest()
        {
            var world = GetDefaultWorld();
            Check.That(world.Spheres).ContainsExactly(s1, s2);
            Check.That(world.Lights).ContainsExactly(l);
        }

        [Fact]
        public void WorldIntersectsTest()
        {
            var world = GetDefaultWorld();
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var intersections = world.Intersect(ray);
            Check.That(intersections.Select(intersection => intersection.T)).ContainsExactly(4, 4.5, 5.5, 6);
        }

        [Fact]
        public void ShadeHit_NotInside_Test()
        {
            var world = GetDefaultWorld();
            var ray = new Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = world.Spheres.FirstOrDefault();

            var intersection = Helper.Intersection(4, sphere);
            var intersectionData = intersection.Compute(ray);
            var c = world.ShadeHit(intersectionData);
            Check.That(c.Red).IsCloseTo(0.08, 1e-5);
            Check.That(c.Green).IsCloseTo(0.1, 1e-5);
            Check.That(c.Blue).IsCloseTo(0.06, 1e-5);
        }

        [Fact]
        public void ShadeHit_Inside_Test()
        {
            var world = GetDefaultWorld();
            world.Lights.Clear();
            world.Lights.Add(new PointLight(Helper.CreatePoint(0, 0.25, 0), Color.White));

            var ray = new Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var sphere = world.Spheres[1];

            var intersection = Helper.Intersection(0.5, sphere);
            var intersectionData = intersection.Compute(ray);
            var c = world.ShadeHit(intersectionData);
            Check.That(c.Red).IsCloseTo(0.1, 1e-5);
            Check.That(c.Green).IsCloseTo(0.1, 1e-5);
            Check.That(c.Blue).IsCloseTo(0.1, 1e-5);
        }

        [Fact]
        public void ShadeHit_NoHit_Test()
        {
            var world = GetDefaultWorld();
            var ray = new Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 1, 0));
            Check.That(world.ColorAt(ray)).IsEqualTo(Color.Black);
        }

        [Fact]
        public void ShadeHit_Hit_Test()
        {
            var world = GetDefaultWorld();
            var ray = new Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var color = world.ColorAt(ray);
            Check.That(color.Red).IsCloseTo(0.08, 1e-5);
            Check.That(color.Green).IsCloseTo(0.1, 1e-5);
            Check.That(color.Blue).IsCloseTo(0.06, 1e-5);
        }

        [Fact]
        public void ShadeHit_IntersectionBehindRay_Test()
        {
            var world = GetDefaultWorld();
            var outter = world.Spheres[0];
            outter.Material.Ambient = 1;
            var inner = world.Spheres[1];
            inner.Material.Ambient = 1;

            var ray = new Ray(Helper.CreatePoint(0, 0, 0.75), Helper.CreateVector(0, 0, -1));
            var color = world.ColorAt(ray);
            Check.That(color).IsEqualTo(inner.Material.Color);
        }

        [Theory]
        [InlineData(0, 10, 0, false)]
        [InlineData(10, -10, 10, true)]
        [InlineData(-20, 20, -20, false)]
        public void IsShadowedTest(double x, double y, double z, bool isShadowed)
        {
            var world = GetDefaultWorld();
            var point = Helper.CreatePoint(x, y, z);
            Check.That(world.IsShadowed(point)).IsEqualTo(isShadowed);
        }

        [Fact]
        public void ShadeHitIsGivenAnIntersectionInShadowTest()
        {
            var w = new World();
            w.Lights.Add(new PointLight(Helper.CreatePoint(0, 0, -10), new Color(1, 1, 1)));
            var s1 = new Sphere();
            w.Spheres.Add(s1);
            var s2 = new Sphere();
            s2.Transform = Helper.Translation(0, 0, 10);
            w.Spheres.Add(s2);

            var r = Helper.Ray(Helper.CreatePoint(0, 0, 5), Helper.CreateVector(0, 0, 1));
            var i = new Intersection(4, s2);
            var intersectionData = i.Compute(r);
            var c = w.ShadeHit(intersectionData);
            Check.That(c).IsEqualTo(new Color(0.1, 0.1, 0.1));
        }
    }
}