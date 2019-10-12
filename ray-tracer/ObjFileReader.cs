using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ray_tracer.Shapes;

namespace ray_tracer
{
    public class ObjFileReader
    {
        public int Ignored { get; private set; }
        public List<Tuple> Vertices { get; } = new List<Tuple>();
        public List<Tuple> Normals { get; } = new List<Tuple>();
        public List<Triangle> Triangles { get; } = new List<Triangle>();
        public Group DefaultGroup => Groups[0];
        public List<Group> Groups { get; } = new List<Group> {new Group()};

        public Group ObjToGroup()
        {
            if (Groups.Count == 1)
            {
                return DefaultGroup;
            }
            
            var group = new Group();
            Groups.Where(g => g.Count !=0).ToList().ForEach(g => group.Add(g));
            return group;
        }

        public ObjFileReader(string file)
        {
            Console.WriteLine($"Reading : {file}");
            Init(File.ReadLines(file));
            Console.WriteLine($"Triangles: {Triangles.Count} Vertices: {Vertices.Count}");
        }

        public ObjFileReader(params string[] lines)
        {
            Init(lines);
        }

        public ObjFileReader(IEnumerable<string> lines)
        {
            Init(lines);
        }

        private void Init(IEnumerable<string> lines)
        {
            var group = DefaultGroup;
            foreach (var line in lines)
            {
                var items = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (!items.Any())
                {
                    continue;
                }

                switch (items[0])
                {
                    case "v":
                        ReadVertex(items);
                        break;
                    case "vn":
                        ReadNormal(items);
                        break;
                    case "f":
                        if (line.Contains('/'))
                        {
                            ReadSmoothTriangle(items, group);
                        }
                        else
                        {
                            ReadTriangle(items, group);
                        }

                        break;
                    case "g":
                        group = new Group();
                        Groups.Add(group);
                        break;
                    default:
                        Ignored++;
                        break;
                }
            }
        }

        private void ReadTriangle(string[] items, Group group)
        {
            var ns = items.Skip(1).Select(int.Parse).ToArray();
            var n1 = ns[0];
            for (int i = 0; i < ns.Length - 2; i++)
            {
                var n2 = ns[i + 1];
                var n3 = ns[i + 2];
                var p1 = Vertices[n1 - 1];
                var p2 = Vertices[n2 - 1];
                var p3 = Vertices[n3 - 1];
                var triangle = new Triangle(p1, p2, p3);
                Triangles.Add(triangle);
                group.Add(triangle);
            }
        }

        private void ReadSmoothTriangle(string[] items, Group group)
        {
            var ns = items.Skip(1)
                .Select(s => s.Split('/'))
                .Select(s => new {VertexIndex = int.Parse(s[0])-1, NormalIndex= int.Parse(s[2])-1})
                .ToArray();
            
            var n1 = ns[0];
            
            for (int i = 0; i < ns.Length - 2; i++)
            {
                var n2 = ns[i + 1];
                var n3 = ns[i + 2];
                var p1 = Vertices[n1.VertexIndex];
                var p2 = Vertices[n2.VertexIndex];
                var p3 = Vertices[n3.VertexIndex];
                var norm1 = Normals[n1.NormalIndex];
                var norm2 = Normals[n2.NormalIndex];
                var norm3 = Normals[n3.NormalIndex];
                    
                var triangle = new SmoothTriangle(p1, p2, p3, norm1, norm2, norm3);
                Triangles.Add(triangle);
                group.Add(triangle);
            }
        }

        private void ReadVertex(string[] items)
        {
            var x = double.Parse(items[1]);
            var y = double.Parse(items[2]);
            var z = double.Parse(items[3]);
            Vertices.Add(Helper.CreatePoint(x, y, z));
        }
        
        private void ReadNormal(string[] items)
        {
            var x = double.Parse(items[1]);
            var y = double.Parse(items[2]);
            var z = double.Parse(items[3]);
            Normals.Add(Helper.CreatePoint(x, y, z));
        }
    }
}