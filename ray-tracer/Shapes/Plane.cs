using System;

namespace ray_tracer.Shapes
{
    public class Plane : AbstractShape
    {
        private Tuple normal = Helper.CreateVector(0, 1, 0);

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null) => normal;
        
        public override Bounds Box  => new Bounds{PMin = Helper.CreatePoint(double.NegativeInfinity,0, double.NegativeInfinity), PMax = Helper.CreatePoint(double.PositiveInfinity,0, double.PositiveInfinity)}; 

        public override Intersections IntersectLocal(ref Tuple origin, ref Tuple direction)
        {
            // if ray is // to plane => no intersection
            if (Math.Abs(direction.Y) < Helper.Epsilon)
            {
                return Intersections.Empty;
            }

            var t = - origin.Y / direction.Y;
            return new Intersections(new Intersection(t, this));
        }
    }
}