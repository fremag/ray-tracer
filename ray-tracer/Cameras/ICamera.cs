namespace ray_tracer.Cameras
{
    public interface ICamera
    {
        Ray RayForPixel(int px, int py);
        int HSize { get; }
        int VSize { get; }
    }
}