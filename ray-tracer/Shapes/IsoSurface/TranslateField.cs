namespace ray_tracer.Shapes.IsoSurface
{
    public class TranslateField : IScalarField
    {
        private readonly double x0;
        private readonly double y0;
        private readonly double z0;
        private readonly IScalarField field;

        public TranslateField(double x0, double y0, double z0, IScalarField field)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.z0 = z0;
            this.field = field;
        }

        public double F(double x, double y, double z) => field.F(x - x0, y - y0, z - z0);
    }
}    
