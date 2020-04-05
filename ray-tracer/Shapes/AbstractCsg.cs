using System.Linq;

namespace ray_tracer.Shapes
{
    public abstract class AbstractCsg : Group
    {
        public IShape Left { get; private set; }
        public IShape Right { get; private set; }

        public abstract bool IntersectionAllowed(bool leftHit, bool insideLeft, bool insideRight);

        protected AbstractCsg()
        {
        }

        protected AbstractCsg(IShape left, IShape right)
        {
            Init(left, right);
        }

        public void Init(IShape left, IShape right)
        {
            Left = left;
            Right = right;
            left.Parent = this;
            right.Parent = this;
            Shapes.Add(Left);
            Shapes.Add(Right);
        }

        public override bool Contains(IShape shape)
        {
            return ReferenceEquals(Right, shape) || ReferenceEquals(Left, shape) || Right.Contains(shape) || Left.Contains(shape);
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

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            Intersections leftXs = new Intersections(); 
            Left.Intersect(ref origin, ref direction, leftXs);
            var rightXs = new Intersections(); 
            Right.Intersect(ref origin, ref direction, rightXs);
            Intersections result;
            if (leftXs.Any() || rightXs.Any())
            {
                 var xs = new Intersections(leftXs.Concat(rightXs));
                 result = Filter(xs);
                 intersections.AddRange(result);
            }
        }

        public override IShape Divide(int threshold)
        {
            Right.Divide(threshold);
            Left.Divide(threshold);
            return this;
        }
    }
}