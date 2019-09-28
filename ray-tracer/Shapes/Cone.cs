using System;

namespace ray_tracer.Shapes
{
    public class Cone : AbstractShape
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool Closed { get; set; }
        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, Minimum, -1), PMax = Helper.CreatePoint(1, Maximum, 1)};

        public Cone(double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity, bool closed = false)
        {
            Minimum = minimum;
            Maximum = maximum;
            Closed = closed;
        }

        public override Intersections IntersectLocal(Ray ray)
        {
            var xs = new Intersections();
            var a = ray.Direction.X * ray.Direction.X - ray.Direction.Y * ray.Direction.Y + ray.Direction.Z * ray.Direction.Z;
            var b = 2 * ray.Origin.X * ray.Direction.X - 2 * ray.Origin.Y * ray.Direction.Y + 2 * ray.Origin.Z * ray.Direction.Z;
            var c = ray.Origin.X * ray.Origin.X - ray.Origin.Y * ray.Origin.Y + ray.Origin.Z * ray.Origin.Z;

            if (Math.Abs(a) <= double.Epsilon && Math.Abs(b) > double.Epsilon)
            {
                var t = -c / (2 * b);
                xs.Add(new Intersection(t, this));
            }

            if (Math.Abs(a) > double.Epsilon)
            {
                var disc = b * b - 4 * a * c;
                if (disc >= 0)
                {
                    var t0 = (-b - Math.Sqrt(disc)) / (2 * a);
                    var t1 = (-b + Math.Sqrt(disc)) / (2 * a);

                    if (t0 > t1)
                    {
                        var tmp = t0;
                        t0 = t1;
                        t1 = tmp;
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

            double y = Math.Sqrt(worldPoint.X * worldPoint.X + worldPoint.Z * worldPoint.Z);
            if (worldPoint.Y > 0)
            {
                y *= -1;
            }
            return Helper.CreateVector(worldPoint.X, y, worldPoint.Z);
        }

        // a helper function to reduce duplication.
        // checks to see if the intersection at `t` is within a radius
        // of y (the radius of your cone) from the y axis.
        private bool CheckCap(Ray ray, double t, double y)
        {
            var x = ray.Origin.X + t * ray.Direction.X;
            var z = ray.Origin.Z + t * ray.Direction.Z;
            return (x * x + z * z) <= y*y;
        }

        private void IntersectCaps(Ray ray, Intersections xs)
        {
            // caps only matter if the cone is closed, and might possibly be intersected by the ray.
            if (!Closed || Math.Abs(ray.Direction.Y) <= double.Epsilon)
            {
                return;
            }

            // check for an intersection with the lower end cap by intersecting
            // the ray with the plane at y=cone.minimum
            var t = (Minimum - ray.Origin.Y) / ray.Direction.Y;
            if (CheckCap(ray, t, Minimum))
            {
                xs.Add(new Intersection(t, this));
            }

            // check for an intersection with the upper end cap by intersecting
            // the ray with the plane at y=cone.maximum
            t = (Maximum - ray.Origin.Y) / ray.Direction.Y;
            if (CheckCap(ray, t, Maximum))
            {
                xs.Add(new Intersection(t, this));
            }
        }
    }
}