using System;

namespace ray_tracer.Shapes
{
    public class Triangle : AbstractShape
    {
        public Tuple E1 { get; }
        public Tuple E2 { get; }
        public Tuple N { get; }

        public Tuple P1 { get; }
        public Tuple P2 { get; }
        public Tuple P3 { get; }

        private Bounds box;

        public Triangle(Tuple p1, Tuple p2, Tuple p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            E1 = p2 - p1;
            E2 = p3 - p1;
            N = (E2 * E1).Normalize();
        }

        public override Bounds Box
        {
            get
            {
                if (box != null) return box;
                box = new Bounds();
                box.Init(P1, P2, P3);
                return box;
            }
        }

        public override Intersections IntersectLocal(Ray ray)
        {
            var dirCrossE2 = ray.Direction * E2;
            var det = E1.DotProduct(dirCrossE2);
            if (Math.Abs(det) < Helper.Epsilon)
            {
                return new Intersections();
            }

            var f = 1.0 / det;
            var p1ToOrigin = ray.Origin - P1;
            var u = f * p1ToOrigin.DotProduct(dirCrossE2);
            if (u < 0 || u > 1)
            {
                return new Intersections();
            }

            var originCrossE1 = p1ToOrigin * E1;
            var v = f * ray.Direction.DotProduct(originCrossE1);
            if (v < 0 || (u + v) > 1)
            {
                return new Intersections();
            }

            var t = f * E2.DotProduct(originCrossE1);
            return new Intersections {new Intersection(t, this)};
        }

        public override Tuple NormalAtLocal(Tuple worldPoint)
        {
            return N;
        }
    }
}