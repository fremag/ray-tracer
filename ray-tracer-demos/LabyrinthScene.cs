using System;
using System.IO;
using ray_tracer;
using ray_tracer.Patterns;
using ray_tracer.Shapes;

namespace ray_tracer_demos
{
    public class LabyrinthScene : AbstractScene
    {
        public LabyrinthScene()
        {
            CameraParameters.Clear();
            CameraParameters.Add(new CameraParameters{Name = "Default", Width = 640, Height = 400,
                CameraX = 35, CameraY = 25, CameraZ = -35,
                LookX = 14, LookY = 0, LookZ = -13});
        }

        public override void InitWorld()
        {
            IShape floor = new Plane
            {
                Material = new Material(new CheckerPattern(Color.Black, Color.White))
            };
            Add(floor);

            var w = 60;
            var lab = ComputeLabyrinth(w, w);
            for (int i = 0; i < w; i++)
            {
                Group g = new Group();
                for (int j = 0; j < w; j++)
                {
                    if (lab[i][j] == 0)
                    {
                        var cube = new Cube().Scale(0.5).Translate(tz: j-w/2);
                        cube.Material.Pattern = new SolidPattern(Color.White *0.8);
                        g.Add(cube);
                    }
                }

                if (g.Count > 0)
                {
                    Add(g.Translate(tx: i-w/2));
                }
            }

            var point = Helper.CreatePoint(0, 1, -0.75) * w*0.75;
            Light(0, w/3.0, 0, Color.White);
        }

        public static int[][] ComputeLabyrinth(int w, int h)
        {
            int[][] laby = new int[h][];
            for (int i = 0; i < h; i++)
            {
                laby[i] = new int[w];
                for (int j = 0; j < w; j++)
                {
                    laby[i][j] = 1;
                }
            }

            Random rand = new Random(0);
            laby[0][0] = 0;
            CreateLabyrinth(laby, rand, 0, 0);

            return laby;
        }

        private static void CreateLabyrinth(int[][] maze, Random rand, int r, int c)
        {
            // 4 random directions
            int randDirs = rand.Next() % 4;
            // Examine each direction
            for (int i = 0; i < 4; i++)
            {
                int dir = (randDirs + i) % 4;
                switch (dir)
                {
                    case 0: // Up
                        //　Whether 2 cells up is out or not
                        if (r - 2 <= 0)
                            continue;
                        if (maze[r - 2][c] != 0)
                        {
                            maze[r - 2][c] = 0;
                            maze[r - 1][c] = 0;
                            CreateLabyrinth(maze, rand, r - 2, c);
                        }

                        break;
                    case 1: // Right
                        // Whether 2 cells to the right is out or not
                        if (c + 2 >= maze[0].Length - 1)
                            continue;
                        if (maze[r][c + 2] != 0)
                        {
                            maze[r][c + 2] = 0;
                            maze[r][c + 1] = 0;
                            CreateLabyrinth(maze, rand, r, c + 2);
                        }

                        break;
                    case 2: // Down
                        // Whether 2 cells down is out or not
                        if (r + 2 >= maze.Length - 1)
                            continue;
                        if (maze[r + 2][c] != 0)
                        {
                            maze[r + 2][c] = 0;
                            maze[r + 1][c] = 0;
                            CreateLabyrinth(maze, rand, r + 2, c);
                        }

                        break;
                    case 3: // Left
                        // Whether 2 cells to the left is out or not
                        if (c - 2 <= 0)
                            continue;
                        if (maze[r][c - 2] != 0)
                        {
                            maze[r][c - 2] = 0;
                            maze[r][c - 1] = 0;
                            CreateLabyrinth(maze, rand, r, c - 2);
                        }

                        break;
                }
            }
        }

        public static void PrintLabyrinth(int[][] laby)
        {
            for (int i = 0; i < laby.Length; i++)
            {
                for (int j = 0; j < laby[i].Length; j++)
                {
                    var c = laby[i][j] == 0 ? '█' : ' ';
                    Console.Write(c);
                }

                Console.WriteLine();
            }
        }
 
    }
}