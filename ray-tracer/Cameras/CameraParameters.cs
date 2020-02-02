using System;

namespace ray_tracer.Cameras
{
    public class CameraParameters : AbstractCameraParameters
    {
        public double FieldOfView { get; set; } = Math.PI / 3;
        public override ICamera BuildCamera()
        {
            var viewTransform = Helper.ViewTransform(Position, LookAt, Helper.CreateVector(0, 1, 0));
            var camera = new Camera(Width, Height, FieldOfView, viewTransform);
            return camera;
        }
    }
}