using System.Collections.Generic;
using System.Linq;

namespace ray_tracer.Shapes.IsoSurface
{
    public interface IScalarField
    {
        double F(double x, double y, double z);
    }

    public class SumField : IScalarField
    {
        public  List<IScalarField> Fields { get; }
        public SumField(params IScalarField[] fields)
        {
            Fields = fields.ToList();
        }

        public double F(double x, double y, double z) => Fields.Sum(field => field.F(x, y, z));
    }
    
    public class InvSqrSumField : IScalarField
    {
        public  List<IScalarField> Fields { get; }
        public InvSqrSumField(params IScalarField[] fields)
        {
            Fields = fields.ToList();
        }

        public double F(double x, double y, double z) => Fields.Sum(field =>
        {
            var v = field.F(x, y, z);
            return 1/(v*v);
        });
    }
    
}