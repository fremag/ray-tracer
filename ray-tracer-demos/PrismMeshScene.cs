using System;
using System.Collections.Generic;
using System.Linq;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using ray_tracer.Shapes.Mesh;
using ray_tracer.Triangulation;
using Tuple = ray_tracer.Tuple;

namespace ray_tracer_demos
{
    public class PrismMeshScene : AbstractScene
    {
        public PrismMeshScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 1, CameraZ = -1,
                LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            DefaultFloor();
            Light(1, 1, -1);

            Add(BuildPolygon(3).Scale(0.2, 0.25, 0.2).Translate(tx: -0.75, tz: 0));
            Add(BuildPolygon(4).Scale(0.2, 0.25, 0.2).Translate(tx: -0, tz: 0.75));
            Add(BuildPolygon(5).Scale(0.2, 0.25, 0.2).Translate(tx: 0.75, tz: 0.5));
            Add(BuildPolygon(6).Scale(0.2, 0.25, 0.2).Translate(tx: 0.5, tz: -0.5));

            IEnumerable<Point2D> points = new Point2D[]
            {
                new Point2D(-3, 5), 
                new Point2D(3, 5),
                new Point2D(3, 3),
                new Point2D(-1, 3), 
                new Point2D(-1, 1),
                new Point2D(1, 1),

                new Point2D(1, -1),
                new Point2D(-1, -1),
                new Point2D(-1, -3),
                new Point2D(3, -3),
                new Point2D(3, -5),
                new Point2D(-3, -5)
            }.Reverse();
            
            var mesh = new PrismMesh(points);
            var triangleMeshFactory = new TriangleMeshFactory(false);
            var walls = triangleMeshFactory.Build(mesh);
            var bottom = BuildPolygonShape(points);
            var top = BuildPolygonShape(points).Translate(ty: 1);

            var letterE = new Group();
            letterE.Add(walls);
            letterE.Add(top);
            letterE.Add(bottom);

            Add(letterE.Scale(0.1).Rotate(ry: Math.PI/4));
        }

        private IShape BuildPolygon(int n)
        {
            var points = Enumerable.Range(0, n).Select(i =>
            {
                var x = Math.Cos(2 * Math.PI * i / n);
                var y = Math.Sin(2 * Math.PI * i / n);
                return new Point2D(x, y);
            }).ToArray();
            var mesh = new PrismMesh(points);
            var triangleMeshFactory = new TriangleMeshFactory(false);
            var prism =  triangleMeshFactory.Build(mesh);

            var bottom = BuildPolygonShape(points);
            var top = BuildPolygonShape(points).Translate(ty: 1);
            var g = new Group();
            g.Add(prism);
            g.Add(bottom);
            g.Add(top);
            return g;
        }

        private IShape BuildPolygonShape(IEnumerable<Point2D> points)
        {
            var polygon = new Polygon2D();
            polygon.Points.AddRange(points);
            var triangles = new List<Triangle2D>();
            polygon.Triangulation(triangles);

            var group = new Group();
            for (var i = 0; i < triangles.Count; i++)
            {
                var triangle2D = triangles[i];
                var triangle = new Triangle(
                    Helper.CreatePoint(triangle2D.A.X, 0, triangle2D.A.Y),
                    Helper.CreatePoint(triangle2D.B.X, 0, triangle2D.B.Y),
                    Helper.CreatePoint(triangle2D.C.X, 0, triangle2D.C.Y)
                );
                group.Add(triangle);
            }

            return group;
        }
    }
}