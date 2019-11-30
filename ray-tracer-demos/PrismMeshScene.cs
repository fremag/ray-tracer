using System;
using System.Collections.Generic;
using System.Linq;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes.Mesh;
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

            Add(BuildPolygon(3).Scale(0.2, 0.25, 0.2).Translate(tx: -0.75, tz: 0));
            Add(BuildPolygon(4).Scale(0.2, 0.25, 0.2).Translate(tx: -0, tz: 0.75));
            Add(BuildPolygon(5).Scale(0.2, 0.25, 0.2).Translate(tx: 0.75, tz: 0.5));
            Add(BuildPolygon(6).Scale(0.2, 0.25, 0.2).Translate(tx: 0.5, tz: -0.5));

            IEnumerable<Tuple> points = new Tuple[]
            {
                Helper.CreatePoint(-3, 0, 5),
                Helper.CreatePoint(3, 0, 5),
                Helper.CreatePoint(3, 0, 3),
                Helper.CreatePoint(-1, 0, 3),
                Helper.CreatePoint(-1, 0, 1),
                Helper.CreatePoint(1, 0, 1),

                Helper.CreatePoint(1, 0, -1),
                Helper.CreatePoint(-1, 0, -1),
                Helper.CreatePoint(-1, 0, -3),
                Helper.CreatePoint(3, 0, -3),
                Helper.CreatePoint(3, 0, -5),
                Helper.CreatePoint(-3, 0, -5)
            };
            
            var mesh = new PrismMesh(points);
            var triangleMeshFactory = new TriangleMeshFactory(false);
            var letterE = (triangleMeshFactory.Build(mesh).Scale(0.1).Rotate(ry: Math.PI/4));
            letterE.Material.Pattern = new SolidPattern(Color._Blue);
            Add(letterE);
        }

        private IShape BuildPolygon(int n)
        {
            var points = Enumerable.Range(0, n).Select(i =>
            {
                var x = Math.Cos(2 * Math.PI * i / n);
                var z = Math.Sin(2 * Math.PI * i / n);
                return Helper.CreatePoint(x, 0, z);
            }).ToArray();
            var mesh = new PrismMesh(points, true);
            var triangleMeshFactory = new TriangleMeshFactory(false);
            var prism =  triangleMeshFactory.Build(mesh);
            return prism;
        }
    }
}