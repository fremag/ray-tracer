namespace ray_tracer.Cameras
{
    public abstract class AbstractCamera : ICamera
    {
        public abstract Ray RayForPixel(int px, int py);
        
        public int HSize { get; }
        public int VSize { get; }
        public Matrix Transform { get; }

        protected AbstractCamera(int hSize, int vSize, Matrix transform)
        {
            HSize = hSize;
            VSize = vSize;
            Transform = transform;
        }
    }
}