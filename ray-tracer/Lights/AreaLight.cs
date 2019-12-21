using System;

namespace ray_tracer.Lights
{
    public class AreaLight : ILight
    {
        public Tuple Position { get; }
        public Tuple UVec { get; }
        public Tuple VVec { get; }
        public Color Intensity { get; }

        public Tuple V1 { get; } 
        public Tuple V2 { get; }
        
        public int USteps { get; }
        public int VSteps { get; }

        public AreaLight(Tuple corner, Color intensity, Tuple v1, Tuple v2, int uSteps, int vSteps)
        {
            Position = corner + (v1+v2)/2;
            Intensity = intensity;
            UVec = v1 / uSteps;
            VVec = v2 / vSteps;
            USteps = uSteps;
            VSteps = vSteps;
        }

        private Random rand = new Random(0);
        public double IntensityAt(Tuple point, World world)
        {
            double intensity = 0;
            
            for(int i=0; i < USteps; i++)
            {
                for (int j = 0; j < VSteps; j++)
                {
                    double u = -USteps / 2.0 + i + rand.NextDouble();
                    double v = -VSteps / 2.0 + j + rand.NextDouble();
                    var dU = u * UVec;
                    var dV = v * VVec;
                    var p = Position + dU + dV;
                    var isShadowed = world.IsShadowed(point, p);
                    intensity += isShadowed ? 0: 1;
                }
            }

            return intensity / (USteps*VSteps);
        }
    }
}