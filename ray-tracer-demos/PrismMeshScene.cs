using System;
using System.Collections.Generic;
using System.Linq;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using ray_tracer.Shapes.Mesh;
using ray_tracer.Triangulation;
using Tuple = ray_tracer.Tuple;

namespace ray_tracer_demos
{
    public class PrismMeshScene : AbstractScene
    {
        public PrismMeshScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 1, CameraZ = -1,
                LookX = 0, LookY = 0, LookZ = 0});
        }

        public override void InitWorld()
        {
            DefaultFloor();
            Light(1, 1, -1);

            Add(new Prism(BuildPolygon(3)).Scale(0.2, 0.25, 0.2).Translate(tx: -0.75, tz: 0));
            Add(new Prism(BuildPolygon(4)).Scale(0.2, 0.25, 0.2).Translate(tx: -0, tz: 0.75));
            Add(new Prism(BuildPolygon(5)).Scale(0.2, 0.25, 0.2).Translate(tx: 0.75, tz: 0.5));
            var hex = new Prism(BuildPolygon(6));
            var diff = new CsgDifference(hex, new Cylinder().Scale(0.5));
            Add(diff.Scale(0.2, 0.25, 0.2).Translate(tx: 0.5, tz: -0.5));

            IEnumerable<Point2D> points = new[]
            {
                new Point2D(-3, 5), 
                new Point2D(3, 5),
                new Point2D(3, 3),
                new Point2D(-1, 3), 
                new Point2D(-1, 1),
                new Point2D(1, 1),

                new Point2D(1, -1),
                new Point2D(-1, -1),
                new Point2D(-1, -3),
                new Point2D(3, -3),
                new Point2D(3, -5),
                new Point2D(-3, -5)
            }.Reverse();

            var letterE = new Prism(points);
            Add(letterE.Scale(0.1).Rotate(ry: Math.PI/4));
        }

        private IEnumerable<Point2D> BuildPolygon(int n)
        {
            for(int i=0; i < n; i++) 
            {
                var x = Math.Cos(2 * Math.PI * i / n);
                var y = Math.Sin(2 * Math.PI * i / n);
                yield return new Point2D(x, y);
            }
        }
    }
}