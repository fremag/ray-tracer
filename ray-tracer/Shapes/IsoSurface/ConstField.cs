namespace ray_tracer.Shapes.IsoSurface
{
    public class ConstField : IScalarField
    {
        private readonly double c;

        public ConstField(double c)
        {
            this.c = c;
        }

        public double F(double x, double y, double z) => c;
    }
}