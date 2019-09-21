using System;

namespace ray_tracer.Shapes
{
    public class Cylinder : AbstractShape
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool Closed { get; set; }

        public Cylinder(double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity, bool closed = false)
        {
            Minimum = minimum;
            Maximum = maximum;
            Closed = closed;
        }

        public override Intersections IntersectLocal(Ray ray)
        {

            var xs = new Intersections();
            var a = ray.Direction.X * ray.Direction.X + ray.Direction.Z * ray.Direction.Z;
            // ray is not parallel to the y axis
            if (a > double.Epsilon)
            {

                var b = 2 * ray.Origin.X * ray.Direction.X + 2 * ray.Origin.Z * ray.Direction.Z;
                var c = ray.Origin.X * ray.Origin.X + ray.Origin.Z * ray.Origin.Z - 1;
                var disc = b * b - 4 * a * c;

                // ray does not intersect the cylinder
                if (disc < 0)
                {
                    return xs;
                }

                var t0 = (-b - Math.Sqrt(disc)) / (2 * a);
                var t1 = (-b + Math.Sqrt(disc)) / (2 * a);

                if (t0 > t1)
                {
                    var t = t0;
                    t0 = t1;
                    t1 = t;
                }

                var y0 = ray.Origin.Y + t0 * ray.Direction.Y;
                if (Minimum < y0 && y0 < Maximum)
                {
                    xs.Add(new Intersection(t0, this));
                }

                var y1 = ray.Origin.Y + t1 * ray.Direction.Y;
                if (Minimum < y1 && y1 < Maximum)
                {
                    xs.Add(new Intersection(t1, this));
                }
            }

            IntersectCaps(ray, xs);
            xs.Sort();
            return xs;
        }

        public override Tuple NormalAtLocal(Tuple worldPoint)
        {
            // compute the square of the distance from the y axis
            var dist = worldPoint.X * worldPoint.X + worldPoint.Z * worldPoint.Z;
            if (dist < 1 && worldPoint.Y >= Maximum - Helper.Epsilon)
            {
                return Helper.CreateVector(0, 1, 0);
            }

            if (dist < 1 && worldPoint.Y <= Minimum + Helper.Epsilon)
            {
                return Helper.CreateVector(0, -1, 0);
            }

            return Helper.CreateVector(worldPoint.X, 0, worldPoint.Z);
        }

        // a helper function to reduce duplication.
        // checks to see if the intersection at `t` is within a radius
        // of 1 (the radius of your cylinders) from the y axis.
        private bool CheckCap(Ray ray, double t)
        {
            var x = ray.Origin.X + t * ray.Direction.X;
            var z = ray.Origin.Z + t * ray.Direction.Z;
            return (x * x + z * z) <= 1;
        }

        private void IntersectCaps(Ray ray, Intersections xs)
        {
            // caps only matter if the cylinder is closed, and might possibly be intersected by the ray.
            if (!Closed || Math.Abs(ray.Direction.Y) <= double.Epsilon)
            {
                return;
            }

            // check for an intersection with the lower end cap by intersecting
            // the ray with the plane at y=cyl.minimum
            var t = (Minimum - ray.Origin.Y) / ray.Direction.Y;
            if (CheckCap(ray, t))
            {
                xs.Add(new Intersection(t, this));
            }

            // check for an intersection with the upper end cap by intersecting
            // the ray with the plane at y=cyl.maximum
            t = (Maximum - ray.Origin.Y) / ray.Direction.Y;
            if (CheckCap(ray, t))
            {
                xs.Add(new Intersection(t, this));
            }
        }
    }
}