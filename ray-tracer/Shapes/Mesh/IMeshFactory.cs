namespace ray_tracer.Shapes.Mesh
{
    public interface IMeshFactory
    {
        Group Build(IMesh mesh);
    }
}