using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class BlobScene : AbstractScene
    {
        public BlobScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters
            {
                Name = "Default", Width = 1600, Height = 1200,
                CameraX = 0, CameraY = 2, CameraZ = -5,
                LookX = 0, LookY = 0, LookZ = 0
            });
        }

        public override void InitWorld()
        {
            DefaultFloor(Brown, Yellow);
            Light(-10, 10, -10);
            var blob = new Blob {Size = 0.5};
            blob.Points.Add(Helper.CreatePoint(0, 1, 0));
            blob.Points.Add(Helper.CreatePoint(0, 0, 0));
//            blob.Points.Add(Helper.CreatePoint(1, -1, 0));
            Add(blob);
        }
    }
}