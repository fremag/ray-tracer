using System;

namespace ray_tracer.Lights
{
    public class SpotLight : ILight
    {
        /*                   Position
         *                  __+__  R1    
         *                 /    \
         *               /       \
         *             /_____+____\ R2
         *                Center  
         */
        
        public Color Intensity { get; }
        public Tuple Position { get; }

        public double R1 { get; }    
        public double R2 { get; }
        public int N { get; }

        private Tuple normal;
        private Tuple center;
        private readonly Random rand = new Random(0);
        private readonly Tuple v1; 
        private readonly Tuple v2;
        
        public SpotLight(Tuple position, Color intensity, Tuple center, double r1, double r2, int n=1) 
        {
            Position = position;
            Intensity = intensity;
            this.center = center;
            R1 = r1;
            R2 = r2;
            N = n;
            normal = (center - Position).Normalize();

            // let's take a random vector not // to normal
            Tuple randomVector = Helper.RandomVector();
            double dotProd = randomVector.DotProduct(normal);
            while ( Math.Abs(dotProd) <= Helper.Epsilon)
            {
                randomVector = Helper.RandomVector();
                dotProd = randomVector.DotProduct(normal);
            }

            // v1 = randVector * normal so v1 is orthogonal to normal
            v1 = (normal * randomVector).Normalize();
            // v2 = normal * v1 so v2 is orthogonal to normal and v1
            // so we have a base of unit vectors.
            // so V1 and v2 are a base for a 2D plane and its normal vector is normal tuple 
            v2 = (normal * v1).Normalize();
        }

        public Color GetIntensityAt(double x, double y, double z, ref Tuple point)
        {
            var ray = (Position - point).Normalize();
            var intersection = Helper.IntersectDisk(ref normal, ref center, R2, ref point, ref ray);
            return intersection ? Intensity : Color.Black;
        }

        public unsafe int GetPositions(double* x, double* y, double* z)
        {
            if (N == 1)
            {
                x[0] = Position.X;
                y[0] = Position.Y;
                z[0] = Position.Z;
                return 1;
            }
            double sector = 2 * Math.PI / N;
            lock (rand)
            {
                for (int i = 0; i < N; i++)
                {
                    // to have a uniform distribution in the disk, we must take sqrt(t)
                    // http://mathworld.wolfram.com/DiskPointPicking.html
                    double r = R1*Math.Sqrt(rand.NextDouble());
                    double alpha = sector * (i + rand.NextDouble());
                    var cos = Math.Cos(alpha);
                    var sin = Math.Sin(alpha);
                    x[i] = Position.X + r * (cos * v1.X + sin * v2.X);
                    y[i] = Position.Y + r * (cos * v1.Y + sin * v2.Y);
                    z[i] = Position.Z + r * (cos * v1.Z + sin * v2.Z);
                }
            }

            return N;
        }
    }
}