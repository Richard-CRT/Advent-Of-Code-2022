using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<(char, int)> Instructions = inputList.Select(s => { string[] split = s.Split(' '); return (split[0][0], int.Parse(split[1])); }).ToList();

void P1()
{
    HashSet<(int, int)> tailPositionHistory = new HashSet<(int, int)>();
    int headX = 0;
    int headY = 0;
    int tailX = 0;
    int tailY = 0;

    tailPositionHistory.Add((tailX, tailY));
    foreach ((char direction, int count) in Instructions)
    {
        for (int i = 0; i < count; i++)
        {
            int newHeadX = headX;
            int newHeadY = headY;
            switch (direction)
            {
                case 'U': newHeadY--; break;
                case 'L': newHeadX--; break;
                case 'R': newHeadX++; break;
                case 'D': newHeadY++; break;
            }

            if (Math.Abs(newHeadX - tailX) > 1 || Math.Abs(newHeadY - tailY) > 1)
            {
                tailX = headX;
                tailY = headY;

                tailPositionHistory.Add((tailX, tailY));
            }

            headX = newHeadX;
            headY = newHeadY;
        }
    }

    Console.WriteLine(tailPositionHistory.Count);
    Console.ReadLine();
}

void P2()
{
    (int, int)[] buffer = new (int, int)[10];
    for (int i = 0; i < 10; i++)
        buffer[0] = (0, 0);
    HashSet<(int, int)> tailPositionHistory = new HashSet<(int, int)>();
    tailPositionHistory.Add((0, 0));

    int headX = 0;
    int headY = 0;
    foreach ((char direction, int count) in Instructions)
    {
        for (int i = 0; i < count; i++)
        {
            int newHeadX = headX;
            int newHeadY = headY;
            switch (direction)
            {
                case 'U': newHeadY--; break;
                case 'L': newHeadX--; break;
                case 'R': newHeadX++; break;
                case 'D': newHeadY++; break;
            }

            (int, int)[] previousBuffer = ((int, int)[])buffer.Clone();

            buffer[0] = (newHeadX, newHeadY);
            for (int j = 1; j < buffer.Length; j++)
            {
                (int x, int y) = buffer[j];
                (int prevX, int prevY) = buffer[j - 1];
                if (Math.Abs(prevX - x) > 1 || Math.Abs(prevY - y) > 1)
                {
                    if (prevX - x == 2 && y == prevY)
                        buffer[j] = (x + 1, y);
                    else if (prevX - x == -2 && y == prevY)
                        buffer[j] = (x - 1, y);
                    else if (prevY - y == 2 && x == prevX)
                        buffer[j] = (x, y + 1);
                    else if (prevY - y == -2 && x == prevX)
                        buffer[j] = (x, y - 1);
                    else
                    {
                        int xDir = prevX - x > 0 ? 1 : -1;
                        int yDir = prevY - y > 0 ? 1 : -1;

                        int newX = x;
                        int newY = y;

                        newX = x + xDir;
                        newY = y + yDir;
                        buffer[j] = (newX, newY);
                    }

                    if (j == buffer.Length - 1)
                    {
                        tailPositionHistory.Add(buffer[buffer.Length - 1]);
                    }
                }
            }

            /*
            int minX = buffer.Select(x => x.Item1).Min();
            int maxX = buffer.Select(x => x.Item1).Max();
            int minY = buffer.Select(x => x.Item2).Min();
            int maxY = buffer.Select(x => x.Item2).Max();

            for (int y = -20; y <= 20; y++)
            {
                for (int x = -20; x <= 20; x++)
                {
                    if (buffer.Contains((x, y)))
                        Console.Write(Array.IndexOf(buffer, (x, y)));
                    else
                        Console.Write(".");
                }
                Console.WriteLine();
            }
            Console.ReadLine();
            */

            headX = newHeadX;
            headY = newHeadY;
        }
    }

    Console.WriteLine(tailPositionHistory.Count);
    Console.ReadLine();
}

P1();
P2();