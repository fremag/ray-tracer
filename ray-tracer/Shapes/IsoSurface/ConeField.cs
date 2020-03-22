using System;

namespace ray_tracer.Shapes.IsoSurface
{
    public class ConeField : IScalarField
    {
        public double F(double x, double y, double z) => Math.Max(Math.Abs(y), y + Math.Sqrt(x * x + z * z));
    }
}