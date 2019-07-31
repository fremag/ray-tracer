using System;

namespace ray_tracer
{
    public class Plane : AbstractShape
    {
        private Tuple normal = Helper.CreateVector(0, 1, 0);

        public override Tuple NormalAt(Tuple worldPoint) => normal;
        
        public override Intersections Intersect(Ray ray)
        {
            // if ray is // to plane => no intersection
            if (Math.Abs(ray.Direction.Y) < Helper.Epsilon)
            {
                return new Intersections();
            }

            var t = -ray.Origin.Y / ray.Direction.Y;
            return new Intersections(new [] {new Intersection(t, this)});
        }
    }
}