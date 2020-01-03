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
                    if (RadiusSphere > 0)
                    {
                        var s = new Sphere();
                        s.Scale(RadiusSphere);
                        s.Translate(mesh.Points[i][j]);
                        g.Add(s);
                    }

                    if (RadiusCylinder > 0)
                    {
                        var c1 = Helper.Cylinder(mesh.Points[i][j], mesh.Points[i - 1][j], RadiusCylinder);
                        c1.Material = new Material(Color._Green + Color._Red);
                        var c2 = Helper.Cylinder(mesh.Points[i][j], mesh.Points[i][j - 1], RadiusCylinder);
                        c2.Material = new Material(Color._Green + Color._Blue);
                        g.Add(c1);
                        g.Add(c2);
                    }
                }

                group.Add(g);
            }

            for (int i = 1; i < mesh.M; i++)
            {
                if (RadiusCylinder > 0)
                {

                    var c1 = Helper.Cylinder(mesh.Points[0][i - 1], mesh.Points[0][i], RadiusCylinder);
                    group.Add(c1);
                }

                if (RadiusSphere > 0)
                {
                    var s = new Sphere();
                    s.Scale(RadiusSphere);
                    s.Translate(mesh.Points[0][i]);
                    group.Add(s);
                }
            }

            if (CloseU)
            {
                for (int i = 0; i < mesh.M; i++)
                {
                    var p0 = mesh.Points[0][i];
                    var p1 = mesh.Points[mesh.N - 1][i];
                    if (RadiusCylinder > 0)
                    {
                        var c1 = Helper.Cylinder(p1, p0, RadiusCylinder);
                        c1.Material = new Material(Color._Green);
                        group.Add(c1);
                    }

                    if (RadiusSphere > 0)
                    {
                        var s = new Sphere();
                        s.Scale(RadiusSphere);
                        s.Translate(p0);
                        group.Add(s);
                    }
                }
            }

            for (int i = 1; i < mesh.N; i++)
            {
                if (RadiusCylinder > 0)
                {
                    var c1 = Helper.Cylinder(mesh.Points[i - 1][0], mesh.Points[i][0], RadiusCylinder);
                    group.Add(c1);
                }

                if (RadiusSphere > 0)
                {
                    var s = new Sphere();
                    s.Scale(RadiusSphere);
                    s.Translate(mesh.Points[i][0]);
                    group.Add(s);
                }
            }

            if (CloseV && RadiusCylinder > 0)
            {
                for (int i = 0; i < mesh.N; i++)
                {
                    var c1 = Helper.Cylinder(mesh.Points[i][0], mesh.Points[i][mesh.M - 1], RadiusCylinder);
                    c1.Material = new Material(Color._Red);
                    group.Add(c1);
                }
            }

            if (!CloseV && !CloseU && RadiusSphere > 0)
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