using System.Collections.Generic;
using System.Linq;
using ray_tracer.Shapes.Mesh;
using ray_tracer.Triangulation;

namespace ray_tracer.Shapes
{
    public class Prism : Group
    {
        public Prism(IEnumerable<Point2D> points)
        {
            var point2Ds = points as Point2D[] ?? points.ToArray();
            var bottom = BuildPolygonShape(point2Ds);
            var top = BuildPolygonShape(point2Ds).Translate(ty: 1);

            var mesh = new PrismMesh(point2Ds);
            var triangleMeshFactory = new TriangleMeshFactory(false);
            var walls = triangleMeshFactory.Build(mesh);

            Add(walls, top, bottom);
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