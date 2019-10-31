using System;

namespace ray_tracer
{
    public class Matrix
    {
        private double[][] Values { get; }
        private Matrix Inversed { get; set; }
        public static Matrix Identity = new Matrix(4);

        static Matrix()
        {
            Identity[0, 0] = 1;
            Identity[1, 1] = 1;
            Identity[2, 2] = 1;
            Identity[3, 3] = 1;
        }
        
        public Matrix(int size)
        {
            Values = new double[size][];
            for (int i = 0; i < size; i++)
            {
                Values[i] = new double[size];
            }
        }

        public Matrix(int size, double[][] values) : this(size)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var row = values[i];
                for (int j = 0; j < row.Length; j++)
                {
                    Values[i][j] = row[j];
                }
            }
        }

        public Matrix(int size, params double[] values) : this(size)
        {
            int n = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Values[i][j] = values[n++];
                }
            }
        }

        public int Size => Values.Length;

        public double this[int i, int j]
        {
            get => Values[i][j];
            set => Values[i][j] = value;
        }

        public bool Equals(Matrix m)
        {
            if (m.Size != Size)
            {
                return false;
            }

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (! Helper.AreEquals(m[i, j], Values[i][j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object m)
        {
            if (m == null) return false;

            if (m is Matrix matrix)
            {
                return Equals(matrix);
            }

            return false;
        }

        public static bool operator ==(Matrix m1, Matrix m2)
        {
            if (ReferenceEquals(m1, null) && ReferenceEquals(m2, null))
            {
                return true;
            }
            if (! ReferenceEquals(m1, null) && ReferenceEquals(m2, null))
            {
                return false;
            }
            
            return !ReferenceEquals(m1, null) && m1.Equals(m2);
        }

        public static bool operator !=(Matrix m1, Matrix m2)
        {
            return !(m1 == m2);
        }

        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            var m = new Matrix(m1.Size);
            for (int i = 0; i < m1.Size; i++)
            {
                for (int j = 0; j < m1.Size; j++)
                {
                    double d = 0;
                    for (int k = 0; k < m1.Size; k++)
                    {
                        d += m1[i, k] * m2[k, j];
                    }

                    m[i, j] = d;
                }
            }

            return m;
        }

        public static Matrix operator *(Matrix m1, Matrix m2) => Multiply(m1, m2);

        public static Tuple Multiply(Matrix m, Tuple t)
        {
            double[] d = new double[4];
            for (int i = 0; i < m.Size; i++)
            {
                for (int j = 0; j < m.Size; j++)
                {
                    d[i] += m[i, j] * t[j];
                }
            }

            var v = new Tuple(d[0], d[1], d[2], d[3]);
            return v;
        }
        
        public static Matrix operator +(Matrix m1, Matrix m2) => Add(m1, m2);
        
        public static Matrix Add(Matrix m1, Matrix m2)
        {
            var m= new Matrix(m1.Size);
            for (int i = 0; i < m1.Size; i++)
            {
                for (int j = 0; j < m1.Size; j++)
                {
                    m[i,j] = m1[i, j] + m2[i, j];
                }
            }

            return m;
        }
        public static Matrix Multiply(Matrix m, double d)
        {
            var result = new Matrix(m.Size);
            for (int i = 0; i < m.Size; i++)
            {
                for (int j = 0; j < m.Size; j++)
                {
                    result[i,j] = d * m[i, j];
                }
            }

            return result;
        }
        
        public static Tuple operator *(Matrix m1, Tuple t) => Multiply(m1, t);
        public static Tuple operator *(Tuple t, Matrix m1) => Multiply(m1, t);
        public static Matrix operator *(double d, Matrix m) => Multiply(m, d);
        public static Matrix operator *(Matrix m, double d) => Multiply(m, d);

        public Matrix Transpose()
        {
            var m = new Matrix(Size);
            for(int i=0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    m[i,j] = this[j, i];
                }
            }

            return m;
        }

        public Matrix SubMatrix(int row, int column)
        {
            var m= new Matrix(Size-1);
            for(int i=0; i < Size; i++)
            {
                if (i == row)
                {
                    continue;
                }

                int deltaRow = 0;
                if (i > row)
                {
                    deltaRow = -1;
                }
                
                for (int j = 0; j < Size; j++)
                {
                    if (j == column)
                    {
                        continue;
                    }
                    int deltaCol = 0;
                    if (j > column)
                    {
                        deltaCol= -1;
                    }

                    var subRow = i+deltaRow;
                    var subCol = j+deltaCol;
                    m[subRow, subCol] = this[i, j];
                }
            }

            return m;
        }

        public double Determinant()
        {
            double det = 0;
            if (Size == 2)
            {
                det = this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
            }
            else
            {
                for (int i = 0; i < Size; i++)
                {
                    det += this[0, i] * Cofactor(0, i);
                }
            }

            return det;
        }

        public double Minor(int row, int column)
        {
            var subMat = SubMatrix(row, column);
            var minor = subMat.Determinant();
            return minor;
        }

        public double Cofactor(int row, int column)
        {
            var n = (row + column) % 2;
            var coeff = 1 - 2 * n;
            var minor = Minor(row, column);
            return coeff * minor;
        }

        public bool Invertible() => Determinant() != 0;

        public Matrix Inverse()
        {
            var b = Inversed != null;
            if (b)
            {
                return Inversed;
            }
            
            var det = Determinant();
            if (det == 0)
            {
                throw new InvalidOperationException("Can not inverse matrix: determinant == 0 !");
            }

            Inversed = new Matrix(Size);
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    var cof = Cofactor(row, col);
                    Inversed[col, row] = cof / det;
                }
            }

            return Inversed;
        }
    }
}