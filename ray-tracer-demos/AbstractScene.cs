using System;
using System.Collections.Generic;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using Tuple = ray_tracer.Tuple;

namespace ray_tracer_demos
{
    public abstract class AbstractScene
    {
        public World World { get; } = new World();
        protected static readonly double Pi = Math.PI;
        protected static readonly Color Red = Color._Red;
        protected static readonly Color Green = Color._Green;
        protected static readonly Color Blue = Color._Blue;

        public List<CameraParameters> CameraParameters { get; set; } = new List<CameraParameters>(); 
        public abstract void InitWorld();

        protected AbstractScene()
        {
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400, CameraX = 0, CameraY = 1, CameraZ = -1, LookX = 0, LookY = 0, LookZ = 0});
        }

        public IShape DefaultFloor()
        {
            var floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            World.Add(floor);
            return floor;
        }

        protected T Add<T>(T shape) where T : IShape
        {
            World.Add(shape);
            return shape;
        }

        protected void Add(params IShape[] shapes)
        {
            World.Add(shapes);
        }

        protected void Light(Tuple position)
        {
            Light(position.X, position.Y, position.Z);
        }
        
        protected void Light(double x, double y, double z)
        {
            Light(x, y, z, Color.White);
        }
        
        protected void Light(double x, double y, double z, Color c)
        {
            World.Lights.Add(new PointLight(Helper.CreatePoint(x, y, z), c));
        }
    }
}