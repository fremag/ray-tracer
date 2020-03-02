using System;
using System.Collections.Generic;
using System.Linq;

namespace ray_tracer.Shapes.IsoSurface
{
    public class MarchingCubes : Marching
    {
        private Tuple[] EdgeVertex { get; }

        public MarchingCubes()
        {
            EdgeVertex = new Tuple[EdgeConnection.Length];
        }

        /// <summary>
        /// MarchCube performs the Marching Cubes algorithm on a single cube
        /// </summary>
        protected override void March(double threshold, double cx, double cy, double cz, Voxel[] cube, IList<Tuple> vertList, IList<Triplet> indexList)
        {
            //Find which vertices are inside of the surface and which are outside
            int flagIndex = 0;
            for (int i = 0; i < cube.Length; i++)
            {
                if (cube[i].Value <= threshold)
                {
                    flagIndex |= 1 << i;
                }
            }

            //Find which edges are intersected by the surface
            int edgeFlags = CubeEdgeFlags[flagIndex];

            //If the cube is entirely inside or outside of the surface, then there will be no intersections
            if (edgeFlags == 0) return;

            double x0 = cube[0].X;
            double y0 = cube[0].Y;
            double z0 = cube[0].Z;

            //Find the point of intersection of the surface with each edge
            for (int i = 0; i < EdgeVertex.Length; i++)
            {
                //if there is an intersection on this edge
                if ((edgeFlags & (1 << i)) != 0)
                {
                    var index0 = EdgeConnection[i, 0];
                    var index1 = EdgeConnection[i, 1];
                    var voxel0 = cube[index0];
                    var voxel1 = cube[index1];

                    double v0 = voxel0.Value;
                    double v1 = voxel1.Value;
                    var offset = GetOffset(v0, v1, threshold);

                    var dx = VertexOffset[index0, 0];
                    var dy = VertexOffset[index0, 1];
                    var dz = VertexOffset[index0, 2];

                    var xx = x0 + cx * (dx + offset * EdgeDirection[i, 0]);
                    var yy = y0 + cy * (dy + offset * EdgeDirection[i, 1]);
                    var zz = z0 + cz * (dz + offset * EdgeDirection[i, 2]);
                    const int nbDigits = 8;
                    EdgeVertex[i] = Helper.CreatePoint(Math.Round(xx, nbDigits), Math.Round(yy, nbDigits), Math.Round(zz, nbDigits));
                }
            }

            //Save the triangles that were found. There can be up to five per cube
            var triangles = TriangleConnectionTable[flagIndex];
            for (int i = 0; i < triangles.Length; i++)
            {
                var triplet = triangles[i];
                var idx = vertList.Count;

                indexList.Add(new Triplet(idx + WindingOrder[0], idx + WindingOrder[1], idx + WindingOrder[2]));
                vertList.Add(EdgeVertex[triplet.Index0]);
                vertList.Add(EdgeVertex[triplet.Index1]);
                vertList.Add(EdgeVertex[triplet.Index2]);
            }
        }

        /// <summary>
        /// EdgeConnection lists the index of the endpoint vertices for each 
        /// of the 12 edges of the cube.
        /// edgeConnection[12][2]
        /// </summary>
        private static readonly int[,] EdgeConnection =
        {
            {0, 1}, {1, 2}, {2, 3}, {3, 0},
            {4, 5}, {5, 6}, {6, 7}, {7, 4},
            {0, 4}, {1, 5}, {2, 6}, {3, 7}
        };

        /// <summary>
        /// edgeDirection lists the direction vector (vertex1-vertex0) for each edge in the cube.
        /// edgeDirection[12][3]
        /// </summary>
        private static readonly double[,] EdgeDirection =
        {
            {1, 0, 0},
            {0, 1, 0},
            {-1, 0, 0},
            {0, -1, 0},
            {1, 0, 0},
            {0, 1, 0},
            {-1, 0, 0},
            {0, -1, 0},
            {0, 0, 1},
            {0, 0, 1},
            {0, 0, 1},
            {0, 0, 1}
        };


        /// <summary>
        /// For any edge, if one vertex is inside of the surface and the other 
        /// is outside of the surface then the edge intersects the surface.
        /// For each of the 8 vertices of the cube can be two possible states,
        /// either inside or outside of the surface.
        /// For any cube the are 2^8=256 possible sets of vertex states.
        /// This table lists the edges intersected by the surface for all 256 
        /// possible vertex states. There are 12 edges.  
        /// For each entry in the table, if edge #n is intersected, then bit #n is set to 1.
        /// cubeEdgeFlags[256]
        /// </summary>
        private static readonly int[] CubeEdgeFlags =
        {
            0x000, 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c, 0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
            0x190, 0x099, 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c, 0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
            0x230, 0x339, 0x033, 0x13a, 0x636, 0x73f, 0x435, 0x53c, 0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
            0x3a0, 0x2a9, 0x1a3, 0x0aa, 0x7a6, 0x6af, 0x5a5, 0x4ac, 0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
            0x460, 0x569, 0x663, 0x76a, 0x066, 0x16f, 0x265, 0x36c, 0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
            0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0x0ff, 0x3f5, 0x2fc, 0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
            0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x055, 0x15c, 0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
            0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0x0cc, 0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
            0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc, 0x0cc, 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
            0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c, 0x15c, 0x055, 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
            0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc, 0x2fc, 0x3f5, 0x0ff, 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
            0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c, 0x36c, 0x265, 0x16f, 0x066, 0x76a, 0x663, 0x569, 0x460,
            0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac, 0x4ac, 0x5a5, 0x6af, 0x7a6, 0x0aa, 0x1a3, 0x2a9, 0x3a0,
            0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c, 0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x033, 0x339, 0x230,
            0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c, 0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x099, 0x190,
            0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c, 0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x000
        };


        private static Triplet T(int i0, int i1, int i2) => new Triplet(i0, i1, i2);
        private static Triplet[] A(params Triplet[] triplets) => triplets.ToArray();

        /// <summary>
        /// For each of the possible vertex states listed in cubeEdgeFlags there is a specific triangulation
        /// of the edge intersection points.  triangleConnectionTable lists all of them in the form of
        /// 0-5 edge triples with the list terminated by the invalid value -1.
        /// For example: triangleConnectionTable[3] list the 2 triangles formed when corner[0] 
        /// and corner[1] are inside of the surface, but the rest of the cube is not.
        /// triangleConnectionTable[256][16]
        /// </summary>
        private static readonly Triplet[][] TriangleConnectionTable =
        {
            A(),
            A(T(0, 8, 3)),
            A(T(0, 1, 9)),
            A(T(1, 8, 3), T(9, 8, 1)),
            A(T(1, 2, 10)),
            A(T(0, 8, 3), T(1, 2, 10)),
            A(T(9, 2, 10), T(0, 2, 9)),
            A(T(2, 8, 3), T(2, 10, 8), T(10, 9, 8)),
            A(T(3, 11, 2)),
            A(T(0, 11, 2), T(8, 11, 0)),
            A(T(1, 9, 0), T(2, 3, 11)),
            A(T(1, 11, 2), T(1, 9, 11), T(9, 8, 11)),
            A(T(3, 10, 1), T(11, 10, 3)),
            A(T(0, 10, 1), T(0, 8, 10), T(8, 11, 10)),
            A(T(3, 9, 0), T(3, 11, 9), T(11, 10, 9)),
            A(T(9, 8, 10), T(10, 8, 11)),
            A(T(4, 7, 8)),
            A(T(4, 3, 0), T(7, 3, 4)),
            A(T(0, 1, 9), T(8, 4, 7)),
            A(T(4, 1, 9), T(4, 7, 1), T(7, 3, 1)),
            A(T(1, 2, 10), T(8, 4, 7)),
            A(T(3, 4, 7), T(3, 0, 4), T(1, 2, 10)),
            A(T(9, 2, 10), T(9, 0, 2), T(8, 4, 7)),
            A(T(2, 10, 9), T(2, 9, 7), T(2, 7, 3), T(7, 9, 4)),
            A(T(8, 4, 7), T(3, 11, 2)),
            A(T(11, 4, 7), T(11, 2, 4), T(2, 0, 4)),
            A(T(9, 0, 1), T(8, 4, 7), T(2, 3, 11)),
            A(T(4, 7, 11), T(9, 4, 11), T(9, 11, 2), T(9, 2, 1)),
            A(T(3, 10, 1), T(3, 11, 10), T(7, 8, 4)),
            A(T(1, 11, 10), T(1, 4, 11), T(1, 0, 4), T(7, 11, 4)),
            A(T(4, 7, 8), T(9, 0, 11), T(9, 11, 10), T(11, 0, 3)),
            A(T(4, 7, 11), T(4, 11, 9), T(9, 11, 10)),
            A(T(9, 5, 4)),
            A(T(9, 5, 4), T(0, 8, 3)),
            A(T(0, 5, 4), T(1, 5, 0)),
            A(T(8, 5, 4), T(8, 3, 5), T(3, 1, 5)),
            A(T(1, 2, 10), T(9, 5, 4)),
            A(T(3, 0, 8), T(1, 2, 10), T(4, 9, 5)),
            A(T(5, 2, 10), T(5, 4, 2), T(4, 0, 2)),
            A(T(2, 10, 5), T(3, 2, 5), T(3, 5, 4), T(3, 4, 8)),
            A(T(9, 5, 4), T(2, 3, 11)),
            A(T(0, 11, 2), T(0, 8, 11), T(4, 9, 5)),
            A(T(0, 5, 4), T(0, 1, 5), T(2, 3, 11)),
            A(T(2, 1, 5), T(2, 5, 8), T(2, 8, 11), T(4, 8, 5)),
            A(T(10, 3, 11), T(10, 1, 3), T(9, 5, 4)),
            A(T(4, 9, 5), T(0, 8, 1), T(8, 10, 1), T(8, 11, 10)),
            A(T(5, 4, 0), T(5, 0, 11), T(5, 11, 10), T(11, 0, 3)),
            A(T(5, 4, 8), T(5, 8, 10), T(10, 8, 11)),
            A(T(9, 7, 8), T(5, 7, 9)),
            A(T(9, 3, 0), T(9, 5, 3), T(5, 7, 3)),
            A(T(0, 7, 8), T(0, 1, 7), T(1, 5, 7)),
            A(T(1, 5, 3), T(3, 5, 7)),
            A(T(9, 7, 8), T(9, 5, 7), T(10, 1, 2)),
            A(T(10, 1, 2), T(9, 5, 0), T(5, 3, 0), T(5, 7, 3)),
            A(T(8, 0, 2), T(8, 2, 5), T(8, 5, 7), T(10, 5, 2)),
            A(T(2, 10, 5), T(2, 5, 3), T(3, 5, 7)),
            A(T(7, 9, 5), T(7, 8, 9), T(3, 11, 2)),
            A(T(9, 5, 7), T(9, 7, 2), T(9, 2, 0), T(2, 7, 11)),
            A(T(2, 3, 11), T(0, 1, 8), T(1, 7, 8), T(1, 5, 7)),
            A(T(11, 2, 1), T(11, 1, 7), T(7, 1, 5)),
            A(T(9, 5, 8), T(8, 5, 7), T(10, 1, 3), T(10, 3, 11)),
            A(T(5, 7, 0), T(5, 0, 9), T(7, 11, 0), T(1, 0, 10), T(11, 10, 0)),
            A(T(11, 10, 0), T(11, 0, 3), T(10, 5, 0), T(8, 0, 7), T(5, 7, 0)),
            A(T(11, 10, 5), T(7, 11, 5)),
            A(T(10, 6, 5)),
            A(T(0, 8, 3), T(5, 10, 6)),
            A(T(9, 0, 1), T(5, 10, 6)),
            A(T(1, 8, 3), T(1, 9, 8), T(5, 10, 6)),
            A(T(1, 6, 5), T(2, 6, 1)),
            A(T(1, 6, 5), T(1, 2, 6), T(3, 0, 8)),
            A(T(9, 6, 5), T(9, 0, 6), T(0, 2, 6)),
            A(T(5, 9, 8), T(5, 8, 2), T(5, 2, 6), T(3, 2, 8)),
            A(T(2, 3, 11), T(10, 6, 5)),
            A(T(11, 0, 8), T(11, 2, 0), T(10, 6, 5)),
            A(T(0, 1, 9), T(2, 3, 11), T(5, 10, 6)),
            A(T(5, 10, 6), T(1, 9, 2), T(9, 11, 2), T(9, 8, 11)),
            A(T(6, 3, 11), T(6, 5, 3), T(5, 1, 3)),
            A(T(0, 8, 11), T(0, 11, 5), T(0, 5, 1), T(5, 11, 6)),
            A(T(3, 11, 6), T(0, 3, 6), T(0, 6, 5), T(0, 5, 9)),
            A(T(6, 5, 9), T(6, 9, 11), T(11, 9, 8)),
            A(T(5, 10, 6), T(4, 7, 8)),
            A(T(4, 3, 0), T(4, 7, 3), T(6, 5, 10)),
            A(T(1, 9, 0), T(5, 10, 6), T(8, 4, 7)),
            A(T(10, 6, 5), T(1, 9, 7), T(1, 7, 3), T(7, 9, 4)),
            A(T(6, 1, 2), T(6, 5, 1), T(4, 7, 8)),
            A(T(1, 2, 5), T(5, 2, 6), T(3, 0, 4), T(3, 4, 7)),
            A(T(8, 4, 7), T(9, 0, 5), T(0, 6, 5), T(0, 2, 6)),
            A(T(7, 3, 9), T(7, 9, 4), T(3, 2, 9), T(5, 9, 6), T(2, 6, 9)),
            A(T(3, 11, 2), T(7, 8, 4), T(10, 6, 5)),
            A(T(5, 10, 6), T(4, 7, 2), T(4, 2, 0), T(2, 7, 11)),
            A(T(0, 1, 9), T(4, 7, 8), T(2, 3, 11), T(5, 10, 6)),
            A(T(9, 2, 1), T(9, 11, 2), T(9, 4, 11), T(7, 11, 4), T(5, 10, 6)),
            A(T(8, 4, 7), T(3, 11, 5), T(3, 5, 1), T(5, 11, 6)),
            A(T(5, 1, 11), T(5, 11, 6), T(1, 0, 11), T(7, 11, 4), T(0, 4, 11)),
            A(T(0, 5, 9), T(0, 6, 5), T(0, 3, 6), T(11, 6, 3), T(8, 4, 7)),
            A(T(6, 5, 9), T(6, 9, 11), T(4, 7, 9), T(7, 11, 9)),
            A(T(10, 4, 9), T(6, 4, 10)),
            A(T(4, 10, 6), T(4, 9, 10), T(0, 8, 3)),
            A(T(10, 0, 1), T(10, 6, 0), T(6, 4, 0)),
            A(T(8, 3, 1), T(8, 1, 6), T(8, 6, 4), T(6, 1, 10)),
            A(T(1, 4, 9), T(1, 2, 4), T(2, 6, 4)),
            A(T(3, 0, 8), T(1, 2, 9), T(2, 4, 9), T(2, 6, 4)),
            A(T(0, 2, 4), T(4, 2, 6)),
            A(T(8, 3, 2), T(8, 2, 4), T(4, 2, 6)),
            A(T(10, 4, 9), T(10, 6, 4), T(11, 2, 3)),
            A(T(0, 8, 2), T(2, 8, 11), T(4, 9, 10), T(4, 10, 6)),
            A(T(3, 11, 2), T(0, 1, 6), T(0, 6, 4), T(6, 1, 10)),
            A(T(6, 4, 1), T(6, 1, 10), T(4, 8, 1), T(2, 1, 11), T(8, 11, 1)),
            A(T(9, 6, 4), T(9, 3, 6), T(9, 1, 3), T(11, 6, 3)),
            A(T(8, 11, 1), T(8, 1, 0), T(11, 6, 1), T(9, 1, 4), T(6, 4, 1)),
            A(T(3, 11, 6), T(3, 6, 0), T(0, 6, 4)),
            A(T(6, 4, 8), T(11, 6, 8)),
            A(T(7, 10, 6), T(7, 8, 10), T(8, 9, 10)),
            A(T(0, 7, 3), T(0, 10, 7), T(0, 9, 10), T(6, 7, 10)),
            A(T(10, 6, 7), T(1, 10, 7), T(1, 7, 8), T(1, 8, 0)),
            A(T(10, 6, 7), T(10, 7, 1), T(1, 7, 3)),
            A(T(1, 2, 6), T(1, 6, 8), T(1, 8, 9), T(8, 6, 7)),
            A(T(2, 6, 9), T(2, 9, 1), T(6, 7, 9), T(0, 9, 3), T(7, 3, 9)),
            A(T(7, 8, 0), T(7, 0, 6), T(6, 0, 2)),
            A(T(7, 3, 2), T(6, 7, 2)),
            A(T(2, 3, 11), T(10, 6, 8), T(10, 8, 9), T(8, 6, 7)),
            A(T(2, 0, 7), T(2, 7, 11), T(0, 9, 7), T(6, 7, 10), T(9, 10, 7)),
            A(T(1, 8, 0), T(1, 7, 8), T(1, 10, 7), T(6, 7, 10), T(2, 3, 11)),
            A(T(11, 2, 1), T(11, 1, 7), T(10, 6, 1), T(6, 7, 1)),
            A(T(8, 9, 6), T(8, 6, 7), T(9, 1, 6), T(11, 6, 3), T(1, 3, 6)),
            A(T(0, 9, 1), T(11, 6, 7)),
            A(T(7, 8, 0), T(7, 0, 6), T(3, 11, 0), T(11, 6, 0)),
            A(T(7, 11, 6)),
            A(T(7, 6, 11)),
            A(T(3, 0, 8), T(11, 7, 6)),
            A(T(0, 1, 9), T(11, 7, 6)),
            A(T(8, 1, 9), T(8, 3, 1), T(11, 7, 6)),
            A(T(10, 1, 2), T(6, 11, 7)),
            A(T(1, 2, 10), T(3, 0, 8), T(6, 11, 7)),
            A(T(2, 9, 0), T(2, 10, 9), T(6, 11, 7)),
            A(T(6, 11, 7), T(2, 10, 3), T(10, 8, 3), T(10, 9, 8)),
            A(T(7, 2, 3), T(6, 2, 7)),
            A(T(7, 0, 8), T(7, 6, 0), T(6, 2, 0)),
            A(T(2, 7, 6), T(2, 3, 7), T(0, 1, 9)),
            A(T(1, 6, 2), T(1, 8, 6), T(1, 9, 8), T(8, 7, 6)),
            A(T(10, 7, 6), T(10, 1, 7), T(1, 3, 7)),
            A(T(10, 7, 6), T(1, 7, 10), T(1, 8, 7), T(1, 0, 8)),
            A(T(0, 3, 7), T(0, 7, 10), T(0, 10, 9), T(6, 10, 7)),
            A(T(7, 6, 10), T(7, 10, 8), T(8, 10, 9)),
            A(T(6, 8, 4), T(11, 8, 6)),
            A(T(3, 6, 11), T(3, 0, 6), T(0, 4, 6)),
            A(T(8, 6, 11), T(8, 4, 6), T(9, 0, 1)),
            A(T(9, 4, 6), T(9, 6, 3), T(9, 3, 1), T(11, 3, 6)),
            A(T(6, 8, 4), T(6, 11, 8), T(2, 10, 1)),
            A(T(1, 2, 10), T(3, 0, 11), T(0, 6, 11), T(0, 4, 6)),
            A(T(4, 11, 8), T(4, 6, 11), T(0, 2, 9), T(2, 10, 9)),
            A(T(10, 9, 3), T(10, 3, 2), T(9, 4, 3), T(11, 3, 6), T(4, 6, 3)),
            A(T(8, 2, 3), T(8, 4, 2), T(4, 6, 2)),
            A(T(0, 4, 2), T(4, 6, 2)),
            A(T(1, 9, 0), T(2, 3, 4), T(2, 4, 6), T(4, 3, 8)),
            A(T(1, 9, 4), T(1, 4, 2), T(2, 4, 6)),
            A(T(8, 1, 3), T(8, 6, 1), T(8, 4, 6), T(6, 10, 1)),
            A(T(10, 1, 0), T(10, 0, 6), T(6, 0, 4)),
            A(T(4, 6, 3), T(4, 3, 8), T(6, 10, 3), T(0, 3, 9), T(10, 9, 3)),
            A(T(10, 9, 4), T(6, 10, 4)),
            A(T(4, 9, 5), T(7, 6, 11)),
            A(T(0, 8, 3), T(4, 9, 5), T(11, 7, 6)),
            A(T(5, 0, 1), T(5, 4, 0), T(7, 6, 11)),
            A(T(11, 7, 6), T(8, 3, 4), T(3, 5, 4), T(3, 1, 5)),
            A(T(9, 5, 4), T(10, 1, 2), T(7, 6, 11)),
            A(T(6, 11, 7), T(1, 2, 10), T(0, 8, 3), T(4, 9, 5)),
            A(T(7, 6, 11), T(5, 4, 10), T(4, 2, 10), T(4, 0, 2)),
            A(T(3, 4, 8), T(3, 5, 4), T(3, 2, 5), T(10, 5, 2), T(11, 7, 6)),
            A(T(7, 2, 3), T(7, 6, 2), T(5, 4, 9)),
            A(T(9, 5, 4), T(0, 8, 6), T(0, 6, 2), T(6, 8, 7)),
            A(T(3, 6, 2), T(3, 7, 6), T(1, 5, 0), T(5, 4, 0)),
            A(T(6, 2, 8), T(6, 8, 7), T(2, 1, 8), T(4, 8, 5), T(1, 5, 8)),
            A(T(9, 5, 4), T(10, 1, 6), T(1, 7, 6), T(1, 3, 7)),
            A(T(1, 6, 10), T(1, 7, 6), T(1, 0, 7), T(8, 7, 0), T(9, 5, 4)),
            A(T(4, 0, 10), T(4, 10, 5), T(0, 3, 10), T(6, 10, 7), T(3, 7, 10)),
            A(T(7, 6, 10), T(7, 10, 8), T(5, 4, 10), T(4, 8, 10)),
            A(T(6, 9, 5), T(6, 11, 9), T(11, 8, 9)),
            A(T(3, 6, 11), T(0, 6, 3), T(0, 5, 6), T(0, 9, 5)),
            A(T(0, 11, 8), T(0, 5, 11), T(0, 1, 5), T(5, 6, 11)),
            A(T(6, 11, 3), T(6, 3, 5), T(5, 3, 1)),
            A(T(1, 2, 10), T(9, 5, 11), T(9, 11, 8), T(11, 5, 6)),
            A(T(0, 11, 3), T(0, 6, 11), T(0, 9, 6), T(5, 6, 9), T(1, 2, 10)),
            A(T(11, 8, 5), T(11, 5, 6), T(8, 0, 5), T(10, 5, 2), T(0, 2, 5)),
            A(T(6, 11, 3), T(6, 3, 5), T(2, 10, 3), T(10, 5, 3)),
            A(T(5, 8, 9), T(5, 2, 8), T(5, 6, 2), T(3, 8, 2)),
            A(T(9, 5, 6), T(9, 6, 0), T(0, 6, 2)),
            A(T(1, 5, 8), T(1, 8, 0), T(5, 6, 8), T(3, 8, 2), T(6, 2, 8)),
            A(T(1, 5, 6), T(2, 1, 6)),
            A(T(1, 3, 6), T(1, 6, 10), T(3, 8, 6), T(5, 6, 9), T(8, 9, 6)),
            A(T(10, 1, 0), T(10, 0, 6), T(9, 5, 0), T(5, 6, 0)),
            A(T(0, 3, 8), T(5, 6, 10)),
            A(T(10, 5, 6)),
            A(T(11, 5, 10), T(7, 5, 11)),
            A(T(11, 5, 10), T(11, 7, 5), T(8, 3, 0)),
            A(T(5, 11, 7), T(5, 10, 11), T(1, 9, 0)),
            A(T(10, 7, 5), T(10, 11, 7), T(9, 8, 1), T(8, 3, 1)),
            A(T(11, 1, 2), T(11, 7, 1), T(7, 5, 1)),
            A(T(0, 8, 3), T(1, 2, 7), T(1, 7, 5), T(7, 2, 11)),
            A(T(9, 7, 5), T(9, 2, 7), T(9, 0, 2), T(2, 11, 7)),
            A(T(7, 5, 2), T(7, 2, 11), T(5, 9, 2), T(3, 2, 8), T(9, 8, 2)),
            A(T(2, 5, 10), T(2, 3, 5), T(3, 7, 5)),
            A(T(8, 2, 0), T(8, 5, 2), T(8, 7, 5), T(10, 2, 5)),
            A(T(9, 0, 1), T(5, 10, 3), T(5, 3, 7), T(3, 10, 2)),
            A(T(9, 8, 2), T(9, 2, 1), T(8, 7, 2), T(10, 2, 5), T(7, 5, 2)),
            A(T(1, 3, 5), T(3, 7, 5)),
            A(T(0, 8, 7), T(0, 7, 1), T(1, 7, 5)),
            A(T(9, 0, 3), T(9, 3, 5), T(5, 3, 7)),
            A(T(9, 8, 7), T(5, 9, 7)),
            A(T(5, 8, 4), T(5, 10, 8), T(10, 11, 8)),
            A(T(5, 0, 4), T(5, 11, 0), T(5, 10, 11), T(11, 3, 0)),
            A(T(0, 1, 9), T(8, 4, 10), T(8, 10, 11), T(10, 4, 5)),
            A(T(10, 11, 4), T(10, 4, 5), T(11, 3, 4), T(9, 4, 1), T(3, 1, 4)),
            A(T(2, 5, 1), T(2, 8, 5), T(2, 11, 8), T(4, 5, 8)),
            A(T(0, 4, 11), T(0, 11, 3), T(4, 5, 11), T(2, 11, 1), T(5, 1, 11)),
            A(T(0, 2, 5), T(0, 5, 9), T(2, 11, 5), T(4, 5, 8), T(11, 8, 5)),
            A(T(9, 4, 5), T(2, 11, 3)),
            A(T(2, 5, 10), T(3, 5, 2), T(3, 4, 5), T(3, 8, 4)),
            A(T(5, 10, 2), T(5, 2, 4), T(4, 2, 0)),
            A(T(3, 10, 2), T(3, 5, 10), T(3, 8, 5), T(4, 5, 8), T(0, 1, 9)),
            A(T(5, 10, 2), T(5, 2, 4), T(1, 9, 2), T(9, 4, 2)),
            A(T(8, 4, 5), T(8, 5, 3), T(3, 5, 1)),
            A(T(0, 4, 5), T(1, 0, 5)),
            A(T(8, 4, 5), T(8, 5, 3), T(9, 0, 5), T(0, 3, 5)),
            A(T(9, 4, 5)),
            A(T(4, 11, 7), T(4, 9, 11), T(9, 10, 11)),
            A(T(0, 8, 3), T(4, 9, 7), T(9, 11, 7), T(9, 10, 11)),
            A(T(1, 10, 11), T(1, 11, 4), T(1, 4, 0), T(7, 4, 11)),
            A(T(3, 1, 4), T(3, 4, 8), T(1, 10, 4), T(7, 4, 11), T(10, 11, 4)),
            A(T(4, 11, 7), T(9, 11, 4), T(9, 2, 11), T(9, 1, 2)),
            A(T(9, 7, 4), T(9, 11, 7), T(9, 1, 11), T(2, 11, 1), T(0, 8, 3)),
            A(T(11, 7, 4), T(11, 4, 2), T(2, 4, 0)),
            A(T(11, 7, 4), T(11, 4, 2), T(8, 3, 4), T(3, 2, 4)),
            A(T(2, 9, 10), T(2, 7, 9), T(2, 3, 7), T(7, 4, 9)),
            A(T(9, 10, 7), T(9, 7, 4), T(10, 2, 7), T(8, 7, 0), T(2, 0, 7)),
            A(T(3, 7, 10), T(3, 10, 2), T(7, 4, 10), T(1, 10, 0), T(4, 0, 10)),
            A(T(1, 10, 2), T(8, 7, 4)),
            A(T(4, 9, 1), T(4, 1, 7), T(7, 1, 3)),
            A(T(4, 9, 1), T(4, 1, 7), T(0, 8, 1), T(8, 7, 1)),
            A(T(4, 0, 3), T(7, 4, 3)),
            A(T(4, 8, 7)),
            A(T(9, 10, 8), T(10, 11, 8)),
            A(T(3, 0, 9), T(3, 9, 11), T(11, 9, 10)),
            A(T(0, 1, 10), T(0, 10, 8), T(8, 10, 11)),
            A(T(3, 1, 10), T(11, 3, 10)),
            A(T(1, 2, 11), T(1, 11, 9), T(9, 11, 8)),
            A(T(3, 0, 9), T(3, 9, 11), T(1, 2, 9), T(2, 11, 9)),
            A(T(0, 2, 11), T(8, 0, 11)),
            A(T(3, 2, 11)),
            A(T(2, 3, 8), T(2, 8, 10), T(10, 8, 9)),
            A(T(9, 10, 2), T(0, 9, 2)),
            A(T(2, 3, 8), T(2, 8, 10), T(0, 1, 8), T(1, 10, 8)),
            A(T(1, 10, 2)),
            A(T(1, 3, 8), T(9, 1, 8)),
            A(T(0, 9, 1)),
            A(T(0, 3, 8)),
            A(),
        };
   }
}