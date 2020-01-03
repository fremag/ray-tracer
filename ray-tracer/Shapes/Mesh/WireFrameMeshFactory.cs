namespace ray_tracer.Shapes.Mesh
{
    public class WireFrameMeshFactory : IMeshFactory
    {
        public double Radius { get; }
        public bool CloseU { get; }
        public bool CloseV { get; }

        public WireFrameMeshFactory(double radius = 0.02, bool closeU = false, bool closeV = false)
        {
            Radius = radius;
            CloseU = closeU;
            CloseV = closeV;
        }

        public Group Build(IMesh mesh)
        {
            var group = new Group();

            for (int i = 0; i < mesh.N; i++)
            {
                var g = new Group();
                for (int j = 0; j < mesh.M-1; j++)
                {
                    var c1 = Helper.Cylinder(mesh.Points[i][j], mesh.Points[i][j+1], Radius);
                    g.Add(c1);
                }

                if (CloseV)
                {
                    var c1 = Helper.Cylinder(mesh.Points[i][0], mesh.Points[i][mesh.M-1], Radius);
                    g.Add(c1);
                }
                group.Add(g);
            }

            for (int i = 0; i < mesh.M; i++)
            {
                var g = new Group();
                for (int j = 0; j < mesh.N-1; j++)
                {
                    var c1 = Helper.Cylinder(mesh.Points[j][i], mesh.Points[j+1][i], Radius);
                    g.Add(c1);
                }

                if (CloseU)
                {
                    var c1 = Helper.Cylinder(mesh.Points[0][i], mesh.Points[mesh.N-1][i], Radius);
                    g.Add(c1);
                }
                group.Add(g);
            }

            return group;
        }
    }
}