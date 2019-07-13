using NFluent;
using raytracer;
using Xunit;

namespace ray_tracer_tests
{
    public class MatrixTests
    {
        [Fact]
        public void CreateMatrix_4x4_Test()
        {
            var matrix = new Matrix(4, new[]
            { new double[]{ 1 , 2 , 3 , 4 },
                new double[] { 5.5 ,6.5 , 7.5 , 8.5 },
                new double[]{ 9 , 10 , 11 , 12 },
                new double[]{ 13.5 , 14.5,  15.5, 16.5 }});

            Check.That(matrix[0, 0]).IsEqualTo(1);
            Check.That(matrix[0,3] ).IsEqualTo( 4);
            Check.That(matrix[1,0] ).IsEqualTo(5.5);
            Check.That(matrix[1,2] ).IsEqualTo( 7.5);
            Check.That(matrix[2,2] ).IsEqualTo( 11);
            Check.That(matrix[3,0] ).IsEqualTo( 13.5);
            Check.That(matrix[3,2]).IsEqualTo( 15.5);
        }
        [Fact]
        public void CreateMatrix_2x2_Test()
        {
            var matrix = new Matrix(2, new[]
            { new double[]{ -3, 5},
                new double[]{1, 2 }});

            Check.That(matrix[0, 0]).IsEqualTo(-3);
            Check.That(matrix[0,1] ).IsEqualTo( 5);
            Check.That(matrix[1,0] ).IsEqualTo(1);
            Check.That(matrix[1,1] ).IsEqualTo( 2);
        }
        
        [Fact]
        public void CreateMatrix_3x3_Test()
        {
            var matrix = new Matrix(3, new[]
            { new double[]{ -3 , 5 , 0},
                new double[] {1, -2, -7 },
                new double[]{ 0,1,1 }});

            Check.That(matrix[0, 0]).IsEqualTo(-3);
            Check.That(matrix[1,1] ).IsEqualTo( -2);
            Check.That(matrix[2,2] ).IsEqualTo(1);
        }

        [Fact]
        public void EqualsMatrixTest()
        {
            var matrix1 = new Matrix(4, new[]
        { new double[]{ 1 , 2 , 3 , 4 },
            new double[] { 5 ,6 , 7 , 8 },
            new double[]{ 9 , 8 , 7 , 6 },
            new double[]{ 5 , 4,  3, 2 }});

            var matrix2 = new Matrix(4, new[]
            { new double[]{ 1 , 2 , 3 , 4 },
                new double[] { 5 ,6 , 7 , 8 },
                new double[]{ 9 , 8 , 7 , 6 },
                new double[]{ 5 , 4,  3, 2 }});

            Check.That(matrix1.Equals(matrix2)).IsTrue();
            Check.That(matrix1).IsEqualTo(matrix2);
            Check.That(matrix1 == matrix2).IsTrue();
            Check.That(matrix1 != matrix2).IsFalse();
            
            var m3= new Matrix(4);
            Check.That(matrix1).IsNotEqualTo(m3);
        }
        

        [Fact]
        public void MultiplyTest()
        {
            var matrix1 = new Matrix(4, new[]
            { new double[]{ 1 , 2 , 3 , 4 },
                new double[] { 5 ,6 , 7 , 8 },
                new double[]{ 9 , 8 , 7 , 6 },
                new double[]{ 5 , 4,  3, 2 }});

            var matrix2 = new Matrix(4, new[]
                { new double[]{ -2 , 1 , 2 , 3 },
                    new double[] { 3 ,2 , 1 , -1 },
                    new double[]{ 4 , 3 , 6 , 5 },
                    new double[]{ 1 , 2,  7, 8 }});
            var matrix3 = new Matrix(4, new[]
                { new double[]{ 20 , 22 , 50 , 48 },
                    new double[] { 44 ,54 , 114 , 108 },
                    new double[]{ 40 , 58 , 110 , 102 },
                    new double[]{ 16 , 26,  46, 42 }});

            var m = matrix1 * matrix2;
            Check.That(m).IsEqualTo(matrix3);
        }

        [Fact]
        public void MultiplyTupleTest()
        {
            var A = new Matrix(4, new[]
            {
                new double[] {1, 2, 3, 4},
                new double[] {2, 4, 4, 2},
                new double[] {8, 6, 4, 1},
                new double[] {0, 0, 0, 1}
            });

            var b = new Tuple(1, 2, 3, 1);
            var c = A * b;
            var expected = new Tuple(18, 24, 33, 1);

            Check.That(c).IsEqualTo(expected);
        }
        [Fact]
        public void MultiplyIdentityTupleTest()
        {
            var A = Helper.CreateIdentity(4);

            var b = new Tuple(1, 2, 3, 1);
            var c = A * b;

            Check.That(c).IsEqualTo(b);
        }

        [Fact]
        public void TransposeTest()
        {
            var m = new Matrix(4, new []
            {
                new double[]{0 , 9 , 3 , 0 },
                new double[]{9 , 8 , 0 , 8} ,
                new double[]{1 , 8 , 5 , 3} ,
                new double[]{0 , 0 , 5 , 8}
            });
            var t = new Matrix(4, new []
            {
                new double[]{0 , 9 , 1 , 0 },
                new double[]{9 , 8 , 8 , 0} ,
                new double[]{3 , 0 , 5 , 5} ,
                new double[]{0 , 8 , 3 , 8}
            });

            Check.That(m.Transpose()).IsEqualTo(t);
        }
    }
}