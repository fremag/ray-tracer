using System.Numerics;

namespace ray_tracer
{
    public class Ray
    {
        public Tuple Origin { get; }
        public Tuple Direction { get; }

        public Ray(ref Vector4 origin, ref Vector4 direction) : this(Helper.CreatePoint(origin.X, origin.Y, origin.Z), Helper.CreatePoint(direction.X, direction.Y, direction.Z ))
        {}
            
         
        public Ray(Tuple origin, Tuple direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Tuple Position(double t) => Origin + t * Direction;

        public Ray Transform(Matrix m)
        {
            return new Ray(Origin * m, Direction * m);
        }
    }
}