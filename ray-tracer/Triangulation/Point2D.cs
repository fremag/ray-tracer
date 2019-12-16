using System.Numerics;

namespace ray_tracer.Triangulation
{
    public class Point2D
    {
        public double X { get; }
        public double Y { get; }

        public Vector3 Vector => new Vector3((float) X, (float) Y, 0);
        
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool IsLeft(Point2D p0, Point2D p1)
        {
            var d =  Dist(p0, p1);
            return d >= 0;
        }

        public double Dist(Point2D p0, Point2D p1)
        {
            var d = (p1.X - p0.X) * (Y - p0.Y) - (p1.Y - p0.Y) * (X - p0.X);
            return d;
        }

        public override string ToString() => $"({X}, {Y})";
    }
}