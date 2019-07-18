using System.IO;
using System.Linq;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class SphereTests
    {
        [Fact]
        public void IntersectTwoPointsTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(4, 6);
        }
        
        [Fact]
        public void IntersectTangentTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 1, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(5, 5);
        }
        
        [Fact]
        public void IntersectMissSphereTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 2, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs).IsEmpty();
        }
        
        [Fact]
        public void IntersectInsideSphereTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, 0), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(-1, 1);
            Check.That(xs.Select(i => i.Object)).ContainsExactly(sphere, sphere);
        }

        [Fact]
        public void IntersectSphereBehindRayTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, 5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            var xs = sphere.Intersect(ray);
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
            
            var xs = sphere.Intersect(ray);
            Check.That(xs.Select(i => i.T)).ContainsExactly(3, 7);
        }
        
        [Fact]
        public void IntersectTranslatedTest()
        {
            var ray = Helper.Ray(Helper.CreatePoint(0, 0, -5), Helper.CreateVector(0, 0, 1));
            var sphere = Helper.Sphere();
            sphere.Transform = Helper.Translation(5, 0, 0);
            
            var xs = sphere.Intersect(ray);
            Check.That(xs).IsEmpty();
        }

        public void SpherePictureTest(Matrix transform)
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
                    var intersections = sphere.Intersect(r);
                    if (intersections.Hit() != null)
                    {
                        canvas.SetPixel(x, y, color);
                    }
                }
            }
        
            var tmpFile = Path.GetTempFileName();
            var ppmFile = Path.ChangeExtension(tmpFile, "ppm");
            canvas.SavePPM(ppmFile);
 
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
    }
}