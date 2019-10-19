using System.Linq;

namespace ray_tracer.Shapes
{
    public abstract class AbstractCsg : Group
    {
        public IShape Left { get; }
        public IShape Right { get; }

        public abstract bool IntersectionAllowed(bool leftHit, bool insideLeft, bool insideRight);

        protected AbstractCsg(IShape left, IShape right)
        {
            Left = left;
            Right = right;
            left.Parent = this;
            right.Parent = this;
        }

        protected AbstractCsg()
        {
            
        }

        public Intersections Filter(Intersections xs)
        {
// begin outside of both children
            bool inl = false;
            bool inr = false;
// prepare a list to receive the filtered intersections
            var result = new Intersections();
            foreach (var intersection in xs)
            {
// if i.object is part of the "left" child, then lhit is true
                bool lhit = intersection.Object.Contains(Left);
                if (IntersectionAllowed(lhit, inl, inr))
                {
                    result.Add(intersection);

                }

// depending on which object was hit, toggle either inl or inr
                if (lhit)
                {
                    inl = !inl;
                }
                else
                    inr = !inr;
            }

            return result;
        }

        public override Intersections IntersectLocal(Ray ray)
        {
            Intersections leftxs = Left.Intersect(ray);
            var rightxs = Right.Intersect(ray);
            var xs = new Intersections(leftxs.Concat(rightxs));
            var result = Filter(xs);
            return result;
        }
    }
}