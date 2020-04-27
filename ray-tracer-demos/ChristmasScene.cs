using System;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;
using ray_tracer.Shapes.TriangleGroup;

namespace ray_tracer_demos
{
    public class ChristmasScene : AbstractScene
    {
        public ChristmasScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 2*400, Height = 2*300,
                CameraX = 0, CameraY = 0, CameraZ = -4,
                LookX = 0, LookY = 0, LookZ = 0,
                FieldOfView = 1.047
            });
        }

        public override void InitWorld()
        {
            SetupLights();
//#if TOTO
            Add(new Sphere
            {
                Material = new Material(new Color(1, 0.25, 0.25)) {Ambient = 0, Specular = 0, Diffuse = 0.5, Reflective = 0.5}
            });
            Add(new Cylinder(0, 1, true)
            {
                Material = new Material(new CheckerPattern(White, White * 0.94))
                {
                    Ambient = 0.02, Diffuse = 0.7, Specular = 0.8, Shininess = 20, Reflective = 0.05
                }
            }.Scale(0.2, 0.3, 0.2).Translate(0, 0.9, 0).Rotate(rz: -0.1));
//#endif
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(rx: -1.5708).Rotate(ry: 0.349).Translate(-1, -1, 0));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708).Rotate(ry:  0.349).Translate(-1, -1, 0));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708).Rotate(ry:  0.349).Translate(-1, 1, 0));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708).Rotate(ry:  -0.1745).Translate(1, -1, 0));
            
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708).Rotate(ry: -0.349).Translate(1, 1, 0));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708).Rotate(ry:  -0.349).Translate(0.2, -1.25, 0));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708).Rotate(ry:  0.349).Translate(-0.2, -1.25, 0));
            
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708+0.087).Rotate(ry:  0.5236).Translate(-1.2, 0.1, 0));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708-0.1745).Rotate(ry:  0.5236).Translate(-1.2, -0.35, 0.5));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708+0.087).Rotate(ry:  -0.5236).Translate(-0.2, 1.5, 0.25));
            
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708-0.087).Rotate(ry:  -0.5236).Translate(1.3, 0.4, 0));
            Add(FirBranch().Translate(0, -0.5, 0).Rotate(-1.5708+0.087).Rotate(ry:  -0.1745).Translate(1.5, -0.4, 0));
        }

        private IShape FirBranch()
        {
            var length = 2.0;
            var radius = 0.025;
            var segments = 20;
            var perSegment = 24;
            var branch = new Cylinder(0, length, true)
            {
                Material = new Material(new Color(0.5, 0.35, 0.26))
                {
                    Ambient = 0.2, Specular = 0, Diffuse = 0.6
                }
            }.Scale(radius, 1, radius);

            var segSize = length / (segments - 1);
            var theta = 2.1 * Pi / perSegment;
            var maxLength = 20 * radius;
            
            var g = new Group();
            g.Add(branch);

            Random rand = new Random(0);

            var material = new Material(new Color(0.26, 0.36, 0.16)) {Specular = 0.1};
            for (var y = 0; y < segments; y++)
            {
                var subGroup = new TriangleGroupAvx();
                for (var i = 0; i < perSegment; i++)
                {
                    var yBase = segSize * y + rand.NextDouble() * segSize;
                    var yTip = yBase - rand.NextDouble() * segSize;
                    var yAngle = i * theta + rand.NextDouble() * theta;
                    var needleLength = maxLength / 2 * (1 + rand.NextDouble());
                    var ofs = radius / 2;

                    var p1 = Helper.CreatePoint(ofs, yBase, ofs);
                    var p2 = Helper.CreatePoint(-ofs, yBase, ofs);
                    var p3 = Helper.CreatePoint(0, yTip, needleLength);
                    var triangle = new Triangle(p1, p2, p3).Rotate(0, yAngle, 0);
                    triangle.Material = material;
                    subGroup.Add(triangle);
                }

                g.Add(subGroup);
            }
            
            return g;
        }
        
        private void SetupLights()
        {
            Light(-10, 10, -10, White * 0.6);
            Add(new Sphere
            {
                HasShadow = false,
                Material = new Material(White)
                {
                    Ambient = 0.6, Diffuse = 0, Specular = 0
                }
            }.Scale(1.5).Translate(-10, 10, -10));

            Light(10, 10, -10, White * 0.6);
            Add(new Sphere
            {
                HasShadow = false, 
                Material = new Material(White)
                {
                    Ambient = 0.6, Diffuse = 0, Specular = 0
                }
            }.Scale(1.5).Translate(10, 10, -10));

            Light(-2, 1, -6, new Color(0.2, 0.1, 0.1));
            Add(new Sphere
            {
                HasShadow = false, 
                Material = new Material(new Color(1, 0.5, 0.5))
                {
                    Ambient = 0.6, Diffuse = 0, Specular = 0
                }
            }.Scale(0.4).Translate(-2, 1, -6));

            Light(-1, -2, -6, new Color(0.1, 0.2, 0.1));
            Add(new Sphere
            {
                HasShadow = false, 
                Material = new Material(new Color(0.5, 1, 0.5))
                {
                    Ambient = 0.6, Diffuse = 0, Specular = 0
                }
            }.Scale(0.4).Translate(-1, -2, -6));

            Light(3, -1, -6, new Color(0.2, 0.2, 0.2));
            Add(new Sphere
            {
                HasShadow = false, 
                Material = new Material(White)
                {
                    Ambient = 0.6, Diffuse = 0, Specular = 0
                }
            }.Scale(0.5).Translate(3, -1, -6));
        }
    }
}