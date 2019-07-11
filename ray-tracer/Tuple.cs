using System;

namespace raytracer
{
    public class Tuple
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double W { get; }

        public Tuple(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override bool Equals(object o)
        {
            var other = o as Tuple;
            if (other == null)
            {
                return false;
            }

            return other.X == X && other.Y == Y && other.Z == Z && other.W == W;
        }

        public Tuple Add(Tuple tuple) => new Tuple(X + tuple.X, Y + tuple.Y, Z + tuple.Z, W + tuple.W);
        public static Tuple operator +(Tuple t1, Tuple t2) => t1.Add(t2);

        public Tuple Sub(Tuple tuple) => new Tuple(X - tuple.X, Y - tuple.Y, Z - tuple.Z, W - tuple.W);
        public static Tuple operator -(Tuple t1, Tuple t2) => t1.Sub(t2);

        public Tuple Neg() => new Tuple(-X, -Y, -Z, -W);
        public static Tuple operator -(Tuple t1) => t1.Neg();

        public static Tuple operator *(Tuple t1, double coeff) =>
            new Tuple(t1.X * coeff, t1.Y * coeff, t1.Z * coeff, t1.W * coeff);

        public static Tuple operator /(Tuple t1, double coeff) =>
            new Tuple(t1.X / coeff, t1.Y / coeff, t1.Z / coeff, t1.W / coeff);

        public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public Tuple Normalize()
        {
            var magnitude = Magnitude;
            return new Tuple(X / magnitude, Y / magnitude, Z / magnitude, W / magnitude);
        }

        public double DotProduct(Tuple v) => v.X * X + v.Y * Y + v.Z * Z + v.W * W;

        public Tuple CrossProduct(Tuple v) =>
            Helper.CreateVector(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);

        public double this[in int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }
    }
}