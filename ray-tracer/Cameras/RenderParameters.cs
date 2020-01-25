using System;

namespace ray_tracer.Cameras
{
    public class RenderParameters
    {
        public int NbThreads { get; set; } = Environment.ProcessorCount;
        public bool Shuffle { get; set; } = true;
    }
}