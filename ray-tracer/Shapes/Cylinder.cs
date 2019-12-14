using System;

namespace ray_tracer.Shapes
{
    public class Cylinder : AbstractShape
    {
        private static Tuple NormTop { get; } = Helper.CreateVector(0, 1, 0);
        private static Tuple NormBottom { get; } = Helper.CreateVector(0, -1, 0);

        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool Closed { get; set; }

        public Cylinder(double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity, bool closed = false)
        {
            Minimum = minimum;
            Maximum = maximum;
            Closed = closed;
        }

        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, Minimum, -1), PMax = Helper.CreatePoint(1, Maximum, 1)};

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            var a = 2*(direction.X * direction.X + direction.Z * direction.Z);
            // ray is not parallel to the y axis
            if (a > double.Epsilon)
            {
                var b = 2 * (origin.X * direction.X + origin.Z * direction.Z);
                var c = origin.X * origin.X + origin.Z * origin.Z - 1;
                var disc = b * b - 2 * a * c;

                // ray does not intersect the cylinder
                if (disc < 0)
                {
                    return;
                }

                var x= -b/a;
                var sqrt = Math.Sqrt(disc) / a;
                var t0 = x - sqrt;
                var t1 = x + sqrt;

                if (t0 > t1)
                {
                    var t = t0;
                    t0 = t1;
                    t1 = t;
                }

                var y0 = origin.Y + t0 * direction.Y;
                if (Minimum < y0 && y0 < Maximum)
                {
                    intersections.Add(new Intersection(t0, this));
                }

                var y1 = origin.Y + t1 * direction.Y;
                if (Minimum < y1 && y1 < Maximum)
                {
                    intersections.Add(new Intersection(t1, this));
                }
            }

            IntersectCaps(new Ray(origin, direction), intersections);
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null)
        {
            // compute the square of the distance from the y axis
            var dist = worldPoint.X * worldPoint.X + worldPoint.Z * worldPoint.Z;
            if (dist < 1 && worldPoint.Y >= Maximum - Helper.Epsilon)
            {
                return NormTop;
            }

            if (dist < 1 && worldPoint.Y <= Minimum + Helper.Epsilon)
            {
                return NormBottom;
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