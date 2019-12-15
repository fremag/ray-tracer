using System;

namespace ray_tracer.Shapes
{
    public class Icosahedron : Group
    {
        public Icosahedron(double radius, double rCylinder)
        {
            var phi = (1 + Math.Sqrt(5)) / 2;

            Tuple[] vertices = {
                Helper.CreatePoint( radius / 2, radius / 2 * phi, 0),
                Helper.CreatePoint(-radius / 2, radius / 2 * phi, 0),
                Helper.CreatePoint( radius / 2, -radius / 2 * phi, 0),
                Helper.CreatePoint(-radius / 2, -radius / 2 * phi, 0),
                Helper.CreatePoint(0,  radius / 2, radius / 2 * phi),
                Helper.CreatePoint(0, -radius / 2, radius / 2 * phi),
                Helper.CreatePoint(0,  radius / 2, -radius / 2 * phi),
                Helper.CreatePoint(0, -radius / 2, -radius / 2 * phi),
                Helper.CreatePoint( radius / 2 * phi, 0,  radius / 2),
                Helper.CreatePoint(radius / 2 * phi, 0, -radius / 2),
                Helper.CreatePoint(-radius / 2 * phi, 0,  radius / 2),
                Helper.CreatePoint(-radius / 2 * phi, 0, -radius / 2)
            };

            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];

                for (var j = i+1; j < vertices.Length; j++)
                {
                    var vertexB = vertices[j % vertices.Length];
                    var dist = (vertexB - vertex).Magnitude;
                
                    if (Math.Abs(dist - radius) < Helper.Epsilon)
                    {
                        var cyl = Helper.Cylinder(vertex, vertexB, rCylinder);
                        Add(cyl);
                    }
                }
            }
            
        }
    }
}