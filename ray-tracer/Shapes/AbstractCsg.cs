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

        public override bool Contains(IShape shape)
        {
            return ReferenceEquals(shape, Right) || ReferenceEquals(shape, Left);
        }

        public Intersections Filter(Intersections xs)
        {
            // begin outside of both children
            bool inLeft = false;
            bool inRight = false;
            
            // prepare a list to receive the filtered intersections
            var result = new Intersections();
            foreach (var intersection in xs)
            {
                // if i.object is part of the "left" child, then lhit is true
                bool leftHit = Left.Contains(intersection.Object);
                if (IntersectionAllowed(leftHit, inLeft, inRight))
                {
                    result.Add(intersection);
                }

                // depending on which object was hit, toggle either inl or inr
                if (leftHit)
                {
                    inLeft = !inLeft;
                }
                else
                {
                    inRight = !inRight;
                }
            }

            result.Sort();
            return result;
        }

        public override Intersections IntersectLocal(Ray ray)
        {
            Intersections leftXs = Left.Intersect(ray);
            var rightXs = Right.Intersect(ray);
            Intersections result;
            if (leftXs.Any() || rightXs.Any())
            {
                 var xs = new Intersections(leftXs.Concat(rightXs));
                 result = Filter(xs);
            }
            else
            {
                result = new Intersections();
            }

            return result;
        }
    }
}