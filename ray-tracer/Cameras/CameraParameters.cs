using System;

namespace ray_tracer.Cameras
{
    public class CameraParameters : AbstractCameraParameters
    {
        public override ICamera BuildCamera()
        {
            var point = Helper.CreatePoint(CameraX, CameraY, CameraZ);
            var look = Helper.CreatePoint(LookX, LookY, LookZ);

            var viewTransform = Helper.ViewTransform(point, look, Helper.CreateVector(0, 1, 0));
            var camera = new Camera(Width, Height, Math.PI / 3, viewTransform);
            return camera;
        }
    }
}