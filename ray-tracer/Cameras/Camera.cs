using System;

namespace ray_tracer.Cameras
{
    public class Camera : AbstractCamera
    {
        public double FieldOfView { get; }
        public double PixelSize { get; }
        public double HalfHeight { get; }
        public double HalfWidth { get; }

        private Matrix InverseTransform { get; }

        public Camera(int hSize, int vSize, double fieldOfView) :
            this(hSize, vSize, fieldOfView, Helper.CreateIdentity())
        {
        }

        public Camera(int hSize, int vSize, double fieldOfView, Matrix transform) 
            : base(hSize, vSize, transform)
        {
            FieldOfView = fieldOfView;
            var halfView = Math.Tan(fieldOfView / 2);
            var aspect = (double) HSize / VSize;
            if (aspect >= 1)
            {
                HalfWidth = halfView;
                HalfHeight = halfView / aspect;
            }
            else
            {
                HalfWidth = halfView * aspect;
                HalfHeight = halfView;
            }

            PixelSize = HalfWidth * 2 / HSize;

            InverseTransform = Transform.Invert();
        }

        public override Ray RayForPixel(int px, int py)
        {
            var pixel = GetPixel(px, py);
            var origin = InverseTransform * Helper.CreatePoint(0, 0, 0);
            var direction = (pixel - origin).Normalize();
            var ray =  Helper.Ray(origin, direction);

            return ray;
        }

        private Tuple GetPixel(in int px, in int py)
        {
// the offset from the edge of the canvas to the pixel's center
            var xOffset = (px + 0.5) * PixelSize;
            var yOffset = (py + 0.5) * PixelSize;
// the untransformed coordinates of the pixel in world space.
// (remember that the camera looks toward -z, so +x is to the *left*.)
            var worldX = HalfWidth - xOffset;
            var worldY = HalfHeight - yOffset;
// using the camera matrix, transform the canvas point and the origin,
// and then compute the ray's direction vector.
// (remember that the canvas is at z=-1)
            var pixel = InverseTransform * Helper.CreatePoint(worldX, worldY, -1);
            return pixel;
        }
    }
}