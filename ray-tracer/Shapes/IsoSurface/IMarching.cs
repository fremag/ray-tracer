using System.Collections.Generic;

namespace ray_tracer.Shapes.IsoSurface
{
    public interface IMarching
    {
        double Surface { get; set; }
        void Generate(IList<double> voxels, int width, int height, int depth, IList<Tuple> verts, IList<int> indices);
    }
}