namespace ray_tracer.Shapes.IsoSurface
{
    public static class ScalarField 
    {
        public static IScalarField Translate(this IScalarField field, double x, double y, double z) => new TranslateField(x, y, z, field);    
        public static IScalarField Scale(this IScalarField field, double x, double y, double z) => new ScaleField(x, y, z, field);    
        public static IScalarField InvSqr(this IScalarField field) => new InvSqrField(field);    
    }
}