using System;
using System.Linq;
using System.Numerics;

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

        public override Intersections IntersectLocal(ref Vector4 origin, ref Vector4 direction)
        {
            Intersections leftXs = Left.Intersect(ref origin, ref direction);
            var rightXs = Right.Intersect(ref origin, ref direction);
            Intersections result;
            if (leftXs.Any() || rightXs.Any())
            {
                var xs = new Intersections(leftXs.Concat(rightXs));
                result = Filter(xs);
            }
            else
            {
                result = Intersections.Empty;
            }

            return result;
        }
        
        public override Intersections IntersectLocal(Ray ray)
        {
            throw new InvalidOperationException();
        }
    }
}