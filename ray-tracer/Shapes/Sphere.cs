using System;

namespace ray_tracer.Shapes
{
    public class Sphere : AbstractShape
    {
        public override Intersections Intersect(Ray ray)
        {
            var transformedRay = ray.Transform(Transform.Inverse());
            var sphereToRay = transformedRay.Origin - Helper.CreatePoint(0, 0, 0);
            var a = transformedRay.Direction.DotProduct(transformedRay.Direction);
            var b = 2 * transformedRay.Direction.DotProduct(sphereToRay);
            var c = sphereToRay.DotProduct(sphereToRay) - 1;
            var discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                return Helper.Intersections();
            }

            var t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            var t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);

            return Helper.Intersections(
                new Intersection(t1, this),
                new Intersection(t2, this)
            );
        }

        public override Tuple NormalAt(Tuple worldPoint)
        {
            var inverseTransform = Transform.Inverse();
            var objectPoint = inverseTransform * worldPoint;
            var objectNormal = objectPoint - Helper.CreatePoint(0, 0, 0);
            var worldNormal = inverseTransform.Transpose() * objectNormal;
            worldNormal = Helper.CreateVector(worldNormal.X, worldNormal.Y, worldNormal.Z);
            return worldNormal.Normalize();
        }        
    }
}