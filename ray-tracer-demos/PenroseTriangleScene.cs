using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class PenroseTriangleScene : AbstractScene
    {
        // thanks to them:
        // http://www.f-lohmueller.de/pov_tut/x_sam/sam_430f.htm
        // http://www.f-lohmueller.de/pov_tut/camera_light/camera_f1.htm
        // https://github.com/POV-Ray/povray/blob/2d35fd4b36ad19e3c17273101f8f71a2ce7924aa/source/core/render/tracepixel.cpp#L375
        // http://www.cs.cornell.edu/courses/cs4620/2013fa/lectures/02view-ray.pdf
        
        public PenroseTriangleScene()
        {
            CameraParameters.Add(new OrthographicCameraParameters(0, 3.5, 0, 3.5, 0, 0)
            {
                Name = "Default", Width = 1000, Height = 1000,
                CameraX = -2, CameraY = -1.5, CameraZ = -4,
                LookX = 0, LookY = 0, LookZ = 1
            });
        }

        public override void InitWorld()
        {
            Light(10, -20, -10);
            var length = 2;
            var diameter = 0.25;
            var r = diameter / 2;
            var l = length - 2 * r;

            IShape Element() => new Cube {HasShadow = false}.Scale(r+l/2, r, r).Translate(tx: l/2);
            IShape ElementCut() => new CsgDifference(Element(), new Cube().Scale(sy: 1, sz: 10).Translate(tx: 1).Rotate(ry: Pi/4).Translate(tx: l/2)) {HasShadow = false};
            
            Group penroseTriangle = new Group {HasShadow = false}
            .Add(ElementCut().Rotate(0, -Pi/2, 0))
            .Add(Element().Rotate(0,0,Pi/2))
            .Add(Element().Rotate(0,0,0).Translate(0,l,0))
            .Add(ElementCut().Rotate(0, Pi/2, 0).Translate(l, l, 0));

            Add(penroseTriangle
                .Translate(tx: -l/2, ty: -l/2)
                .RotateDeg(-35.5,45.5,0)
                );
        }
    }
}