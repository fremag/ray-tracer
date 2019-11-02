namespace ray_tracer.Shapes.Mesh
{
    public class WireFrameMeshFactory : IMeshFactory
    {
        public double RadiusSphere { get; }
        public double RadiusCylinder { get; }
        public bool CloseU { get; }
        public bool CloseV { get; }

        public WireFrameMeshFactory(double radiusSphere = 0.1, double radiusCylinder = 0.02, bool closeU = false, bool closeV = false)
        {
            RadiusSphere = radiusSphere;
            RadiusCylinder = radiusCylinder;
            CloseU = closeU;
            CloseV = closeV;
        }

        public Group Build(IMesh mesh)
        {
            var group = new Group();

            for (int i = 1; i < mesh.N; i++)
            {
                var g = new Group();
                for (int j = 1; j < mesh.M; j++)
                {
                    var s = new Sphere();
                    s.Scale(RadiusSphere);
                    s.Translate(mesh.Points[i][j]);
                    g.Add(s);

                    var c1 = Helper.Cylinder(mesh.Points[i][j], mesh.Points[i - 1][j], RadiusCylinder);
                    var c2 = Helper.Cylinder(mesh.Points[i][j], mesh.Points[i][j - 1], RadiusCylinder);
                    g.Add(c1);
                    g.Add(c2);
                }

                group.Add(g);
            }

            if (!CloseU)
            {
                for (int i = 1; i < mesh.M; i++)
                {
                    var c1 = Helper.Cylinder(mesh.Points[0][i - 1], mesh.Points[0][i], RadiusCylinder);
                    var s = new Sphere();
                    s.Scale(RadiusSphere);
                    s.Translate(mesh.Points[0][i]);
                    group.Add(c1);
                    group.Add(s);
                }
            }

            if (!CloseV)
            {
                for (int i = 1; i < mesh.N; i++)
                {
                    var c1 = Helper.Cylinder(mesh.Points[i - 1][0], mesh.Points[i][0], RadiusCylinder);
                    var s = new Sphere();
                    s.Scale(RadiusSphere);
                    s.Translate(mesh.Points[i][0]);
                    group.Add(c1);
                    group.Add(s);
                }
            }

            if (!CloseV && !CloseU)
            {
                var s = new Sphere();
                s.Scale(RadiusSphere);
                s.Translate(mesh.Points[0][0]);
                group.Add(s);
            }

            return group;
        }
    }
}