namespace ray_tracer.Cameras
{
    public interface ICameraParameters
    {
        int Height { get; }
        int Width { get; }

        double CameraX { get; }
        double CameraY { get; }
        double CameraZ { get; }

        double LookX { get; }
        double LookY { get; }
        double LookZ { get; }

        ICamera BuildCamera();
    }
}