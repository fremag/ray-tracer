using System;

namespace ray_tracer
{
    public class Intersection : IComparable<Intersection>
    {
        public double T { get; }
        public Sphere Object { get; }

        public Intersection(double t, Sphere o)
        {
            T = t;
            Object = o;
        }

        public int CompareTo(Intersection other) => Math.Sign(T - other.T);
        public IntersectionData Compute(Ray ray) => new IntersectionData(this, ray);
    }
}