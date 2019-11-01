namespace ray_tracer.Shapes.Mesh
{
    public interface IMesh
    {
        int N { get; }
        int M { get; }
        Tuple[][] Points { get; }
    }

    public abstract class AbstractMesh : IMesh
    {
        public int N { get; }
        public int M { get; }
        public Tuple[][] Points { get; }

        public AbstractMesh(int n, int m)
        {
            N = n;
            M = m;
            Points = new Tuple[n][];
            for (int i = 0; i < n; i++)
            {
                Points[i] = new Tuple[m];
            }
        }
    }
}