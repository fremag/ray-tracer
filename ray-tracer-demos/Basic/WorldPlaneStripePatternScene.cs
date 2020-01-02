using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos.Basic
{
    public class WorldPlaneStripePatternScene : AbstractScene
    {
        public WorldPlaneStripePatternScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 0, CameraY = 3, CameraZ = -5,
                LookX = 0, LookY = 1, LookZ = 0});
        }

        public override void InitWorld()
        {
            IShape floor = new Plane();
            floor.Material = new Material(new RingPattern(Color.White, new Color(0, 0, 1)), specular: 0);
            floor.Transform = Helper.Translation(0, 0.5, 0);
            var middle = Helper.Sphere();
            middle.Transform = Helper.Translation(-0.5, 1, 0.5);
            middle.Material = new Material(new StripePattern(new Color(0.5, 1, 0.1), new Color(1, 0.5, 0.1)) {Transform = Helper.Scaling(0.1, 1, 1)}, diffuse: 0.7, specular: 0.3);

            var right = Helper.Sphere();
            right.Transform = Helper.Translation(1.5, 0.5, -0.5) * Helper.Scaling(0.5, 0.5, 0.5);
            right.Material = new Material(new GradientPattern(new Color(0, 0, 1), new Color(1, 0.5, 0.1)) {Transform = Helper.RotationY(30) * Helper.Scaling(0.25, 1, 1)}, diffuse: 0.7, specular: 1);

            var left = Helper.Sphere();
            left.Transform = Helper.Translation(-1.5, 0.33, -0.75) * Helper.Scaling(0.33, 0.33, 0.33);
            left.Material = new Material(new CheckerPattern(new Color(1, 0, 0), new Color(0, 1, 0)) {Transform = Helper.Scaling(0.25, 0.25, 0.25)}, diffuse: 0.7, specular: 0.3);

            Add(new[] {floor, middle, left, right});
            Light(-10, 10, -10);
        }
    }
}