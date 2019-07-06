using System;

namespace raytracer
{
    public static class Helper
    {
        const double Epsilon = 1e-6;

        public static Tuple CreatePoint(double x, double y, double z) => new Tuple(x, y, z, 1);
        public static Tuple CreateVector(double x, double y, double z) => new Tuple(x, y, z, 0);
        public static bool IsPoint(this Tuple tuple) => tuple.W == 1;
        public static bool IsVector(this Tuple tuple) => tuple.W == 0;
        public static bool AreEquals(double d1, double d2) => Math.Abs(d1 - d2) < Epsilon;
    }
}