using System;

namespace ray_tracer.Shapes.IsoSurface
{
    public class CubeField : IScalarField
    {
        public double F(double x, double y, double z)
        {
            var v = Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z)));
            return v;
        }
    }
}