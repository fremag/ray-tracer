namespace ray_tracer.Shapes.Mesh
{
    public class Curve2DAdapter : AbstractCurve2D
    {
        private Curve2D Func { get; }
        public Curve2DAdapter(Curve2D func)
        {
            Func = func;
        }

        public override void GetPoint(double u, double v, out double x, out double y)
        {
            Func(u, v, out x, out y);
        }
    }
}