using System;
using System.IO;
using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class CameraTests
    {
        [Fact]
        public void Camera_BasicTest()
        {
            var cam = new Camera(160, 120, Math.PI / 2);
            Check.That(cam.HSize).IsEqualTo(160);
            Check.That(cam.VSize).IsEqualTo(120);
            Check.That(cam.FieldOfView).IsEqualTo(Math.PI / 2);
            Check.That(cam.Transform).IsEqualTo(Helper.CreateIdentity());
        }

        [Fact]
        public void Camera_HorizontalCanvasTest()
        {
            var cam = new Camera(200, 125, Math.PI / 2);
            Check.That(cam.PixelSize).IsCloseTo(0.01, 1e-6);
        }

        [Fact]
        public void Camera_VerticalCanvasTest()
        {
            var cam = new Camera(125, 200, Math.PI / 2);
            Check.That(cam.PixelSize).IsCloseTo(0.01, 1e-6);
        }

        [Fact]
        public void ConstructingRayThroughTheCenterOfTheCanvas()
        {
            var cam = new Camera(201, 101, Math.PI / 2);
            var ray = cam.RayForPixel(100, 50);
            Check.That(ray.Origin).IsEqualTo(Helper.CreatePoint(0, 0, 0));
            Check.That(ray.Direction).IsEqualTo(Helper.CreateVector(0, 0, -1));
        }

        [Fact]
        public void ConstructingRayThroughACornerOfTheCanvas()
        {
            var cam = new Camera(201, 101, Math.PI / 2);
            var ray = cam.RayForPixel(0, 0);
            Check.That(ray.Origin).IsEqualTo(Helper.CreatePoint(0, 0, 0));
            Check.That(ray.Direction).IsEqualTo(Helper.CreateVector(0.66519, 0.33259, -0.66851));
        }

        [Fact]
        public void ConstructingRayWhenCameraIsTransformed()
        {
            var cam = new Camera(201, 101, Math.PI / 2, Helper.RotationY(Math.PI / 4) * Helper.Translation(0, -2, 5));
            var ray = cam.RayForPixel(100, 50);
            Check.That(ray.Origin).IsEqualTo(Helper.CreatePoint(0, 2, -5));
            Check.That(ray.Direction).IsEqualTo(Helper.CreateVector(Math.Sqrt(2) / 2, 0, -Math.Sqrt(2) / 2));
        }

        [Fact]
        public void RenderWorldTest()
        {
            var floor = Helper.Sphere();
            floor.Transform = Helper.Scaling(10, 0.01, 10);
            floor.Material = new Material(new Color(1, 0.9, 0.9), specular: 0);

            var leftWall = Helper.Sphere();
            leftWall.Transform = Helper.Translation(0, 0, 5) * Helper.RotationY(-Math.PI / 4) *
                                 Helper.RotationX(Math.PI / 2) * Helper.Scaling(10, 0.01, 10);
            leftWall.Material = floor.Material;

            var rightWall = Helper.Sphere();
            rightWall.Transform = Helper.Translation(0, 0, 5) * Helper.RotationY(Math.PI / 4) * Helper.RotationX(Math.PI / 2) * Helper.Scaling(10, 0.01, 10);
            rightWall.Material = floor.Material;

            var middle = Helper.Sphere();
            middle.Transform = Helper.Translation(-0.5, 1, 0.5);
            middle.Material = new Material(new Color(0.1, 1, 0.5), diffuse: 0.7, specular: 0.3);

            var right = Helper.Sphere();
            right.Transform = Helper.Translation(1.5, 0.5, -0.5) * Helper.Scaling(0.5, 0.5, 0.5);
            right.Material = new Material(new Color(0.5, 1, 0.1), diffuse: 0.7, specular: 0.3);

            var left = Helper.Sphere();
            left.Transform = Helper.Translation(-1.5, 0.33, -0.75) * Helper.Scaling(0.33, 0.33, 0.33);
            left.Material = new Material(new Color(1, 0.8, 0.1), diffuse: 0.7, specular: 0.3);

            var world = new World();
            world.Shapes.AddRange(new [] {floor, leftWall, rightWall, middle, left, right});
            world.Lights.Add(new PointLight(Helper.CreatePoint(-10,10,-10), Color.White));

            var camera = new Camera(600, 400, Math.PI / 3, Helper.ViewTransform(Helper.CreatePoint(0, 1.5, -5), Helper.CreatePoint(0, 1, 0), Helper.CreateVector(0, 1, 0)));
            var canvas = camera.Render(world);
            string file = Path.Combine(Path.GetTempPath(), "helloword.ppm");
            
            Helper.SavePPM(canvas, file);
//            Helper.Display(file);
            File.Delete(file);
        }
    }
}