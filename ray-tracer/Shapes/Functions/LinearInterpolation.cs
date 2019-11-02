using System;

namespace ray_tracer.Shapes.Functions
{
    public class LinearInterpolation : IPath3D
    {
        public InterpolationData[] Points;

        public LinearInterpolation(params InterpolationData[] points)
        {
            Points = points;
            Array.Sort(Points);
        }

        public void GetPoint(double u, out double x, out double y, out double z)
        {
            FindPoints(u, out var p1, out var p2);
            if (p1 == p2)
            {
                x = p1.Point.X;
                y = p1.Point.Y;
                z = p1.Point.Z;
                return;
            }
            var t = (u-p1.T)*(p2.T - p1.T);
            var p = (p2.Point - p1.Point)*t + p1.Point;
            x = p.X;
            y = p.Y;
            z = p.Z;
        }

        public void FindPoints(double t, out InterpolationData p1, out InterpolationData p2)
        {
            if (t < Points[0].T)
            {
                p1 = Points[0];
                p2 = Points[0];
                return;
            }

            for (int i = 0; i < Points.Length-1; i++)
            {
                p1 = Points[i];
                p2 = Points[i+1];
                if (p1.T <= t && t <= p2.T)
                {
                    return;
                }
            }

            p1 = Points[^1];
            p2 = Points[^1];
        }
    }

    public class InterpolationData : IComparable<InterpolationData>
    {
        public double T { get; }
        public Tuple Point { get; }

        public InterpolationData(double t, Tuple point)
        {
            T = t;
            Point = point;
        }

        public InterpolationData(double t, double x, double y, double z)
        {
            T = t;
            Point = Helper.CreatePoint(x, y, z);
        }

        public int CompareTo(InterpolationData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return T.CompareTo(other.T);
        }
    }
}