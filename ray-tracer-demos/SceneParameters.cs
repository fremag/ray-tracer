namespace ray_tracer_demos
{
    public class RenderParameters
    {
        public int NbThreads { get; set; } = 4;
        public bool Shuffle { get; set; } = true;
    }
    
    public class SceneParameters
    {
        public string Scene { get; set; }
        
        public double CameraX { get; set; } = 0;
        public double CameraY { get; set;} = 1;
        public double CameraZ { get; set;} = -1;

        public double LookX { get; set;} = 0;
        public double LookY { get; set;} = 0;
        public double LookZ { get; set;} = 0;
        
        public int Height { get; set;} = 400;
        public int Width { get; set;} = 600;
    }
}