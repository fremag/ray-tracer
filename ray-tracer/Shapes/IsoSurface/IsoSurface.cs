using System;

namespace ray_tracer.Shapes.IsoSurface
{
    public class IsoSurface
    {
        public Voxel[][][] Voxels { get; set; }
        public double XMin { get; set; }
        public double YMin { get; set; }
        public double ZMin { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }
        public double XMax { get; set; }
        public double YMax { get; set; }
        public double ZMax { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public double Threshold { get; set; }

        public void Init(Func<double, double, double, double> func)
        {
            Voxels = new Voxel[Width][][];
            for (int i = 0; i < Width; i++)
            {
                Voxels[i] = new Voxel[Height][];
                for (int j = 0; j < Height; j++)
                {
                    Voxels[i][j] = new Voxel[Depth];
                }
            }

            Dx = (XMax - XMin) / Width;
            Dy = (YMax - YMin) / Height;
            Dz = (ZMax - ZMin) / Depth;

            InitVoxels(func);
        }

        public IShape GetShape(bool smooth, double compressDistance)
        {
            var marching = new MarchingCubes();
            var mesh = new TriangleMesh();
            marching.Generate(this, mesh);
            var g = new Group();
            mesh.Compress(compressDistance);
            if (smooth)
            {
                foreach (var shape in mesh.GenerateSmoothTriangles())
                {
                    g.Add(shape);
                }
            }
            else
            {
                foreach (var shape in mesh.GenerateTriangles())
                {
                    g.Add(shape);
                }
            }

            return g;
        }

        private void InitVoxels(Func<double, double, double, double> func)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    for (int k = 0; k < Depth; k++)
                    {
                        double x = XMin + i * Dx;
                        double y = YMin + j * Dy;
                        double z = ZMin + k * Dz;

                        var value = func(x, y, z);
                        Voxels[i][j][k] = new Voxel {X = x, Y = y, Z = z, Value = value};
                    }
                }
            }
        }
    }
}