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

        public int CompareTo(Intersection other) => Math.Sign(T - other.T);
        public IntersectionData Compute(Ray ray, Intersections intersections = null ) => new IntersectionData(this, ray, intersections);
    }
}