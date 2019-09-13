using System;
using System.Collections.Generic;
using System.Linq;

namespace ray_tracer
{
    public class IntersectionData
    {
        public double T { get; }
        public IShape Object { get; }
        public Tuple Point { get; }
        public Tuple EyeVector { get; }
        public Tuple ReflectionVector { get; }
        public Tuple Normal { get; }
        public bool Inside { get; }
        public Tuple OverPoint { get; }
        public Tuple UnderPoint { get; }
        public double N1 { get; private set; }
        public double N2 { get; private set; }

        public IntersectionData(Intersection intersection, Ray ray, Intersections intersections = null)
        {
            T = intersection.T;
            Object = intersection.Object;
            Point = ray.Position(T);
            EyeVector = -ray.Direction;
            Normal = Object.NormalAt(Point);
            ReflectionVector = ray.Direction.Reflect(Normal);
            
            if (Normal.DotProduct(EyeVector) < 0)
            {
                Inside = true;
                Normal = - Object.NormalAt(Point);
            } 
            else
            {
                Inside = false;
            }

            OverPoint = Point + Normal * Helper.Epsilon;
            UnderPoint = Point - Normal * Helper.Epsilon;
            
            if (intersections != null)
            {
                ComputeN1N2(intersection, intersections);
            }
        }

        private void ComputeN1N2(Intersection intersection, Intersections intersections)
        {
            var containers = new List<IShape>();
            foreach (var intersec in intersections)
            {
                if (intersec == intersection)
                {
                    N1 = containers.Any() ? containers.Last().Material.RefractiveIndex : 1.0;
                }

                if (containers.Contains(intersec.Object))
                {
                    containers.Remove(intersec.Object);
                }
                else
                {
                    containers.Add(intersec.Object);
                }

                if (intersec == intersection)
                {
                    N2 = containers.Any() ? containers.Last().Material.RefractiveIndex : 1.0;
                }
            }
        }

        public double Schlick()
        {
            // find the cosine of the angle between the eye and normal vectors
            double cos = EyeVector.DotProduct(Normal);
            
            // total internal reflection can only occur if n1 > n2
            if (N1 > N2)
            {
                double n = N1 / N2;
                double sin2t = n * n * (1.0 - cos * cos);
                if (sin2t > 1.0)
                {
                    return 1.0;
                }

                // compute cosine of theta_t using trig identity 
                var cost = Math.Sqrt(1.0 - sin2t);
                // when n1 > n2, use cos(theta_t) instead 
                cos = cost;
            }

            var r0 = Math.Pow((N1 - N2) / (N1 + N2), 2);
            return r0 + (1 - r0) * Math.Pow((1 - cos), 5);
        }
    }
}