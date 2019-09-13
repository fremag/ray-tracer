using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using ray_tracer;
using ray_tracer.Shapes;

namespace ray_tracer
{
    public static class Helper
    {
        public static readonly double Epsilon = 1e-5;

        public static Matrix CreateMatrix(int size) => new Matrix(size);
        public static Tuple CreatePoint(double x, double y, double z) => new Tuple(x, y, z, 1);
        public static Tuple CreateVector(double x, double y, double z) => new Tuple(x, y, z, 0);
        public static bool IsPoint(this Tuple tuple) => tuple.W == 1;
        public static bool IsVector(this Tuple tuple) => tuple.W == 0;
        public static bool AreEquals(double d1, double d2) => Math.Abs(d1 - d2) < Epsilon;

        public static Matrix CreateIdentity(int n = 4)
        {
            var m = new Matrix(n);
            for (int i = 0; i < n; i++)
            {
                m[i, i] = 1;
            }

            return m;
        } 
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

        public static Matrix Translation(double x, double y, double z)
        {
            return new Matrix(4, 
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
                );
        }

        public static Matrix Scaling(double x, double y, double z)
        {
            return new Matrix(4, 
                x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1
            );
        }
        
        public static Matrix RotationX( double a)
        {
            return new Matrix(4, 
                1, 0, 0, 0,
                0, Math.Cos(a), -Math.Sin(a), 0,
                0, Math.Sin(a), Math.Cos(a), 0,
                0, 0, 0, 1
            );
        }
        public static Matrix RotationY( double a)
        {
            return new Matrix(4, 
                Math.Cos(a), 0, Math.Sin(a), 0,
                0, 1, 0, 0,
                -Math.Sin(a), 0, Math.Cos(a), 0,
                0, 0, 0, 1
            );
        }
        
        public static Matrix RotationZ( double a)
        {
            return new Matrix(4, 
                Math.Cos(a), -Math.Sin(a), 0, 0,
                Math.Sin(a),  Math.Cos(a), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }
        
        public static Matrix Shearing( double xy, double xz, double yx, double yz, double zx, double zy)
        {
            return new Matrix(4,
                1, xy, xz, 0,
                yx, 1, yz, 0,
                zx, zy, 1, 0,
                0, 0, 0, 1
            );
        }

        public static Ray Ray(Tuple origin, Tuple direction) => new Ray(origin, direction);
        public static Sphere Sphere() => new Sphere();

        public static Intersection Intersection(double t, IShape s) => new Intersection(t, s);
        public static Intersections Intersections() => new Intersections();
        public static Intersections Intersections(params Intersection[] intersections) => new Intersections(intersections);

        public static Matrix ViewTransform(Tuple from, Tuple to, Tuple up)
        {
            var forward = (to - from).Normalize();
            var upNorm = up.Normalize();
            var left = forward * upNorm;
            var trueUp = left * forward;
            var orientation = new Matrix(4, left.X, left.Y, left.Z, 0,
                trueUp.X, trueUp.Y, trueUp.Z, 0,
                -forward.X, -forward.Y, -forward.Z, 0,
                0, 0, 0, 1);
            var viewTransform = orientation * Translation(-from.X, -from.Y, -from.Z);
            return viewTransform;
        }

        public static void Display(string file)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var p = Process.Start(@"C:\Program Files (x86)\XnView\xnview.exe", file);
            while (!p.HasExited || sw.ElapsedMilliseconds < 10_000)
            {
                Thread.Sleep(100);
            }

            if (!p.HasExited)
            {
                p.Kill();
            }
        }

        public static Sphere CreateGlassSphere()
        {
            var s = new Sphere {Material = {Transparency = 1.0, RefractiveIndex = 1.5}};
            return s;
        } 
    }
}