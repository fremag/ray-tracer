namespace ray_tracer.Cameras
{
    public class OrthographicCamera : AbstractCamera
    {
        public Tuple Position { get; }
        public Tuple Direction { get; }
        public Tuple Up { get; }
        public Tuple Right { get; }

        public OrthographicCamera(int hSize, int vSize, Tuple position, Tuple direction, Tuple up, Tuple right) 
            : base(hSize, vSize)
        {
            Position = position;
            Direction = direction.Normalize();
            Up = up;
            Right = right;
        }

        public override Ray RayForPixel(int px, int py)
        {
            var dx = px / (double) VSize;
            var dy = 1-py / (double) HSize;
            var right = dx * Right;
            var up = dy * Up;
            Tuple origin = Position + right + up;
            return new Ray(origin, Direction);
        }
    }
}