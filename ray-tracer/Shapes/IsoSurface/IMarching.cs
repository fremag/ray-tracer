using System.Collections.Generic;

namespace ray_tracer.Shapes.IsoSurface
{
    public interface IMarching
    {
        void Generate(IsoSurface isoSurface, IList<Tuple> verts, IList<int> indices);
    }
}