namespace ray_tracer.Shapes.IsoSurface
{
    public class InvSqrField : IScalarField
    {
        private IScalarField Field { get; }

        public InvSqrField(IScalarField field)
        {
            Field = field;
        }

        public double F(double x, double y, double z)
        {
            var v =  Field.F(x, y, z);
            return 1/( v * v);
        }
    }
}