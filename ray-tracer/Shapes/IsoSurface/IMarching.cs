namespace ray_tracer.Shapes.IsoSurface
{
    public interface IMarching
    {
        void Generate(IsoSurface isoSurface, TriangleMesh mesh);
    }
}