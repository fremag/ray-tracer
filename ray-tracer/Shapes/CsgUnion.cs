namespace ray_tracer.Shapes
{
    public class CsgUnion : AbstractCsg
    {
        public CsgUnion(IShape left, IShape right) : base(left, right)
        {
        }

        public override bool IntersectionAllowed(bool leftHit, bool insideLeft, bool insideRight)
        {
            return (leftHit && !insideRight) || (!leftHit && !insideLeft);
        }
    }
}