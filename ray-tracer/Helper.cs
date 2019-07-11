using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ray_tracer;

namespace raytracer
{
    public static class Helper
    {
        const double Epsilon = 1e-6;

        public static Matrix CreateMatrix(int size) => new Matrix(size);
        public static Tuple CreatePoint(double x, double y, double z) => new Tuple(x, y, z, 1);
        public static Tuple CreateVector(double x, double y, double z) => new Tuple(x, y, z, 0);
        public static bool IsPoint(this Tuple tuple) => tuple.W == 1;
        public static bool IsVector(this Tuple tuple) => tuple.W == 0;
        public static bool AreEquals(double d1, double d2) => Math.Abs(d1 - d2) < Epsilon;

        public static IEnumerable<string> ToPPM(this Canvas canvas)
        {
            yield return "P3";
            yield return $"{canvas.Width} {canvas.Height}";
            yield return "255";

            StringBuilder row = new StringBuilder(100);
            for (int i = 0; i < canvas.Height; i++)
            {
                row.Length = 0;
                for (int j = 0; j < canvas.Width; j++)
                {
                    var pixel = canvas.GetPixel(j, i);
                    var c = Color.Normalize(pixel.Red);
                    if (!PrintComponent(row, c))
                    {
                        yield return row.ToString();
                        row.Length = 0;
                        row.Append(c);
                    }
                    
                    c = Color.Normalize(pixel.Green);
                    if (!PrintComponent(row, c))
                    {
                        yield return row.ToString();
                        row.Length = 0;
                        row.Append(c);
                    }
                    
                    c = Color.Normalize(pixel.Blue);
                    if (!PrintComponent(row, c))
                    {
                        yield return row.ToString();
                        row.Length = 0;
                        row.Append(c);
                    }
                }
                if (row.Length != 0)
                {
                    yield return row.ToString();
                }
            }
        }

        private static bool PrintComponent(StringBuilder row, int normalizedColorComponent)
        {
            if (row.Length == 0)
            {
                row.Append(normalizedColorComponent);
                return true;
            }
            
            int l = normalizedColorComponent < 10 ? 1 : normalizedColorComponent < 100 ? 2 : 3;
            if (row.Length + l + 1 > 70)
            {
                return false;
            }
            row.Append(' ');
            row.Append(normalizedColorComponent);
            return true;
        }

        public static void SavePPM(this Canvas canvas, string filePath)
        {
            File.WriteAllLines(filePath, canvas.ToPPM());
        } 
    }
}