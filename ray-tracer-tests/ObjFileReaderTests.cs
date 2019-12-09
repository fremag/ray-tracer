using NFluent;
using ray_tracer.Shapes;
using Xunit;

namespace ray_tracer.tests
{
    public class ObjFileReaderTests
    {
        [Fact]
        public void IgnoreUnrecognizedLinesTest()
        {
            ObjFileReader reader = new ObjFileReader("There was a young lady named Bright", 
            "who traveled much faster than light.",
                "She set out one day",
                "in a relative way,",
                "and came back the previous night.");

            Check.That(reader.Ignored).IsEqualTo(5);
        }

        [Fact]
        public void VertexRecordsTest()
        {
            var reader = new ObjFileReader(
                "v -1 1 0",
            "v -1.0000 0.5000 0.0000",
            "v 1 0 0",
            "v 1 1 0");
            Check.That(reader.Ignored).IsZero();
            Check.That(reader.Vertices[0]).IsEqualTo(Helper.CreatePoint(-1, 1, 0));
            Check.That(reader.Vertices[1]).IsEqualTo(Helper.CreatePoint(-1, 0.5, 0));
            Check.That(reader.Vertices[2]).IsEqualTo(Helper.CreatePoint( 1, 0, 0));
            Check.That(reader.Vertices[3]).IsEqualTo(Helper.CreatePoint( 1, 1, 0));
        }

        [Fact]
        public void ParsingTriangleFacesTest()
        {
            var reader = new ObjFileReader(
                "v -1 1 0",
                "v -1 0 0",
                "v 1 0 0",
                "v 1 1 0",
                "f 1 2 3",
                "f 1 3 4");
            Check.That(reader.Triangles[0].P1).IsEqualTo(reader.Vertices[0]);
            Check.That(reader.Triangles[1].P1).IsEqualTo(reader.Vertices[0]);
        }
        
        [Fact]
        public void TriangulatingPolygonsTest()
        {
            var reader = new ObjFileReader(
             "v -1 1 0",
            "v -1 0 0",
            "v 1 0 0",
            "v 1 1 0",
            "v 0 2 0",
            "f 1 2 3 4 5");
            var t1 = reader.DefaultGroup[0] as Triangle;
            var t2 = reader.DefaultGroup[1] as Triangle;
            var t3 = reader.DefaultGroup[2] as Triangle;

            Check.That(t1.P1).IsEqualTo(reader.Vertices[0]);
            
            Check.That(t2.P1).IsEqualTo(reader.Vertices[0]);
            
            Check.That(t3.P1).IsEqualTo(reader.Vertices[0]);
        }
        
        [Fact]
        public void TrianglesInGroupTest()
        {
            var reader = new ObjFileReader(
                "v -1 1 0",
            "v -1 0 0",
            "v 1 0 0",
            "v 1 1 0",
            "g FirstGroup",
            "f 1 2 3",
            "g SecondGroup",
            "f 1 3 4");
            var t1 = reader.Groups[1][0] as Triangle;
            var t2 = reader.Groups[2][0] as Triangle;

            Check.That(t1.P1).IsEqualTo(reader.Vertices[0]);
            
            Check.That(t2.P1).IsEqualTo(reader.Vertices[0]);
        }
        
        [Fact]
        public void VertexNormalRecordTest()
        {
            var reader = new ObjFileReader(
                "vn 0 0 1",
            "vn 0.707 0 -0.707",
            "vn 1 2 3");
            
            Check.That(reader.Normals[0]).IsEqualTo(Helper.CreatePoint(0,0,1));
            Check.That(reader.Normals[1]).IsEqualTo(Helper.CreatePoint(0.707, 0, -0.707));
            Check.That(reader.Normals[2]).IsEqualTo(Helper.CreatePoint(1,2,3));
        }
        
        [Fact]
        public void TrianglesWithNormalsTest()
        {
            var reader = new ObjFileReader(
                "v 0 1 0",
                "v -1 0 0",
                "v 1 0 0",
                "",
                "vn -1 0 0",
                "vn 1 0 0",
                "vn 0 1 0",
                "f 1//3 2//1 3//2",
                "f 1/0/3 2/102/1 3/14/2");
            var t1 = reader.DefaultGroup[0] as Triangle;
            var t2 = reader.DefaultGroup[0] as Triangle;

            Check.That(t1.P1).IsEqualTo(reader.Vertices[0]);
            
            Check.That(t2.P1).IsEqualTo(reader.Vertices[0]);
        }        
    }
}