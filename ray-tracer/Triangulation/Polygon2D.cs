using System;
using System.Collections.Generic;

namespace ray_tracer.Triangulation
{
    // http://www.f-legrand.fr/scidoc/srcdoc/graphie/geometrie/polygone/polygone-pdf.pdf
    public class Polygon2D
    {
        public List<Point2D> Points { get; } = new List<Point2D>();
        private static int NextIndex(int n, int i, int di) => (i + n +di) % n;
        
        public int MaxDist(Point2D p0, Point2D p1, Point2D p2, HashSet<int> indices)
        {
            var n = Points.Count;
            var maxDist = 0.0;
            int j = -1;
            var triangle = new Triangle2D(p0, p1, p2);
            for (int i = 0; i < n; i++)
            {
                if (indices.Contains(i))
                {
                    continue;
                }

                var p = Points[i];
                var inside = triangle.IsInside(p);
                if (!inside)
                {
                    continue;
                }

                var dist = Math.Abs(p.Dist(p1, p2));
                if (dist > maxDist)
                {
                    maxDist = dist;
                    j = i;
                }
            }
        
            return j;
        }

        public int MostLeftPoint()
        {
            var x = Points[0].X;
            int j = 0;
            for (var i = 0; i < Points.Count; i++)
            {
                var p = Points[i];
                if (p.X < x)
                {
                    x = p.X;
                    j = i;
                }
            }

            return j;
        }

        public Polygon2D Extract(int start, int end)
        {
            var poly = new Polygon2D();
            int i = start;
            while (i != end)
            {
                poly.Points.Add(Points[i]);
                i = NextIndex(Points.Count, i, 1);
            }

            poly.Points.Add(Points[end]);
            return poly;
        }

        public void Triangulation(List<Triangle2D> triangles)
        {
            if (Points.Count < 3)
            {
                return;
            }

            if (Points.Count == 3)
            {
                triangles.Add(new Triangle2D(Points[0], Points[1], Points[2]));
                return;
            }
            int n = Points.Count;
            int j0 = MostLeftPoint();
            int j1 = NextIndex(n, j0, 1);
            int j2 = NextIndex(n, j0, -1);
            var p0 = Points[j0];
            var p1 = Points[j1];
            var p2 = Points[j2];

            int j = MaxDist(p0, p1, p2, new HashSet<int> {j0, j1, j2});

            if (j < 0)
            {
                triangles.Add(new Triangle2D(p0, p1, p2));
                var poly = Extract(j1, j2);
                if (poly.Points.Count == 3)
                {
                    triangles.Add(new Triangle2D(poly.Points[0], poly.Points[1], poly.Points[2]));
                }
                else
                {
                    poly.Triangulation(triangles);
                }
            }
            else
            {
                var poly1 = Extract(j0, j);
                var poly2 = Extract(j, j0);
                if (poly1.Points.Count == 3)
                {
                    triangles.Add(new Triangle2D(poly1.Points[0], poly1.Points[1], poly1.Points[2]));
                }
                else
                {
                    poly1.Triangulation(triangles);
                }

                if (poly2.Points.Count == 3)
                {
                    triangles.Add(new Triangle2D(poly2.Points[0], poly2.Points[1], poly2.Points[2]));
                }
                else
                {
                    poly2.Triangulation(triangles);
                }

            }
        }
    }
}