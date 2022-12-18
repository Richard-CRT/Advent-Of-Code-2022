// 05:04

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoCUtilities.GetInputLines();
List<(int, int, int)> CubeList = new List<(int, int, int)>();
HashSet<(int, int, int)> Cubes = new HashSet<(int, int, int)>(
    inputList.Select(x =>
    {
        string[] s = x.Split(',');
        (int, int, int) n = (int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
        CubeList.Add(n);
        return n;
    })
    );


void P1()
{
    int sidesExposed = 0;
    foreach ((int sx, int sy, int sz) in Cubes)
    {
        List<(int, int, int)> coordsToCheck = new List<(int, int, int)>();
        coordsToCheck.Add((sx - 1, sy, sz));
        coordsToCheck.Add((sx + 1, sy, sz));
        coordsToCheck.Add((sx, sy - 1, sz));
        coordsToCheck.Add((sx, sy + 1, sz));
        coordsToCheck.Add((sx, sy, sz - 1));
        coordsToCheck.Add((sx, sy, sz + 1));

        foreach (var c in coordsToCheck)
        {
            if (!Cubes.Contains(c))
            {
                sidesExposed++;
            }
        }
    }
    Console.WriteLine(sidesExposed);
    Console.ReadLine();
}

void P2()
{
    HashSet<(int, int, int)> Cubes2 = new HashSet<(int, int, int)>();

    int minx = CubeList.MinBy(i => i.Item1).Item1;
    int maxx = CubeList.MaxBy(i => i.Item1).Item1;
    int miny = CubeList.MinBy(i => i.Item2).Item2;
    int maxy = CubeList.MaxBy(i => i.Item2).Item2;
    int minz = CubeList.MinBy(i => i.Item3).Item3;
    int maxz = CubeList.MaxBy(i => i.Item3).Item3;

    for (int x = minx; x <= maxx; x++)
    {
        for (int y = miny; y <= maxy; y++)
        {
            for (int z = minz; z <= maxz; z++)
            {
                Cubes2.Add((x, y, z));
            }
        }
    }

    // approach from all 6 sides compressing the cube
    for (int z = minz; z <= maxz; z++)
    {
        // approaching from front
        for (int x = minx; x <= maxx; x++)
        {
            for (int y = miny; y <= maxy; y++)
            {
                var t = (x, y, z);
                var p = (x, y, z - 1);
                if (!Cubes.Contains(t) && !Cubes2.Contains(p))
                {
                    Cubes2.Remove(t);
                }
            }
        }
    }
    for (int z = maxz; z >= minz; z--)
    {
        // approaching from back
        for (int x = minx; x <= maxx; x++)
        {
            for (int y = miny; y <= maxy; y++)
            {
                var t = (x, y, z);
                var p = (x, y, z + 1);
                if (!Cubes.Contains(t) && !Cubes2.Contains(p))
                {
                    Cubes2.Remove(t);
                }
            }
        }
    }
    for (int x = minx; x <= maxx; x++)
    {
        // approaching from left
        for (int z = minz; z <= maxz; z++)
        {
            for (int y = miny; y <= maxy; y++)
            {
                var t = (x, y, z);
                var p = (x - 1, y, z);
                if (!Cubes.Contains(t) && !Cubes2.Contains(p))
                {
                    Cubes2.Remove(t);
                }
            }
        }
    }
    for (int x = maxx; x >= minx; x--)
    {
        // approaching from left
        for (int z = minz; z <= maxz; z++)
        {
            for (int y = miny; y <= maxy; y++)
            {
                var t = (x, y, z);
                var p = (x + 1, y, z);
                if (!Cubes.Contains(t) && !Cubes2.Contains(p))
                {
                    Cubes2.Remove(t);
                }
            }
        }
    }
    for (int y = miny; y <= maxy; y++)
    {
        // approaching from bottom
        for (int z = minz; z <= maxz; z++)
        {
            for (int x = minx; x <= maxx; x++)
            {
                var t = (x, y, z);
                var p = (x, y - 1, z);
                if (!Cubes.Contains(t) && !Cubes2.Contains(p))
                {
                    Cubes2.Remove(t);
                }
            }
        }
    }
    for (int y = maxy; y >= miny; y--)
    {
        // approaching from bottom
        for (int z = minz; z <= maxz; z++)
        {
            for (int x = minx; x <= maxx; x++)
            {
                var t = (x, y, z);
                var p = (x, y + 1, z);
                if (!Cubes.Contains(t) && !Cubes2.Contains(p))
                {
                    Cubes2.Remove(t);
                }
            }
        }
    }

    int encasedCubes = Cubes2.Count - Cubes.Count;


    int sidesExposed = 0;
    foreach ((int sx, int sy, int sz) in Cubes2)
    {
        List<(int, int, int)> coordsToCheck = new List<(int, int, int)>();
        coordsToCheck.Add((sx - 1, sy, sz));
        coordsToCheck.Add((sx + 1, sy, sz));
        coordsToCheck.Add((sx, sy - 1, sz));
        coordsToCheck.Add((sx, sy + 1, sz));
        coordsToCheck.Add((sx, sy, sz - 1));
        coordsToCheck.Add((sx, sy, sz + 1));

        foreach (var c in coordsToCheck)
        {
            if (!Cubes2.Contains(c))
            {
                sidesExposed++;
            }
        }
    }
    Console.WriteLine(sidesExposed);
    Console.ReadLine();
}

P1();
P2();