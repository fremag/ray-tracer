using System.Collections.Generic;
using System.Linq;

namespace ray_tracer.Shapes
{
    public class Group : AbstractShape
    {
        private List<IShape> Shapes { get;  } = new List<IShape>();

        public void Add(params IShape[] shapes)
        {
            foreach (var shape in shapes)
            {
                Shapes.Add(shape);
                shape.Parent = this;
            }
        }
        
        public override Intersections IntersectLocal(Ray ray)
        {
            var intersections = Shapes.SelectMany(shape => shape.Intersect(ray));
            return new Intersections(intersections);
        }

        public override Tuple NormalAtLocal(Tuple worldPoint)
        {
            throw new System.InvalidOperationException();
        }
    }
}