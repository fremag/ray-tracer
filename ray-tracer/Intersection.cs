using System;

namespace ray_tracer
{
    public class Intersection : IComparable<Intersection>
    {
        public double T { get; }
        public IShape Object { get; }

        public Intersection(double t, IShape o)
        {
            T = t;
            Object = o;
        }

        public int CompareTo(Intersection other)
        {
            if (T < other.T) return -1;
            if (T > other.T) return 1;
            return 0;
        }

        public IntersectionData Compute(Ray ray, Intersections intersections = null ) => new IntersectionData(this, ray, intersections);
    }
}