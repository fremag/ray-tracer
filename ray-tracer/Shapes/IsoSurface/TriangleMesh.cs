using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ray_tracer.Shapes.IsoSurface
{
    public class TriangleMesh
    {
        public IList<Triplet> Triplets { get; set; } = new List<Triplet>(4 * 1024);
        public IList<Tuple> Vertices { get; } = new List<Tuple>(4 * 1024);

        public void Compress(double distMax = 1e-7)
        {
            var dMax = distMax * distMax;
            Stopwatch sw = Stopwatch.StartNew();
            bool[] merged = new bool[Vertices.Count];

            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                if (merged[i])
                {
                    continue;
                }

                var p1 = Vertices[i];

                for (int j = i + 1; j < Vertices.Count; j++)
                {
                    if (merged[j])
                    {
                        continue;
                    }

                    var p2 = Vertices[j];

                    var x = p1.X - p2.X;
                    var y = p1.Y - p2.Y;
                    var z = p1.Z - p2.Z;
                    var dist = x * x + y * y + z * z;

                    if (dist > dMax)
                    {
                        continue;
                    }

                    merged[j] = true;
                    foreach (var t in Triplets)
                    {
                        if (t.Index0 == j && t.Index1 != i && t.Index2 != i)
                        {
                            t.Index0 = i;
                        }

                        if (t.Index1 == j && t.Index0 != i && t.Index2 != i)
                        {
                            t.Index1 = i;
                        }

                        if (t.Index2 == j && t.Index0 != i && t.Index1 != i)
                        {
                            t.Index2 = i;
                        }
                    }
                }
            }

            int n1 = Triplets.Count;
            Triplets = Triplets.Distinct().ToList();
            int n2 = Triplets.Count;
            Console.WriteLine($"[Compress] n1: {n1}, n2: {n2}, t: {sw.ElapsedMilliseconds} ms");
        }

        public IEnumerable<Triangle> GenerateTriangles()
        {
            for (int i = 0; i < Triplets.Count; i++)
            {
                int i0 = Triplets[i].Index0;
                int i1 = Triplets[i].Index1;
                int i2 = Triplets[i].Index2;
                Tuple p1 = Vertices[i0];
                Tuple p2 = Vertices[i1];
                Tuple p3 = Vertices[i2];
                var triangle = new Triangle(p1, p2, p3);
                yield return triangle;
            }
        }

        public IEnumerable<IShape> GenerateSmoothTriangles()
        {
            List<Triangle>[] normals = Vertices.Select(_ => new List<Triangle>()).ToArray();

            for (int i = 0; i < Triplets.Count; i++)
            {
                int i0 = Triplets[i].Index0;
                int i1 = Triplets[i].Index1;
                int i2 = Triplets[i].Index2;
                if (i0 == i1 || i0 == i2 || i1== i2)
                {
                    Console.WriteLine("What ?");
                }
                Tuple p1 = Vertices[i0];
                Tuple p2 = Vertices[i1];
                Tuple p3 = Vertices[i2];
                var triangle = new Triangle(p1, p2, p3);
                normals[i0].Add(triangle);
                normals[i1].Add(triangle);
                normals[i2].Add(triangle);
            }

            Material[] materials = {
                new Material(Color.Cyan),
                new Material(Color._Blue),
                new Material(Color._Green),
                new Material(Color._Red),
                new Material(Color.Yellow),
                new Material(Color.Apricot),
                new Material(Color.Pink),
                new Material(Color.Purple),
                new Material(Color.Olive),
                new Material(Color.Lavender),
                new Material(Color.Lime),
                new Material(Color.Mint),
                new Material(Color.Navy),
                new Material(Color.Teal)
            };

            for (int i = 0; i < Triplets.Count; i++)
            {
                int i0 = Triplets[i].Index0;
                int i1 = Triplets[i].Index1;
                int i2 = Triplets[i].Index2;

                Tuple n0 = Average(normals[i0]);
                Tuple n1 = Average(normals[i1]);
                Tuple n2 = Average(normals[i2]);

                Tuple p0 = Vertices[i0];
                Tuple p1 = Vertices[i1];
                Tuple p2 = Vertices[i2];

                yield return new SmoothTriangle(p0, p1, p2, n0, n1, n2)
                {
                    Material = materials[i % materials.Length]
                };
            }
        }

        private Tuple Average(List<Triangle> normals)
        {
            double vx = 0;
            double vy = 0;
            double vz = 0;

            var n = normals.Count;
            for (int i = 0; i < n; i++)
            {
                var normal = normals[i].N;
                vx += normal.X;
                vy += normal.Y;
                vz += normal.Z;
            }

            var avg = Helper.CreateVector(vx / n, vy / n, vz / n);
            return avg;
        }
    }
}