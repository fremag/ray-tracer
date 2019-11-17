using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{

    /*
     * Thanks to Javan Makhmali (https://github.com/javan)
     * 
     * I wanted to check my ray tracer was correct so I got the same scene as him to compare results.
     * https://github.com/javan/ray-tracer-challenge/blob/master/src/controllers/chapter_11_worker.js
     */
    public class WorldReflectionRefractionScene : AbstractScene
    {
        public override void InitWorld()
        {
            IShape floor = new Plane();
            floor.Material = new Material(new CheckerPattern(Color.White * 0.35, Color.White * 0.65)
            {
                Transform = Helper.RotationY(45)
            }, reflective: 0.4, specular: 0);

            Material material = new Material(new Color(1, 0.3, 0.2), specular: 0.4, shininess: 5);
            var s1 = new Sphere {Material = material, Transform = Helper.Translation(6, 1, 4)};
            var s2 = new Sphere {Material = material, Transform = Helper.Translation(2, 1, 3)};
            var s3 = new Sphere {Material = material, Transform = Helper.Translation(-1, 1, 2)};
            var blueSphere = new Sphere
            {
                Material = new Material(new Color(0, 0, 0.2), ambient: 0, diffuse: 0.4, specular: 0.9, shininess: 300, reflective: 0.9, transparency: 0.9, refractiveIndex: 1.5),
                Transform = Helper.Translation(0.6, 0.7, -0.6) * Helper.Scaling(0.7, 0.7, 0.7)
            };
            var greenSphere = new Sphere
            {
                Material = new Material(new Color(0, 0.2, 0), ambient: 0, diffuse: 0.4, specular: 0.9, shininess: 300, reflective: 0.9, transparency: 0.9, refractiveIndex: 1.5),
                Transform = Helper.Translation(-0.7, 0.5, -0.8) * Helper.Scaling(0.5, 0.5, 0.5)
            };
            Add(new[] {floor, s1, s2, s3, blueSphere, greenSphere});
            Light(-4.9, 4.9, -1);
        }
    }
}