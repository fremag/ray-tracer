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

        public virtual void Generate(IsoSurface isoSurface, IList<Tuple> vertices, IList<Triplet> triplets)
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
                        March(isoSurface.Threshold, isoSurface.Dx, isoSurface.Dy, isoSurface.Dz, Cube, vertices, triplets);
                    }
                }
            }

            Compress(vertices, triplets);
        }

        private void Compress(IList<Tuple> vertices, IList<Triplet> triplets)
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool[] merged = new bool[vertices.Count];
            
            for (int i = 0; i < vertices.Count-1; i++)
            {
                if (merged[i])
                {
                    continue;
                }

                var p1 = vertices[i];
                
                for (int j = i+1; j < vertices.Count; j++)
                {
                    if (merged[j])
                    {
                        continue;
                    }
                    var p2 = vertices[j];
                    var x = (p1.X-p2.X);
                    var y = (p1.Y-p2.Y);
                    var z = (p1.Z-p2.Z);
                    var dist =  x * x + y * y + z * z;
                    
                    if ( dist < 1e-10)
                    {
                        merged[j] = true;
                        //Console.WriteLine($"Merge ! {i} {j}");
                        for (int k = 0; k < triplets.Count; k++)
                        {
                            if (triplets[k].Index0 == j)
                            {
                                triplets[k].Index0 = i;
                            }
                            if (triplets[k].Index1 == j)
                            {
                                triplets[k].Index1 = i;
                            }
                            if (triplets[k].Index2 == j)
                            {
                                triplets[k].Index2 = i;
                            }
                        }
                    }
                }
            }
            Console.WriteLine("T: "+sw.ElapsedMilliseconds+"ms, Verts: "+vertices.Count+", Merged: "+merged.Count(b => b)+", N: "+triplets.Distinct().Count() );
        }

        private double Dist(ref Tuple p1, ref Tuple p2)
        {
            var x = (p1.X-p2.X);
            var y = (p1.Y-p2.Y);
            var z = (p1.Z-p2.Z);
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// MarchCube performs the Marching algorithm on a single cube
        /// </summary>
        protected abstract void March(double threshold, double cx, double cy, double cz, Voxel[] cube, IList<Tuple> vertices, IList<Triplet> triplets);

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
 