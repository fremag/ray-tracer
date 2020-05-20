using System;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;
using ray_tracer.Shapes.Mesh;

namespace ray_tracer_demos
{
    public class AmbiguousCylinderScene : AbstractScene
    {
        public AmbiguousCylinderScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 1600, Height = 1200,
                CameraX = 0, CameraY = 8, CameraZ = -7,
                LookX = -0.4, LookY = 1, LookZ = 0.25
            });
        }

        public override void InitWorld()
        {
            Light(100, 150, -150, White/10);
            Light(-100, 300, -150, White);
            var floor = DefaultFloor();
            floor.Material.Reflective = 0;
            floor.Material.Ambient = 0;
            floor.Material.Specular = 0;
            
            var wall = new Plane().Rotate(rx: Pi/2-0.175).Translate(tz:2);
            wall.Material = new Material(White, reflective: 1);
            Add(wall);

            var mesh = new SurfaceMesh(60, 60);
            // thanks to TJ Wei ( https://www.youtube.com/channel/UCLA68RSY6peX50b-Dw8mEpQ)
            // cf https://www.youtube.com/watch?v=vUz4kov3K1c
            // Surface[cos(s) + sgn(s - π / 2) (abs(sin(s)) - 1), 2sin(s), cos(s) - sgn(s - π / 2) (abs(sin(s)) - 1) + d, s, (-π) / 2, π + π / 2, d, 0.5, 3]

            double Interp(double t, double t0, double t1) => t0 + (t1 - t0) * t; 
            double FuncD(double t) => Interp(t, 0.5, 3);
            double FuncS(double t) => Interp(t, -Pi/2, 3*Pi/2);
            
            double FuncX(double u, double v) => Math.Cos(FuncS(u)) + Math.Sign(FuncS(u) - Pi / 2) * (Math.Abs(Math.Sin(FuncS(u))) - 1);
            double FuncZ(double u, double v) => 2*Math.Sin(FuncS(u));
            double FuncY(double u, double v) => Math.Cos(FuncS(u)) - Math.Sign(FuncS(u) - Pi / 2) * (Math.Abs(Math.Sin(FuncS(u))) - 1) + FuncD(v);
            
            mesh.Build(FuncX, FuncY, FuncZ);
            var factory = new TriangleMeshFactory();
            var surf = factory.Build(mesh);
            surf.HasShadow = false;
            Add(surf.Rotate(ry: Pi/2+0.0).Translate(tz: -2));

            Add(new Cube{Material = new Material(Blue)}.Scale(sx: 2).Rotate(ry: Pi / 4).Translate(tx: -4.5, tz: -2));
            Add(new Sphere{Material = new Material(Red)}.Scale(sx: 2).Rotate(ry: -Pi / 4).Translate(tx: 4.5, tz: -2));
        }
    }
}