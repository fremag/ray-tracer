namespace ray_tracer.Shapes.Mesh
{
    public class SphereMeshFactory : IMeshFactory
    {
        public double R { get; }

        public SphereMeshFactory(double r=0.1)
        {
            R = r;
        }

        public Group Build(IMesh mesh)
        {
            var group = new Group();
            
            for (int i = 0; i < mesh.N; i++)
            {
                var g = new Group();
                for (int j = 0; j < mesh.M; j++)
                {
                    var s = new Sphere();
                    s.Scale(R);
                    s.Translate(mesh.Points[i][j]);
                    g.Add(s);
                }

                group.Add(g);
            }

            return group;
        }
    }
}