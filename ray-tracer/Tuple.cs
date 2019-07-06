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
    }
}