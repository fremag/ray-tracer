using System.Collections.Generic;
using ray_tracer.Triangulation;
using Xunit;

namespace ray_tracer.tests.Triangulation
{
    public class Polygon2DTests
    {
        [Fact]
        public void TriangulationTest()
        {
            var polygon = new Polygon2D();
            polygon.Points.Add(new Point2D(0,0));
            polygon.Points.Add(new Point2D(0.5,-1));
            polygon.Points.Add(new Point2D(1.5,-0.2));
            polygon.Points.Add(new Point2D(2,-0.5));
            polygon.Points.Add(new Point2D(2,0));
            polygon.Points.Add(new Point2D(1.5,1));
            polygon.Points.Add(new Point2D(0.3,0));
            polygon.Points.Add(new Point2D(0.5,1));

            List<Triangle2D> triangles = new List<Triangle2D>();
            polygon.Triangulation(triangles);
            
        }
    }
}