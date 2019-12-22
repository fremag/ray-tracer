using System;
using System.IO;
using System.Linq;
using NFluent;
using ray_tracer.Lights;
using ray_tracer.Patterns;
using Xunit;

namespace ray_tracer.tests.Shapes
{
    public class SphereTests
    {
        [Fact]
        public void IntersectTwoPointsTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs =  new Intersections();
            sphere.Intersect(ref ray.Origin, ref ray.Direction, xs);
            Check.That(xs.Select(i => i.T)).ContainsExactly(4, 6);
        }
        
        [Fact]
        public void IntersectTangentTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 1, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs =  new Intersections();
            sphere.Intersect(ref ray.Origin, ref ray.Direction, xs);
            Check.That(xs.Select(i => i.T)).ContainsExactly(5, 5);
        }
        
        [Fact]
        public void IntersectMissSphereTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 2, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = new Intersections();
            sphere.Intersect(ref ray.Origin, ref ray.Direction, xs);
            Check.That(xs).IsEmpty();
        }
        
        [Fact]
        public void IntersectInsideSphereTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs =  new Intersections();
            sphere.Intersect(ref ray.Origin, ref ray.Direction, xs);
            Check.That(xs.Select(i => i.T)).ContainsExactly(-1, 1);
            Check.That(xs.Select(i => i.Object)).ContainsExactly(sphere, sphere);
        }

        [Fact]
        public void IntersectSphereBehindRayTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, 5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs =  new Intersections();
            sphere.Intersect(ref ray.Origin, ref ray.Direction, xs);
            Check.That(xs.Select(i => i.T)).ContainsExactly(-6, -4);
        }

        [Fact]
        public void DefaultTransformTest()
        {
            var sphere = Helper.Sphere();
            Check.That(sphere.Transform).IsEqualTo(Helper.CreateIdentity());
        }

        [Fact]
        public void ChangeTransformTest()
        {
            var sphere = Helper.Sphere();
            var translation = Helper.Translation(2, 3, 4);
            sphere.Transform = translation;
            Check.That(sphere.Transform).IsEqualTo(translation);
        }
        
        [Fact]
        public void IntersectScaledTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            sphere.Transform = Helper.Scaling(2, 2, 2);
            
            var xs =  new Intersections();
            sphere.Intersect(ref ray.Origin, ref ray.Direction, xs);
            Check.That(xs.Select(i => i.T)).ContainsExactly(3, 7);
        }
        
        [Fact]
        public void IntersectTranslatedTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            sphere.Transform = Helper.Translation(5, 0, 0);
            
            var xs =  new Intersections();
            sphere.Intersect(ref ray.Origin, ref ray.Direction, xs);
            Check.That(xs).IsEmpty();
        }

        private void SpherePictureTest(Matrix transform)
        {
            int size = 400;
            var rayOrigin = Helper.CreatePoint(0, 0, -5);
            double wallZ = 10;
            double wallSize = 7;
            var pixelSize = wallSize / size;
            var half = wallSize / 2;

            var canvas = new Canvas(size, size);
            var color = new Color(1, 0, 0);
            var sphere = Helper.Sphere();
            sphere.Transform = transform;

            for (int y = 0; y < size; y++)
            {
                double worldY = half - pixelSize * y;
                for (int x = 0; x < size - 1; x++)
                {
                    // compute the world x coordinate (left = -half, right = half)
                    double worldX = -half + pixelSize * x;
                    // describe the point on the wall that the ray will target
                    var position = Helper.CreatePoint(worldX, worldY, wallZ);
                    var r = Helper.Ray(rayOrigin, (position - rayOrigin).Normalize());
                    var intersections =  new Intersections();
                    sphere.Intersect(ref r.Origin, ref r.Direction, intersections);
                    if (intersections.Hit() != null)
                    {
                        canvas.SetPixel(x, y, color);
                    }
                }
            }
        
            var tmpFile = Path.GetTempFileName();
            var ppmFile = Path.ChangeExtension(tmpFile, "ppm");
            canvas.SavePPM(ppmFile);
//            Helper.Display(ppmFile);
            File.Delete(tmpFile);
            File.Delete(ppmFile);
        }

        [Fact]
        public void NoTransformSphereTest()
        {
            SpherePictureTest(Helper.CreateIdentity());
        }
        
        [Fact]
        public void ScaledSphereTest()
        {
            SpherePictureTest(Helper.Scaling(1, 0.5, 1));
        }

        [Fact]
        public void ShearedScaledSphereTest()
        {
            var transform = Helper.Shearing(1, 0, 0, 0, 0, 0) * Helper.Scaling(0.5, 1, 1);
            SpherePictureTest(transform);
        }
        [Fact]
        public void ScaledShearedSphereTest()
        {
            var transform = Helper.Scaling(0.5, 1, 1) * Helper.Shearing(1, 0, 0, 0, 0, 0);
            SpherePictureTest(transform);
        }

        [Theory]
        [InlineData(1,0,0)]
        [InlineData(0,1,0)]
        [InlineData(0,0,1)]
        public void NormalAtPointOnAxisTest(double x, double y, double z)
        {
            var sphere = Helper.Sphere();
            var n = sphere.NormalAt(Helper.CreatePoint(x, y, z));
            Check.That(n).IsEqualTo(Helper.CreateVector(x,y,z));
        }

        [Fact]
        public void NormalAtPointNotOnAxisTest()
        {
            double x = Math.Sqrt(3)/3;
            double y = Math.Sqrt(3)/3;
            double z = Math.Sqrt(3)/3;
                
            var sphere = Helper.Sphere();
            var n = sphere.NormalAt(Helper.CreatePoint(x, y, z));
            Check.That(n).IsEqualTo(Helper.CreateVector(x,y,z));
        }

        [Fact]
        public void NormalAtPointIsNormalizedTest()
        {
            double x = Math.Sqrt(3)/3;
            double y = Math.Sqrt(3)/3;
            double z = Math.Sqrt(3)/3;
                
            var sphere = Helper.Sphere();
            var n = sphere.NormalAt(Helper.CreatePoint(x, y, z));
            Check.That(n.Normalize()).IsEqualTo(n);
        }

        [Fact]
        public void NormalAtPointOnTranslatedSphereTest()
        {
            var sphere = Helper.Sphere();
            sphere.Transform = Helper.Translation(0, 1, 0);
            
            var n = sphere.NormalAt(Helper.CreatePoint(0, 1.70711, -0.70711));
            Check.That(n).IsEqualTo(Helper.CreateVector(0, 0.70711, -0.70711));
        }

        [Fact]
        public void NormalAtPointOnTransformedSphereTest()
        {
            var sphere = Helper.Sphere();
            sphere.Transform = Helper.Scaling(1, 0.5, 1) * Helper.RotationZ(Math.PI/5);
            
            var n = sphere.NormalAt(Helper.CreatePoint(0, Math.Sqrt(2)/2, -Math.Sqrt(2)/2));
            Check.That(n).IsEqualTo(Helper.CreateVector(0, 0.97014, -0.24254));
        }

        [Fact]
        public void DefaultMaterialTest()
        {
            var sphere = Helper.Sphere();
            Check.That(sphere.Material).IsEqualTo(new Material());
        }
        

        [Fact]
        public void LightingSpherePictureTest()
        {
            int size = 400;
            var rayOrigin = Helper.CreatePoint(0, 0, -5);
            double wallZ = 10;
            double wallSize = 7;
            var pixelSize = wallSize / size;
            var half = wallSize / 2;

            var canvas = new Canvas(size, size);
            var sphere = Helper.Sphere();
            sphere.Material.Pattern = new SolidPattern(new Color(1, 0.2, 1));
            
            var light = new PointLight(Helper.CreatePoint(-10, 10, -10), Color.White);
            
            for (int y = 0; y < size; y++)
            {
                double worldY = half - pixelSize * y;
                for (int x = 0; x < size - 1; x++)
                {
                    // compute the world x coordinate (left = -half, right = half)
                    double worldX = -half + pixelSize * x;
                    // describe the point on the wall that the ray will target
                    var position = Helper.CreatePoint(worldX, worldY, wallZ);
                    var ray = Helper.Ray(rayOrigin, (position - rayOrigin).Normalize());
                    var intersections =  new Intersections();
                    sphere.Intersect(ref ray.Origin, ref ray.Direction, intersections);
                    var hit = intersections.Hit();
                    
                    if (hit != null)
                    {
                        var point = ray.Position(hit.T);
                        var hitObject = hit.Object;
                        var normal = hitObject.NormalAt(point);
                        var eye = -ray.Direction;
                        var color = hitObject.Material.Lighting(light, ref point, ref eye, ref normal, 1);

                        canvas.SetPixel(x, y, color);
                    }
                }
            }
        
            var tmpFile = Path.GetTempFileName();
            var ppmFile = Path.ChangeExtension(tmpFile, "ppm");
            canvas.SavePPM(ppmFile);

//            Helper.Display(ppmFile);
            File.Delete(tmpFile);
            File.Delete(ppmFile);
        }
    }
}