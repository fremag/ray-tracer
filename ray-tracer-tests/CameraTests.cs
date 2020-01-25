using System;
using System.IO;
using NFluent;
using ray_tracer.Cameras;
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
    }
}