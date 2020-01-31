using System;

namespace ray_tracer.Cameras
{
    public class CameraParameters : AbstractCameraParameters
    {
        public double LookX { get; set; } = 0;
        public double LookY { get; set; } = 0;
        public double LookZ { get; set; } = 0;

        public Tuple LookAt => Helper.CreatePoint(LookX, LookY, LookZ);
        
        public override ICamera BuildCamera()
        {
            var viewTransform = Helper.ViewTransform(Position, LookAt, Helper.CreateVector(0, 1, 0));
            var camera = new Camera(Width, Height, Math.PI / 3, viewTransform);
            return camera;
        }
    }
}