using System;

namespace ray_tracer.Cameras
{
    public abstract class AbstractCameraParameters : ICameraParameters
    {
        public string Name { get; set; }
        public double CameraX { get; set; } = 0;
        public double CameraY { get; set; } = 1;
        public double CameraZ { get; set; } = -1;

        public int Height { get; set; } = 400;
        public int Width { get; set; } = 600;

        public Tuple Position => Helper.CreatePoint(CameraX, CameraY, CameraZ);

        public abstract ICamera BuildCamera();
    }
}