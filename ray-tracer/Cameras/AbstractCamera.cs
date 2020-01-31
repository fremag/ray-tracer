namespace ray_tracer.Cameras
{
    public abstract class AbstractCamera : ICamera
    {
        public abstract Ray RayForPixel(int px, int py);
        
        public int HSize { get; }
        public int VSize { get; }

        protected AbstractCamera(int hSize, int vSize)
        {
            HSize = hSize;
            VSize = vSize;
        }
    }
}