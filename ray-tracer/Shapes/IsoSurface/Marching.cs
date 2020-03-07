using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ray_tracer.Shapes.IsoSurface
{
    // thanks to https://github.com/Scrawk/Marching-Cubes
    public abstract class Marching : IMarching
    {
        private Voxel[] Cube { get; }

        /// <summary>
        /// Winding order of triangles use 2,1,0 or 0,1,2
        /// </summary>
        protected int[] WindingOrder { get; }

        protected Marching()
        {
            Cube = new Voxel[8];
            WindingOrder = new[] { 0, 1, 2 };
        }

        public void Generate(IsoSurface isoSurface, TriangleMesh mesh)
        {

            if (isoSurface.Threshold > 0.0f)
            {
                WindingOrder[0] = 0;
                WindingOrder[1] = 1;
                WindingOrder[2] = 2;
            }
            else
            {
                WindingOrder[0] = 2;
                WindingOrder[1] = 1;
                WindingOrder[2] = 0;
            }

            for (int x = 0; x < isoSurface.Width - 1; x++)
            {
                for (int y = 0; y < isoSurface.Height - 1; y++)
                {
                    for (int z = 0; z < isoSurface.Depth - 1; z++)
                    {
                        //Get the values in the 8 neighbours which make up a cube
                        for (int i = 0; i < 8; i++)
                        {
                            int ix = x + VertexOffset[i, 0];
                            int iy = y + VertexOffset[i, 1];
                            int iz = z + VertexOffset[i, 2];

                            Cube[i] = isoSurface.Voxels[ix][iy][iz];
                        }

                        //Perform algorithm
                        March(isoSurface.Threshold, isoSurface.Dx, isoSurface.Dy, isoSurface.Dz, Cube, mesh);
                    }
                }
            }
        }

        /// <summary>
        /// MarchCube performs the Marching algorithm on a single cube
        /// </summary>
        protected abstract void March(double threshold, double cx, double cy, double cz, Voxel[] cube, TriangleMesh mesh);

        /// <summary>
        /// GetOffset finds the approximate point of intersection of the surface
        /// between two points with the values v1 and v2
        /// </summary>
        protected double GetOffset(double v1, double v2, double threshold)
        {
            double delta = v2 - v1;
            return (Math.Abs(delta) < double.Epsilon) ? threshold : (threshold - v1) / delta;
        }

        /// <summary>
        /// VertexOffset lists the positions, relative to vertex0, 
        /// of each of the 8 vertices of a cube.
        /// vertexOffset[8][3]
        /// </summary>
        protected static readonly int[,] VertexOffset = {
	        {0, 0, 0},
            {1, 0, 0},
            {1, 1, 0},
            {0, 1, 0},
	        {0, 0, 1},
            {1, 0, 1},
            {1, 1, 1},
            {0, 1, 1}
	    };
    }
}
 