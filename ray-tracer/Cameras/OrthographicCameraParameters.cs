namespace ray_tracer.Cameras
{
    public class OrthographicCameraParameters : AbstractCameraParameters
    {
        public Tuple Up { get; }
        public Tuple Right { get; }

        public OrthographicCameraParameters(double upX, double upY, double upZ, double rightX, double rightY, double rightZ)
        {
            Up = Helper.CreateVector(upX, upY, upZ);
            Right = Helper.CreateVector(rightX, rightY, rightZ);
        }

        public override ICamera BuildCamera()
        {
            return new OrthographicCamera(Width, Height, Position, LookAt, Up, Right);
        }
    }
}