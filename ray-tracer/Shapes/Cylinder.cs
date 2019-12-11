using System;
using System.Numerics;

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

        public override Bounds Box => new Bounds {PMin =  Helper.CreatePoint(-1, Minimum, -1), PMax = Helper.CreatePoint(1, Maximum, 1)};

        public override Intersections IntersectLocal(Ray ray)
        {
            throw new InvalidOperationException();
        }

        public override Intersections IntersectLocal(ref Vector4 origin, ref Vector4 direction)
        {
            var xs = new Intersections();
            var a = direction.X * direction.X + direction.Z * direction.Z;
            // ray is not parallel to the y axis
            if (a > float.Epsilon)
            {
                var b = 2 * origin.X * direction.X + 2 * origin.Z * direction.Z;
                var c = origin.X * origin.X + origin.Z * origin.Z - 1;
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

                var y0 = origin.Y + t0 * direction.Y;
                if (Minimum < y0 && y0 < Maximum)
                {
                    xs.Add(new Intersection(t0, this));
                }

                var y1 = origin.Y + t1 * direction.Y;
                if (Minimum < y1 && y1 < Maximum)
                {
                    xs.Add(new Intersection(t1, this));
                }
            }

            IntersectCaps(ref origin, ref direction, xs);
            xs.Sort();
            return xs;
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit=null)
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
        private bool CheckCap(ref Vector4 origin, ref Vector4 direction, double t)
        {
            var x = origin.X + t * direction.X;
            var z = origin.Z + t * direction.Z;
            return (x * x + z * z) <= 1;
        }

        private void IntersectCaps(ref Vector4 origin, ref Vector4 direction, Intersections xs)
        {
            // caps only matter if the cylinder is closed, and might possibly be intersected by the ray.
            if (!Closed || Math.Abs(direction.Y) <= float.Epsilon)
            {
                return;
            }

            // check for an intersection with the lower end cap by intersecting
            // the ray with the plane at y=cyl.minimum
            var t = (Minimum - origin.Y) / direction.Y;
            if (CheckCap(ref origin, ref direction, t))
            {
                xs.Add(new Intersection(t, this));
            }

            // check for an intersection with the upper end cap by intersecting
            // the ray with the plane at y=cyl.maximum
            t = (Maximum - origin.Y) / direction.Y;
            if (CheckCap(ref origin, ref direction, t))
            {
                xs.Add(new Intersection(t, this));
            }
        }
    }
}