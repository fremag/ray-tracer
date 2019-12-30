namespace ray_tracer.Lights
{
    public class ConeLight : ILight
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

        private Tuple normal;
        private Tuple center;

        public ConeLight(Tuple position, Color intensity, Tuple center, double r1, double r2) 
        {
            Position = position;
            Intensity = intensity;
            this.center = center;
            R1 = r1;
            R2 = r2;
            normal = (center - Position).Normalize();
        }

        public Color GetIntensityAt(ref Tuple point)
        {
            var ray = (Position - point).Normalize();
            var intersection = Helper.IntersectDisk(ref normal, ref center, R2, ref point, ref ray);
            return intersection ? Intensity : Color.Black;
        }

        public unsafe int GetPositions(double* x, double* y, double* z)
        {
            x[0] = Position.X;
            y[0] = Position.Y;
            z[0] = Position.Z;
            return 1;
        }
    }
}