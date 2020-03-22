
namespace ray_tracer.Shapes.IsoSurface
{
    public class ScaleField : IScalarField
    {
        private readonly double sx;
        private readonly double sy;
        private readonly double sz;
        private readonly IScalarField field;

        public ScaleField(double sx, double sy, double sz, IScalarField field)
        {
            this.sx = sx;
            this.sy = sy;
            this.sz = sz;
            this.field = field;
        }

        public double F(double x, double y, double z) => field.F(x/sx, y/sy, z/sz);
    }
}    
