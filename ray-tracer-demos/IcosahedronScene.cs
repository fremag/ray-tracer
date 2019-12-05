using System;
using ray_tracer;
using ray_tracer.Patterns;
using Tuple = ray_tracer.Tuple;

namespace ray_tracer_demos
{
    public class IcosahedronScene : AbstractScene
    {
        public IcosahedronScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 480,
                CameraX = 4, CameraY = 1, CameraZ = -7,
                LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            Light(-10, 10, 0);
            Light(0, 0, 0);
            var phi = (1 + Math.Sqrt(5)) / 2;
            double a = 1;
            double r1 = 0.2;
            double r2 = 0.1;

            Tuple[] vertices = {
                Helper.CreatePoint( a / 2, a / 2 * phi, 0),
                Helper.CreatePoint(-a / 2, a / 2 * phi, 0),
                Helper.CreatePoint( a / 2, -a / 2 * phi, 0),
                Helper.CreatePoint(-a / 2, -a / 2 * phi, 0),
                Helper.CreatePoint(0,  a / 2, a / 2 * phi),
                Helper.CreatePoint(0, -a / 2, a / 2 * phi),
                Helper.CreatePoint(0,  a / 2, -a / 2 * phi),
                Helper.CreatePoint(0, -a / 2, -a / 2 * phi),
                Helper.CreatePoint( a / 2 * phi, 0,  a / 2),
                Helper.CreatePoint(a / 2 * phi, 0, -a / 2),
                Helper.CreatePoint(-a / 2 * phi, 0,  a / 2),
                Helper.CreatePoint(-a / 2 * phi, 0, -a / 2)
            };
            
            Color[] colors =
            {
                new Color(1,1,1),
                new Color(1,1,1),
                new Color(1,1,1),
                new Color(1,1,1),
                new Color(1,1,1),
                new Color(1,1,1),
                new Color(1,0,0),
                new Color(0,1,0),
                new Color(1,1,1),
                new Color(1,1,1),
                new Color(1,1,1),
                new Color(1,1,1)
            };
            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                var sphere = Helper.Sphere();
                sphere.Scale(r1).Translate(vertex*2);
                sphere.Material = new Material(new SolidPattern(colors[i]));
                Add(sphere);

                for (var j = i+1; j < vertices.Length; j++)
                {
                    var vertexB = vertices[j % vertices.Length];
                    var dist = (vertexB - vertex).Magnitude;
                
                    if (Math.Abs(dist - a) < Helper.Epsilon)
                    {
                        var cyl = Helper.Cylinder(vertex, vertexB, r2);
                        Add(cyl);
                    }
                }
            }
        }
    }
}