using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class FresnelDemo : AbstractScene
    {
        public FresnelDemo()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Width = 600, Height = 600,
                CameraX = 0, CameraY = 0, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 0,
                FieldOfView = 0.45
            });
        }

        public override void InitWorld()
        {
/*# Scene description for image on page 159 of
# "The Ray Tracer Challenge", depicting two nested
# glass spheres against a checkered background.
            #
# author: Jamis Buck <jamis@jamisbuck.org>

            - add: camera
            width: 600
            height: 600
            field-of-view: 0.45
            from: [ 0, 0, -5 ]
            to: [ 0, 0, 0 ]
            up: [ 0, 1, 0 ]

            - add: light
            intensity: [ 0.9, 0.9, 0.9 ]
            at: [ 2, 10, -5 ]

# wall
            - add: plane
            transform:
            - [ rotate-x, 1.5708 ]
            - [ translate, 0, 0, 10 ]
            material:
            pattern:
            type: checkers
            colors:
            - [ 0.15, 0.15, 0.15 ]
            - [ 0.85, 0.85, 0.85 ]
            ambient: 0.8
            diffuse: 0.2
            specular: 0

# glass ball
                - add: sphere
            material:
            color: [ 1, 1, 1 ]
            ambient: 0
            diffuse: 0
            specular: 0.9
            shininess: 300
            reflective: 0.9
            transparency: 0.9
            refractive-index: 1.5

# hollow center
                - add: sphere
            transform:
            - [ scale, 0.5, 0.5, 0.5 ]
            material:
            color: [ 1, 1, 1 ]
            ambient: 0
            diffuse: 0
            specular: 0.9
            shininess: 300
            reflective: 0.9
            transparency: 0.9
            refractive-index: 1.0000034
*/
            Light(2, 10, -5, White*0.9);
            Add(new Plane {Material = new Material(new CheckerPattern(White * 0.15, White * 0.85), ambient: 0.8, diffuse: 0.2, specular: 0)}.Rotate(rx: 1.5708).Translate(tz: 10));
            Add(new Sphere {Material = new Material(White, ambient: 0, diffuse: 0, specular: 0.9, shininess: 300, reflective: 0.9, transparency: 0.9, refractiveIndex: 1.5)});
            Add(new Sphere {Material = new Material(White, ambient: 0, diffuse: 0, specular: 0.9, shininess: 300, reflective: 0.9, transparency: 0.9, refractiveIndex: 1.0000034)}.Scale(0.5));
        }
    }
}