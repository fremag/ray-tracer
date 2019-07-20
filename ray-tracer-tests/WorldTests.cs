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
        PointLight l =new PointLight(Helper.CreatePoint(-10, 10, -10), Color.White);

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
            Check.That(c.Red).IsCloseTo(0.38066, 1e-5);
            Check.That(c.Green).IsCloseTo(0.47583, 1e-5);
            Check.That(c.Blue).IsCloseTo(0.2855, 1e-5);
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
            Check.That(c.Red).IsCloseTo(0.90498, 1e-5);
            Check.That(c.Green).IsCloseTo(0.90498, 1e-5);
            Check.That(c.Blue).IsCloseTo(0.90498, 1e-5);
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
            Check.That(color.Red).IsCloseTo(0.38066, 1e-5);
            Check.That(color.Green).IsCloseTo(0.47583, 1e-5);
            Check.That(color.Blue).IsCloseTo(0.2855, 1e-5);
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
    }
}