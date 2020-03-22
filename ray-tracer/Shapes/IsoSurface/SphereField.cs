using System;

namespace ray_tracer.Shapes.IsoSurface
{
    public class SphereField : IScalarField
    {
        public double F(double x, double y, double z) => Math.Sqrt(x * x + y * y + z * z);
    }
}