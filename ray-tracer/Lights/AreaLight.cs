using System;

namespace ray_tracer.Lights
{
    public class AreaLight : ILight
    {
        public Tuple Position { get; }
        public Color Intensity { get; }

        public Tuple UVec { get; }
        public Tuple VVec { get; }

        public int USteps { get; }
        public int VSteps { get; }

        private readonly Random rand = new Random(0);
        
        public AreaLight(Tuple position, Color intensity, Tuple v1, Tuple v2, int uSteps, int vSteps)
        {
            Position = position + (v1+v2)/2;
            Intensity = intensity;
            UVec = v1 / uSteps;
            VVec = v2 / vSteps;
            USteps = uSteps;
            VSteps = vSteps;
        }

        public Color GetIntensityAt(double x, double y, double z, ref Tuple point) => Intensity;

        public unsafe int GetPositions(double* x, double* y, double* z)
        {
            int k = 0;
            for(int i=0; i < USteps; i++)
            {
                for (int j = 0; j < VSteps; j++ , k++)
                {
                    double u = -USteps / 2.0 + i + rand.NextDouble();
                    double v = -VSteps / 2.0 + j + rand.NextDouble();
                    x[k] = Position.X + u * UVec.X + v * VVec.X;
                    y[k] = Position.Y + u * UVec.Y + v * VVec.Y;
                    z[k] = Position.Z + u * UVec.Z + v * VVec.Z;
                }
            }

            return USteps * VSteps;
        }
    }
}