using System;
using System.Threading;
using System.Threading.Tasks;

namespace ray_tracer
{
    public class Camera
    {
        public int HSize { get; }
        public int VSize { get; }
        public double FieldOfView { get; }
        public Matrix Transform { get; }
        public double PixelSize { get; }
        public double HalfHeight { get; }
        public double HalfWidth { get; }

        private Matrix InverseTransform { get; }

        public Camera(int hSize, int vSize, double fieldOfView) :
            this(hSize, vSize, fieldOfView, Helper.CreateIdentity())
        {
        }

        public Camera(int hSize, int vSize, double fieldOfView, Matrix transform)
        {
            HSize = hSize;
            VSize = vSize;
            FieldOfView = fieldOfView;
            Transform = transform;

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

            InverseTransform = Transform.Inverse();
        }

        public Ray RayForPixel(int px, int py)
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
            var origin = InverseTransform * Helper.CreatePoint(0, 0, 0);
            var direction = (pixel - origin).Normalize();
            return Helper.Ray(origin, direction);
        }

        public event Action<int, int> RowRendered;
        
        public Canvas Render(World world, int maxRecursion = 10)
        {
            var image = new Canvas(HSize, VSize);
            ThreadPool.SetMinThreads(4, 8); 
            int progress = 0;
            Parallel.For(0, VSize, y =>
            {
                for (int x = 0; x < HSize; x++)
                {
                    var ray = RayForPixel(x, y);
                    var color = world.ColorAt(ray, maxRecursion);
                    image.SetPixel(x, y, color);
                }

                Interlocked.Increment(ref progress);
                RowRendered?.Invoke(progress, VSize);
            });

            return image;
        }
    }
}