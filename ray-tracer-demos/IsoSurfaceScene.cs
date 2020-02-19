using System;
using System.Collections.Generic;
using ray_tracer;
using ray_tracer.Cameras;
using ray_tracer.Shapes;
using ray_tracer.Shapes.IsoSurface;
using Tuple = ray_tracer.Tuple;

namespace ray_tracer_demos
{
    public class IsoSurfaceScene : AbstractScene
    {
        public IsoSurfaceScene()
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
            int width = 16;
            int height = 16;
            int length = 16;
            double xMin = -2;
            double xMax =  2;
            double yMin = -2;
            double yMax =  2;
            double zMin = -2;
            double zMax =  2;
            
            double[] voxels = new double[width * height * length];
            double dx = (xMax - xMin) / width;
            double dy = (yMax - yMin) / height;
            double dz = (zMax - zMin) / length;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        double fx = xMin + x * dx;
                        double fy = yMin + y * dy;
                        double fz = zMin + z * dz;

                        int idx = x + y * width + z * width * height;

                        var voxel = Math.Sqrt( fx*fx+fy*fy+fz*fz);
                        voxels[idx] = voxel;
                    }
                }
            }

            var marching = new MarchingCubes
            {
                Surface = 1
            };
            IList<int> indices = new List<int>(2<<16);
            IList<Tuple> verts = new List<Tuple>(2<<16);
            marching.Generate(voxels, width, height, length, verts, indices);
            Group g = new Group();
            int nbTriangles = indices.Count / 3;
            for (int i = 0; i < nbTriangles; i++)
            {
                int i1 = indices[3 * i];
                int i2 = indices[3 * i+1];
                int i3 = indices[3 * i+2];
                Tuple p1 = verts[i1];
                Tuple p2 = verts[i2];
                Tuple p3 = verts[i3];
                var triangle = new Triangle(p1, p2, p3);
                g.Add(triangle);
            }

            Add(g.Scale(dx));
//           Add(g.Translate(xMin-xMax, yMin-yMax, zMin-zMax));
            Add(new Sphere());
        }
    }
}