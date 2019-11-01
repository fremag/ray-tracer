namespace ray_tracer.Shapes.Mesh
{
    public class Path3DAdapter : AbstractPath3D
    {
        private Path3D Func { get; }
        public Path3DAdapter(Path3D func)
        {
            Func = func;
        }

        public override void GetPoint(double t, out double x, out double y, out double z)
        {
            Func(t, out x, out y, out z);
        }
    }
}