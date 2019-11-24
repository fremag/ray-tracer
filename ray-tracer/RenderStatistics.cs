using System;
using System.Threading;

namespace ray_tracer
{
    public class RenderStatistics
    {
        private int nbPixels;
        private DateTime stop = DateTime.MinValue;
        
        public DateTime Start { get; set; }
        public int TotalPixels { get; set; }
        public int NbPixels => nbPixels;
        public double Progress => ((double)NbPixels) / TotalPixels;
        public double Speed => NbPixels /  Time.TotalSeconds;
        public TimeSpan Time => (stop == DateTime.MinValue ? DateTime.Now : stop) - Start;
        
        public void IncNbPixels()
        {
            Interlocked.Increment(ref nbPixels);
        }

        public void Stop()
        {
            stop = DateTime.Now;
        }

        public override string ToString()
        {
            return $"t: {Time:hh\\:mm\\:ss}, pct: {Progress:p2}, speed: {Speed:n2} pix/s";
        }
    }
}