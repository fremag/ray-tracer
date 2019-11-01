namespace ray_tracer.Shapes.Mesh
{
    public class TriangleMeshFactory : IMeshFactory
    {
        public bool LoopU { get; }
        public bool LoopV { get; }

        public TriangleMeshFactory(bool loopU=true, bool loopV=true)
        {
            LoopU = loopU;
            LoopV = loopV;
        }

        public Group Build(IMesh mesh)
        {
            var group = new Group();
            var m = mesh.M;
            var n = mesh.N;
            if (!LoopU)
            {
               m--;
            }

            if (!LoopV)
            {
                n--;
            }
            
            for (int i = 0; i < n; i++)
            {
                var g = new Group();
                for (int j = 0; j < m; j++)
                {
                    var p0 = mesh.Points[i][j];
                    var nextI = (i + 1) % mesh.N;
                    var nextJ = (j + 1) % mesh.M;
                    var p1 = mesh.Points[nextI][nextJ];
                    var tri1 = new Triangle(p0, p1, mesh.Points[i][nextJ]);
                    var tri2 = new Triangle(p0, p1, mesh.Points[nextI][j]);
                    g.Add(tri1);
                    g.Add(tri2);
                }

                group.Add(g);
            }

            return group;
        }
    }
}