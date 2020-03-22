using System;

namespace ray_tracer.Shapes.IsoSurface
{
    public class CylinderField : IScalarField
    {
        public double F(double x, double y, double z)
        {
            var r = Math.Sqrt(x * x + z * z);
            var v = Math.Max(r, Math.Abs(y));
            return v;
        }
    }
}