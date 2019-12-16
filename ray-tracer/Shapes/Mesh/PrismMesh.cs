using System.Collections.Generic;
using System.Linq;
using ray_tracer.Triangulation;

namespace ray_tracer.Shapes.Mesh
{
    public class PrismMesh : AbstractMesh
    {
        public PrismMesh(IEnumerable<Point2D> points, bool close=false) : base(points.Count(), close ? 4 : 2)
        {
            int i = 0;
            foreach(var p in points)
            {
                if (close)
                {
                    Points[i][0] = Helper.CreatePoint(0, 0, 0);
                    Points[i][1] = Helper.CreatePoint(p.X, 0, p.Y);
                    Points[i][2] = Helper.CreatePoint(p.X, 1, p.Y);
                    Points[i][3] = Helper.CreatePoint(0, 1, 0);
                }
                else
                {
                    Points[i][0] = Helper.CreatePoint(p.X, 0, p.Y);
                    Points[i][1] = Helper.CreatePoint(p.X, 1, p.Y);
                }

                i++;
            }
        }
    }
}