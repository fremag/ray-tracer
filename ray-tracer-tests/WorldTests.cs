using System;
using System.Linq;
using NFluent;
using ray_tracer.Lights;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests
{
    public class WorldTests
    {
        [Fact]
        public void EmptyWorldTest()
        {
            var world = new World();
            Check.That(world.Shapes).IsEmpty();
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

            world.Shapes.Add(s1);
            world.Shapes.Add(s2);
            world.Lights.Add(l);
            return world;
        }

        [Fact]
        public void BasicWorldTest()
        {
            var world = GetDefaultWorld();
            Check.That(world.Shapes).ContainsExactly(s1, s2);
            Check.That(world.Lights).ContainsExactly(l);
        }

        [Fact]
        public void WorldIntersectsTest()
        {
            var world = GetDefaultWorld();
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var intersections =  new Intersections();
            world.Intersect(ray, intersections);
            Check.That(intersections.Select(intersection => intersection.T)).ContainsExactly(4, 4.5, 5.5, 6);
        }

        [Fact]
        public void ShadeHit_NotInside_Test()
        {
            var world = GetDefaultWorld();
            var ray = new Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = world.Shapes.FirstOrDefault();

            var intersection = Helper.Intersection(4, sphere);
            var intersectionData = intersection.Compute(ray);
            var c = world.ShadeHit(intersectionData);
            Check.That(c.Red).IsCloseTo(0.38066, 1e-5);
            Check.That(c.Green).IsCloseTo(0.47582, 1e-5);
            Check.That(c.Blue).IsCloseTo(0.28549, 1e-5);
        }

        [Fact]
        public void ShadeHit_Inside_Test()
        {
            var world = GetDefaultWorld();
            world.Lights.Clear();
            world.Lights.Add(new PointLight(Helper.CreatePoint(0, 0.25, 0), Color.White));

            var ray = new Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var sphere = world.Shapes[1];

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
            Check.That(color.Green).IsCloseTo(0.47582, 1e-5);
            Check.That(color.Blue).IsCloseTo(0.28549, 1e-5);
        }

        [Fact]
        public void ShadeHit_IntersectionBehindRay_Test()
        {
            var world = GetDefaultWorld();
            var outter = world.Shapes[0];
            outter.Material.Ambient = 1;
            var inner = world.Shapes[1];
            inner.Material.Ambient = 1;

            var ray = new Ray(Helper.CreatePoint(0, 0, 0.75), Helper.CreateVector(0, 0, -1));
            var color = world.ColorAt(ray);
            Check.That(color).IsEqualTo(new Color(1));
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
            w.Shapes.Add(s1);
            var s2 = new Sphere();
            s2.Transform = Helper.Translation(0, 0, 10);
            w.Shapes.Add(s2);

            var r = Helper.Ray(Helper.CreatePoint(0, 0, 5), Helper.CreateVector(0, 0, 1));
            var i = new Intersection(4, s2);
            var intersectionData = i.Compute(r);
            var c = w.ShadeHit(intersectionData);
            Check.That(c).IsEqualTo(new Color(0.1, 0.1, 0.1));
        }

        [Fact]
        public void ReflectedColor_NonReflectiveMaterialTest()
        {
            var w = GetDefaultWorld();
            var r = Helper.Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var shape = w.Shapes[1];
            shape.Material.Ambient = 1;
            var i = Helper.Intersection(1, shape);
            var comps = i.Compute(r);
            var color = w.ReflectedColor(comps);
            Check.That(color).IsEqualTo(Color.Black);
        }

        [Fact]
        public void ReflectedColor_ReflectiveMaterialTest()
        {
            var w = GetDefaultWorld();
            var plane = new Plane();
            plane.Material.Reflective = 0.5;
            plane.Transform = Helper.Translation(0, -1, 0);
            w.Shapes.Add(plane);

            var r = Helper.Ray(Helper.CreatePoint(0, 0, -3), Helper.CreateVector(0, -Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
            var i = Helper.Intersection(Math.Sqrt(2), plane);
            var comps = i.Compute(r);
            var color = w.ReflectedColor(comps);
            Check.That(color.Red).IsCloseTo(0.19032, 1e-4);
            Check.That(color.Green).IsCloseTo(0.2379, 1e-4);
            Check.That(color.Blue).IsCloseTo(0.14274, 1e-4);
        }

        [Fact]
        public void ShadeHit_ReflectiveMaterialTest()
        {
            var w = GetDefaultWorld();
            var plane = new Plane();
            plane.Material.Reflective = 0.5;
            plane.Transform = Helper.Translation(0, -1, 0);
            w.Shapes.Add(plane);

            var r = Helper.Ray(Helper.CreatePoint(0, 0, -3), Helper.CreateVector(0, -Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
            var i = Helper.Intersection(Math.Sqrt(2), plane);
            var comps = i.Compute(r);
            var color = w.ShadeHit(comps);
            Check.That(color.Red).IsCloseTo(0.87677, 1e-4);
            Check.That(color.Green).IsCloseTo(0.92436, 1e-4);
            Check.That(color.Blue).IsCloseTo(0.82918, 1e-4);
        }

        [Fact]
        public void ColorAtWithMutuallyReflectiveSurfacesTest()
        {
            var w = new World();
            w.Lights.Add(new PointLight(Helper.CreatePoint(0, 0, 0), Color.White));
            var lower = new Plane {Material = {Reflective = 1}, Transform = Helper.Translation(0, -1, 0)};
            var upper = new Plane {Material = {Reflective = 1}, Transform = Helper.Translation(0, 1, 0)};
            w.Shapes.Add(lower);
            w.Shapes.Add(upper);

            var r = Helper.Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 1, 0));
            var c = w.ColorAt(r);
            Check.That(c).IsNotNull();
        }

        [Fact]
        public void RefractedColorWithAnOpaqueSurfaceTest()
        {
            var w = GetDefaultWorld();
            var shape = w.Shapes[0];
            var r = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var xs = new Intersections
            {
                new Intersection(4, shape),
                new Intersection(6, shape)
            };
            var comps = xs[0].Compute(r, xs);
            var c = w.RefractedColor(comps, 5);
            Check.That(c).IsEqualTo(Color.Black);
        }

        [Fact]
        public void RefractedColorAtTheMaximumRecursiveDepthTest()
        {
            var w = GetDefaultWorld();
            var shape = w.Shapes[0];
            shape.Material.Transparency = 1.0;
            shape.Material.RefractiveIndex = 1.5;
            var r = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var xs = new Intersections
            {
                new Intersection(4, shape),
                new Intersection(6, shape)
            };
            var comps = xs[0].Compute(r, xs);
            var c = w.RefractedColor(comps, 0);
            Check.That(c).IsEqualTo(Color.Black);
        }

        [Fact]
        public void RefractedColorUnderTotalInternalReflectionTest()
        {
            var w = GetDefaultWorld();
            var shape = w.Shapes[0];
            shape.Material.Transparency = 1.0;
            shape.Material.RefractiveIndex = 1.5;
            double sqrt2 = Math.Sqrt(2);
            var r = Helper.Ray(Helper.CreatePoint(0, 0, sqrt2 / 2), Helper.CreateVector(0, 1, 0));
            var xs = new Intersections
            {
                new Intersection(-sqrt2 / 2, shape),
                new Intersection(sqrt2 / 2, shape)
            };
            var comps = xs[1].Compute(r, xs);
            var c = w.RefractedColor(comps, 0);
            Check.That(c).IsEqualTo(Color.Black);
        }

        [Fact]
        public void TheRefractedColorWithARefractedRayTest()
        {
            var w = GetDefaultWorld();
            var a = w.Shapes[0];
            a.Material.Ambient = 1.0;
            a.Material.Pattern = new TestPattern();
            var b = w.Shapes[1];
            b.Material.Transparency = 1.0;
            b.Material.RefractiveIndex = 1.5;
            var r = Helper.Ray(Helper.CreatePoint(0, 0, 0.1), Helper.CreateVector(0, 1, 0));
            var xs = new Intersections
            {
                new Intersection(-0.9899, a),
                new Intersection(-0.4899, b),
                new Intersection(0.4899, b),
                new Intersection(0.9899, a)
            };
            var comps = xs[2].Compute(r, xs);
            var c = w.RefractedColor(comps);
            Check.That(c.Red).IsCloseTo(0, 1e-4);
            Check.That(c.Green).IsCloseTo(0.99888, 1e-4);
            Check.That(c.Blue).IsCloseTo(0.04725, 1e-4);
        }

        [Fact]
        public void ShadeHitWithATransparentMaterial()
        {
            var w = GetDefaultWorld();
            var floor = new Plane
            {
                Transform = Helper.Translation(0, -1, 0)
            };
            floor.Material.Transparency = 0.5;
            floor.Material.RefractiveIndex = 1.5;
            w.Shapes.Add(floor);

            var ball = Helper.Sphere();
            ball.Material.Pattern = new SolidPattern(new Color(1, 0, 0));
            ball.Material.Ambient = 0.5;
            ball.Transform = Helper.Translation(0, -3.5, -0.5);
            w.Shapes.Add(ball);
            var sqrt2 = Math.Sqrt(2);
            var r = Helper.Ray(Helper.CreatePoint(0, 0, -3), Helper.CreateVector(0, -sqrt2 / 2, sqrt2 / 2));
            
            var xs = new Intersections {new Intersection(sqrt2, floor)};
            var comps = xs[0].Compute(r, xs);
            var color = w.ShadeHit(comps);
            Check.That(color.Red).IsCloseTo(0.93642, 1e-5);
            Check.That(color.Green).IsCloseTo(0.68642, 1e-5);
            Check.That(color.Blue).IsCloseTo(0.68642, 1e-5);
        }
        
        [Fact]
        public void ShadeHitWithATransparentAndReflectiveMaterial()
        {
            var w = GetDefaultWorld();
            var floor = new Plane
            {
                Transform = Helper.Translation(0, -1, 0)
            };
            floor.Material.Reflective = 0.5;
            floor.Material.Transparency = 0.5;
            floor.Material.RefractiveIndex = 1.5;
            w.Shapes.Add(floor);

            var ball = Helper.Sphere();
            ball.Material.Pattern = new SolidPattern(new Color(1, 0, 0));
            ball.Material.Ambient = 0.5;
            ball.Transform = Helper.Translation(0, -3.5, -0.5);
            w.Shapes.Add(ball);
            var sqrt2 = Math.Sqrt(2);
            var r = Helper.Ray(Helper.CreatePoint(0, 0, -3), Helper.CreateVector(0, -sqrt2 / 2, sqrt2 / 2));
            
            var xs = new Intersections {new Intersection(sqrt2, floor)};
            var comps = xs[0].Compute(r, xs);
            var color = w.ShadeHit(comps);
            Check.That(color.Red).IsCloseTo(0.93391, 1e-5);
            Check.That(color.Green).IsCloseTo(0.69643, 1e-5);
            Check.That(color.Blue).IsCloseTo(0.69243, 1e-5);
        }
    }
}