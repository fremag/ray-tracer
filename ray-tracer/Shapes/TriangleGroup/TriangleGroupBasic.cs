using System.Collections.Generic;

namespace ray_tracer.Shapes.TriangleGroup
{
    public class TriangleGroupBasic : AbstractTriangleGroup
    {
        private TriangleGroupBasic(List<Triangle> triangles, in bool keepParent) : base(triangles, keepParent)
        {
            
        }

        protected override void IntersectTriangles(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            for (int i = 0; i < Triangles.Count; i++)
            {
                var tri = Triangles[i];
                tri.Intersect(ref origin, ref direction, intersections);
            }
        }

        protected override AbstractTriangleGroup GetSubGroup(List<Triangle> triangles, bool keepParent)
        {
            return new TriangleGroupBasic(triangles, keepParent);
        }
    }
}