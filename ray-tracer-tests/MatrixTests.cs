using NFluent;
using Xunit;

namespace ray_tracer.tests
{
    public class MatrixTests
    {
        [Fact]
        public void CreateMatrix_4x4_Test()
        {
            var matrix = new Matrix(4, new[]
            {
                new double[] {1, 2, 3, 4},
                new double[] {5.5, 6.5, 7.5, 8.5},
                new double[] {9, 10, 11, 12},
                new double[] {13.5, 14.5, 15.5, 16.5}
            });

            Check.That(matrix[0, 0]).IsEqualTo(1);
            Check.That(matrix[0, 3]).IsEqualTo(4);
            Check.That(matrix[1, 0]).IsEqualTo(5.5);
            Check.That(matrix[1, 2]).IsEqualTo(7.5);
            Check.That(matrix[2, 2]).IsEqualTo(11);
            Check.That(matrix[3, 0]).IsEqualTo(13.5);
            Check.That(matrix[3, 2]).IsEqualTo(15.5);
        }

        [Fact]
        public void CreateMatrix_2x2_Test()
        {
            var matrix = new Matrix(2, new[]
            {
                new double[] {-3, 5},
                new double[] {1, 2}
            });

            Check.That(matrix[0, 0]).IsEqualTo(-3);
            Check.That(matrix[0, 1]).IsEqualTo(5);
            Check.That(matrix[1, 0]).IsEqualTo(1);
            Check.That(matrix[1, 1]).IsEqualTo(2);
        }

        [Fact]
        public void CreateMatrix_3x3_Test()
        {
            var matrix = new Matrix(3, new[]
            {
                new double[] {-3, 5, 0},
                new double[] {1, -2, -7},
                new double[] {0, 1, 1}
            });

            Check.That(matrix[0, 0]).IsEqualTo(-3);
            Check.That(matrix[1, 1]).IsEqualTo(-2);
            Check.That(matrix[2, 2]).IsEqualTo(1);
        }

        [Fact]
        public void EqualsNullMatrixTest()
        {
            var matrix = new Matrix(4);
            Check.That(matrix).IsNotEqualTo(null);
            Check.That(matrix != null).IsTrue();
            Check.That(matrix == null).IsFalse();

            matrix = null;
            Check.That(matrix).IsEqualTo(null);
            Check.That(matrix).IsNull();
        }

        [Fact]
        public void EqualsMatrixTest()
        {
            var matrix1 = new Matrix(4, new[]
            {
                new double[] {1, 2, 3, 4},
                new double[] {5, 6, 7, 8},
                new double[] {9, 8, 7, 6},
                new double[] {5, 4, 3, 2}
            });

            var matrix2 = new Matrix(4, new[]
            {
                new double[] {1, 2, 3, 4},
                new double[] {5, 6, 7, 8},
                new double[] {9, 8, 7, 6},
                new double[] {5, 4, 3, 2}
            });

            Check.That(matrix1.Equals(matrix2)).IsTrue();
            Check.That(matrix1).IsEqualTo(matrix2);
            Check.That(matrix1 == matrix2).IsTrue();
            Check.That(matrix1 != matrix2).IsFalse();

            var m3 = new Matrix(4);
            Check.That(matrix1).IsNotEqualTo(m3);
        }


        [Fact]
        public void MultiplyTest()
        {
            var matrix1 = new Matrix(4, new[]
            {
                new double[] {1, 2, 3, 4},
                new double[] {5, 6, 7, 8},
                new double[] {9, 8, 7, 6},
                new double[] {5, 4, 3, 2}
            });

            var matrix2 = new Matrix(4, new[]
            {
                new double[] {-2, 1, 2, 3},
                new double[] {3, 2, 1, -1},
                new double[] {4, 3, 6, 5},
                new double[] {1, 2, 7, 8}
            });
            var matrix3 = new Matrix(4, new[]
            {
                new double[] {20, 22, 50, 48},
                new double[] {44, 54, 114, 108},
                new double[] {40, 58, 110, 102},
                new double[] {16, 26, 46, 42}
            });

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
            var m = new Matrix(4, new[]
            {
                new double[] {0, 9, 3, 0},
                new double[] {9, 8, 0, 8},
                new double[] {1, 8, 5, 3},
                new double[] {0, 0, 5, 8}
            });
            var t = new Matrix(4, new[]
            {
                new double[] {0, 9, 1, 0},
                new double[] {9, 8, 8, 0},
                new double[] {3, 0, 5, 5},
                new double[] {0, 8, 3, 8}
            });

            Check.That(m.Transpose()).IsEqualTo(t);
        }

        [Fact]
        public void TransposeIdentityTest()
        {
            var m = Helper.CreateIdentity(4);
            Check.That(m.Transpose()).IsEqualTo(m);
        }

        [Fact]
        public void SubMatrix_3x3_Test()
        {
            var m = new Matrix(3, new[]
            {
                new double[] {1, 5, 0},
                new double[] {-3, 2, 7},
                new double[] {0, 6, -3},
            });

            var subA = m.SubMatrix(0, 2);
            var expectedSubA = new Matrix(2, new[]
            {
                new double[] {-3, 2},
                new double[] {0, 6}
            });
            Check.That(subA).IsEqualTo(expectedSubA);
        }

        [Fact]
        public void SubMatrix_4x4_Test()
        {
            var m = new Matrix(4,
                -6, 1, 1, 6,
                -8, 5, 8, 6,
                -1, 0, 8, 2,
                -7, 1, -1, 1
            );

            var subA = m.SubMatrix(2, 1);
            var expectedSubA = new Matrix(3,
                -6, 1, 6,
                -8, 8, 6,
                -7, -1, 1
            );
            Check.That(subA).IsEqualTo(expectedSubA);
        }

        [Fact]
        public void Determinant_2x2_Test()
        {
            var m = new Matrix(2,
                1, 5,
                -3, 2);
            Check.That(m.Determinant()).IsEqualTo(17);
        }

        [Fact]
        public void MinorTest()
        {
            var m = new Matrix(3,
                3, 5, 0,
                2, -1, -7,
                6, -1, 5);

            var subMat = m.SubMatrix(1, 0);
            Check.That(subMat.Determinant()).IsEqualTo(25);
            Check.That(m.Minor(1, 0)).IsEqualTo(25);
        }

        [Fact]
        public void CofactorTest()
        {
            var m = new Matrix(3,
                3, 5, 0,
                2, -1, -7,
                6, -1, 5);
            Check.That(m.Minor(0, 0)).IsEqualTo(-12);
            Check.That(m.Cofactor(0, 0)).IsEqualTo(-12);
            Check.That(m.Minor(1, 0)).IsEqualTo(25);
            Check.That(m.Cofactor(1, 0)).IsEqualTo(-25);
        }

        [Fact]
        public void Determinant_3x3_Test()
        {
            var m = new Matrix(3,
                1, 2, 6,
                -5, 8, -4,
                2, 6, 4);
            Check.That(m.Cofactor(0, 0)).IsEqualTo(56);
            Check.That(m.Cofactor(0, 1)).IsEqualTo(12);
            Check.That(m.Cofactor(0, 2)).IsEqualTo(-46);
            Check.That(m.Determinant()).IsEqualTo(-196);
        }

        [Fact]
        public void Determinant_4x4_Test()
        {
            var m = new Matrix(4,
                -2, -8, 3, 5,
                -3, 1, 7, 3,
                1, 2, -9, 6,
                -6, 7, 7, -9);
            Check.That(m.Cofactor(0, 0)).IsEqualTo(690);
            Check.That(m.Cofactor(0, 1)).IsEqualTo(447);
            Check.That(m.Cofactor(0, 2)).IsEqualTo(210);
            Check.That(m.Cofactor(0, 3)).IsEqualTo(51);
            Check.That(m.Determinant()).IsEqualTo(-4071);
        }

        [Fact]
        public void InvertibleTest()
        {
            var m = new Matrix(4,
                6, 4, 4, 4,
                5, 5, 7, 6,
                4, -9, 3, -7,
                9, 1, 7, -6);
            Check.That(m.Determinant()).IsEqualTo(-2120);
            Check.That(m.Invertible()).IsTrue();
        }

        [Fact]
        public void NotInvertibleTest()
        {
            var m = new Matrix(4,
                -4, 2, -2, -3,
                9, 6, 2, 6,
                0, -5, 1, -5,
                0, 0, 0, 0);
            Check.That(m.Determinant()).IsEqualTo(0);
            Check.That(m.Invertible()).IsFalse();
        }

        [Fact]
        public void Inverse_1_Test()
        {
            var m = new Matrix(4,
                -5, 2, 6, -8,
                1, -5, 1, 8,
                7, 7, -6, -7,
                1, -3, 7, 4);
            Check.That(m.Determinant()).IsEqualTo(532);
            Check.That(m.Cofactor(2, 3)).IsEqualTo(-160);

            var inv = m.Inverse();
            Check.That(inv[3, 2]).IsEqualTo(-160.0 / 532);

            Check.That(m.Cofactor(3, 2)).IsEqualTo(105);
            Check.That(inv[2, 3]).IsEqualTo(105.0 / 532);

            var expectedInv = new Matrix(4,
                0.21805, 0.45113, 0.24060, -0.04511,
                -0.80827, -1.45677, -0.44361, 0.52068,
                -0.07895, -0.22368, -0.05263, 0.19737,
                -0.52256, -0.81391, -0.30075, 0.30639);
            Check.That(inv).IsEqualTo(expectedInv);
        }

        [Fact]
        public void Inverse_2_Test()
        {
            var m = new Matrix(4,
                8, -5, 9, 2,
                7, 5, 6, 1,
                -6, 0, 9, 6,
                -3, 0, -9, -4);

            var expectedInv = new Matrix(4,
                -0.15385, -0.15385, -0.28205, -0.53846,
                -0.07692, 0.12308, 0.02564, 0.03077,
                0.35897, 0.35897, 0.43590, 0.92308,
                -0.69231, -0.69231, -0.76923, -1.92308
            );

            var inv = m.Inverse();
            Check.That(inv).IsEqualTo(expectedInv);
        }

        [Fact]
        public void Inverse_3_Test()
        {
            var m1 = new Matrix(4,
                3, -9, 7, 3,
                3, -8, 2, -9,
                -4, 4, 4, 1,
                -6, 5, -1, 1);

            var m2 = new Matrix(4,
                8, 2, 2, 2,
                3, -1, 7, 0,
                7, 0, 5, 4,
                6, -2, 0, 5);

            var m3 = m1 * m2;
            
            Check.That(m3 * m2.Inverse()).IsEqualTo(m1);
        }
    }
}