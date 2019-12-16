namespace ray_tracer.Triangulation
{
    public class Triangle2D
    {
        public Point2D A { get; } 
        public Point2D B { get; } 
        public Point2D C { get; }

        public Triangle2D(Point2D a, Point2D b, Point2D c)
        {
            A = a;
            B = b;
            C = c;
        }

        public bool IsInside(Point2D p)
        {
            var l1 = p.IsLeft(A, B);
            var l2 = p.IsLeft(B, C);
            var l3 = p.IsLeft(C, A);

            return l1 && l2 && l3;
        }

        public override string ToString() => $"{{{A}, {B}, {C}}}";
    }
}