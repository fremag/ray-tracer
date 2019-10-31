using System.Collections.Generic;
using System.Linq;

namespace ray_tracer
{
    public class Intersections : List<Intersection>
    {
        public static Intersections Empty = new Intersections();
        public Intersections(params Intersection[] intersections) : base(intersections)
        {
            Sort();
        }
        
        public Intersections(IEnumerable<Intersection> intersections) : base(intersections)
        {
            Sort();
        }

        public Intersections()
        {
            
        }

        public Intersection Hit() => this.FirstOrDefault(i => i.T >= 0);
    }
}