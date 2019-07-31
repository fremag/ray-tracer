using System;

namespace ray_tracer
{
    public abstract class AbstractShape : IShape
    {
        public Matrix Transform { get; set; } = Helper.CreateIdentity();
        public Material Material { get; set; } = new Material();

        public abstract Intersections Intersect(Ray ray);
        public abstract Tuple NormalAt(Tuple worldPoint);
    }
}