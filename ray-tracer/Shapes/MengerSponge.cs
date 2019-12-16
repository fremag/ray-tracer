using System;

namespace ray_tracer.Shapes
{
    public class MengerSponge : CsgDifference
    {
        public MengerSponge(int m)
        {
            var thing1 = MakeCubes(-1.5, -1.5, 1.5, 1.5, 0, m);
            var thing2 = MakeCubes(-1.5, -1.5, 1.5, 1.5, 0, m);
            var thing3 = MakeCubes(-1.5, -1.5, 1.5, 1.5, 0, m);
            var c1 = thing1;
            var c2 = thing2.Rotate(rx: Math.PI / 2);
            var c3 = thing3.Rotate(ry: Math.PI / 2);
            var u1 = new CsgUnion(c1, c2);
            var u2 = new CsgUnion(u1, c3);
            var cube = new Cube().Scale(1.5);
            Init(cube, u2);
        }
            
        private IShape MakeCubes(double x1, double y1, double x2,double y2, int n, int max)
        {
            var g = new Group();

            var deltaX = (x2 - x1) / 3;
            var deltaY = (x2 - x1) / 3;

            var sx = deltaX / 2;
            var sy = deltaY / 2;
            var cube = new Cube().Scale(sx, sy, 2.1);
            var tx = x1  + (x2 - x1) / 2;
            var ty = y1 + (y2 - y1) / 2;
            cube.Translate(tx, ty);

            g.Add(cube);
            if (n < max)
            {
                // left col
                var g1 = MakeCubes(x1 + 0*deltaX, y1+0*deltaY, x1 + 1*deltaX, y1 + 1*deltaY, n + 1, max);
                var g2 = MakeCubes(x1 + 0*deltaX, y1+1*deltaY, x1 + 1*deltaX, y1 + 2*deltaY, n + 1, max);
                var g3 = MakeCubes(x1 + 0*deltaX, y1+2*deltaY, x1 + 1*deltaX, y1 + 3*deltaY, n + 1, max);
                
                // center col
                var g4 = MakeCubes(x1 + 1*deltaX, y1+0*deltaY, x1 + 2*deltaX, y1 + 1*deltaY, n + 1, max);
                //var g5 = MakeCubes(x1 + 1*deltaX, y1+1*deltaY, x1 + 2*deltaX, y1 + 2*deltaY, n + 1, max);
                var g6 = MakeCubes(x1 + 1*deltaX, y1+2*deltaY, x1 + 2*deltaX, y1 + 3*deltaY, n + 1, max);
                
                // right col
                var g7 = MakeCubes(x1 + 2*deltaX, y1+0*deltaY, x1 + 3*deltaX, y1 + 1*deltaY, n + 1, max);
                var g8 = MakeCubes(x1 + 2*deltaX, y1+1*deltaY, x1 + 3*deltaX, y1 + 2*deltaY, n + 1, max);
                var g9 = MakeCubes(x1 + 2*deltaX, y1+2*deltaY, x1 + 3*deltaX, y1 + 3*deltaY, n + 1, max);

                g.Add(g1, g2, g3, g4, g6, g7, g8, g9);
            }

            return g;
        }
    }
}