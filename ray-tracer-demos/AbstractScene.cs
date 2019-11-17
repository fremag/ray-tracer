using System;
using System.IO;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public abstract class AbstractScene
    {
        private readonly World world = new World();
        public event Action<int, int> RowRendered;
        protected static readonly double Pi = Math.PI;
        protected static readonly Color Red = Color._Red;
        protected static readonly Color Green = Color._Green;
        protected static readonly Color Blue = Color._Blue;
        public Canvas Image { get; private set; }

        public abstract void InitWorld();
        
        public void Render(SceneParameters sceneParameters)
        {
            Render( sceneParameters.CameraX, sceneParameters.CameraY, sceneParameters.CameraZ,
                    sceneParameters.LookX,   sceneParameters.LookY,   sceneParameters.LookZ,
                    sceneParameters.Height,  sceneParameters.Width);
        }
        
        public void Render(double camX, double camY, double camZ, double lookX = 0, double lookY = 0, double lookZ = 0, int h = 400, int w = 600)
        {
            Image = new Canvas(w, h);
            var point = Helper.CreatePoint(camX, camY, camZ);
            var look = Helper.CreatePoint(lookX, lookY, lookZ);
            
            var camera = new Camera(w, h, Math.PI / 3, Helper.ViewTransform(point, look, Helper.CreateVector(0, 1, 0)));
            camera.RowRendered += OnRowRendered;

            camera.Render(Image, world);
        }
        
        public string Render(string file, double camX, double camY, double camZ, double lookX=0, double lookY =0, double lookZ =0, int h=400, int w=600)
        {
            Render(camX, camY, camZ, lookX, lookY, lookZ, h, w);
            string outFilePath = Path.Combine(Path.GetTempPath(), file);
            Image.SavePPM(outFilePath);
            return outFilePath;
        }

        public IShape DefaultFloor()
        {
            var floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            world.Add(floor);
            return floor;
        }
        
        private void OnRowRendered(int y, int yMax)
        {
            RowRendered?.Invoke(y, yMax);
        }

        protected T Add<T>(T shape) where T : IShape
        {
            world.Add(shape);
            return shape;
        }

        protected void Add(params IShape[] shapes)
        {
            world.Add(shapes);
        }

        protected void Light(double x, double y, double z)
        {
            Light(x, y, z, Color.White);
        }
        
        protected void Light(double x, double y, double z, Color c)
        {
            world.Lights.Add(new PointLight(Helper.CreatePoint(x, y, z), c));
        }
    }
}