namespace raytracer
{
    public class Matrix
    {
        private double[][] Values { get; }

        public Matrix(int size)
        {
            Values = new double[4][];
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
                    if (m[i, j] != Values[i][j])
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
        
        public static Tuple operator *(Matrix m1, Tuple t) => Multiply(m1, t);

        public Matrix Transpose()
        {
            var m = new Matrix(Size);
            for(int i=0; i < Size; i++)
            for (int j = 0; j < Size; j++)
            {
                m[i,j] = this[j, i];
            }

            return m;
        }
    }
}