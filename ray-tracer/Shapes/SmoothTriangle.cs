namespace ray_tracer.Shapes
{
    public class SmoothTriangle : Triangle
    {
        public Tuple N1 { get; }
        public Tuple N2 { get; }
        public Tuple N3 { get; }

        public SmoothTriangle(Tuple p1, Tuple p2, Tuple p3, Tuple n1, Tuple n2, Tuple n3) : base(p1, p2, p3)
        {
            N1 = n1;
            N2 = n2;
            N3 = n3;
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit = null)
        {
            return N2 * hit.U + N3 * hit.V + N1 * (1 - hit.U - hit.V);
        }
    }
}