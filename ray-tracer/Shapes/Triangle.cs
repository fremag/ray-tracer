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

        public override Bounds Box { get; }

        public Triangle(Tuple p1, Tuple p2, Tuple p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            E1 = p2 - p1;
            E2 = p3 - p1;
            N = (E2 * E1).Normalize();
            var box = new Bounds();
            box.Add(P1);
            box.Add(P2);
            box.Add(P3);
            Box = box;
        }

        public override Intersections IntersectLocal(Ray ray)
        {
            var rayDir = ray.Direction;

            var dirCrossE2_X = rayDir.Y * E2.Z - rayDir.Z * E2.Y;
            var dirCrossE2_Y = rayDir.Z * E2.X - rayDir.X * E2.Z;
            var dirCrossE2_Z = rayDir.X * E2.Y - rayDir.Y * E2.X;

            var det = E1.X * dirCrossE2_X + E1.Y * dirCrossE2_Y + E1.Z * dirCrossE2_Z;
            if (Math.Abs(det) < Helper.Epsilon)
            {
                return Intersections.Empty;
            }

            var f = 1.0 / det;
            var p1ToOrigin_X = ray.Origin.X - P1.X;
            var p1ToOrigin_Y = ray.Origin.Y - P1.Y;
            var p1ToOrigin_Z = ray.Origin.Z - P1.Z;

            var u = f * (p1ToOrigin_X * dirCrossE2_X + p1ToOrigin_Y * dirCrossE2_Y + p1ToOrigin_Z * dirCrossE2_Z);
            if (u < 0 || u > 1)
            {
                return Intersections.Empty;
            }

            var originCrossE1_X = p1ToOrigin_Y * E1.Z - p1ToOrigin_Z * E1.Y;
            var originCrossE1_Y = p1ToOrigin_Z * E1.X - p1ToOrigin_X * E1.Z;
            var originCrossE1_Z = p1ToOrigin_X * E1.Y - p1ToOrigin_Y * E1.X;

            var v = f * (rayDir.X * originCrossE1_X + rayDir.Y * originCrossE1_Y + rayDir.Z * originCrossE1_Z);
            if (v < 0 || (u + v) > 1)
            {
                return Intersections.Empty;
            }

            var t = f * (E2.X * originCrossE1_X + E2.Y * originCrossE1_Y + E2.Z * originCrossE1_Z);
            return new Intersections {new Intersection(t, this, u, v)};
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit = null)
        {
            return N;
        }
        
        public Intersections _IntersectLocal(Ray ray)
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
            return new Intersections {new Intersection(t, this, u, v)};
        }        
    }
}