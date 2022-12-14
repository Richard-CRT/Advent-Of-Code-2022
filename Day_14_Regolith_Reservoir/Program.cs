// 05:37

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoCUtilities.GetInputLines();
Dictionary<(int, int), Cell> Map = new Dictionary<(int, int), Cell>();
int RockMaxY = int.MinValue;
foreach (string s in inputList)
{
    string[] split = s.Split(" -> ");
    for (int i = 0; i < split.Length - 1; i++)
    {
        string[] startSplit = split[i].Split(',');
        string[] endSplit = split[i + 1].Split(',');
        int startY = int.Parse(startSplit[1]);
        int endY = int.Parse(endSplit[1]);
        int startX = int.Parse(startSplit[0]);
        int endX = int.Parse(endSplit[0]);
        if (startX == endX)
        {
            int dir = startY > endY ? -1 : 1;
            for (int y = startY; y != endY + dir; y += dir)
            {
                Map[(startX, y)] = Cell.Rock;
                if (y > RockMaxY)
                    RockMaxY = y;
            }
        }
        else if (startY == endY)
        {
            int dir = startX > endX ? -1 : 1;
            if (startY > RockMaxY)
                RockMaxY = startY;
            for (int x = startX; x != endX + dir; x += dir)
            {
                Map[(x, startY)] = Cell.Rock;
            }
        }
        else
            throw new NotImplementedException();
    }
}

void P1()
{
    int sandUnits = 0;
    while (true)
    {
        int sandX = 500;
        int sandY = 0;
        Map[(sandX, sandY)] = Cell.Sand;
        bool rest = false;
        while (!rest)
        {
            if (Map.GetValueOrDefault((sandX, sandY + 1), Cell.Air) == Cell.Air)
            {
                Map[(sandX, sandY)] = Cell.Air;
                Map[(sandX, sandY + 1)] = Cell.Sand;
                sandY = sandY + 1;

                if (sandY >= RockMaxY)
                    falling = true;
            }
            else if (Map.GetValueOrDefault((sandX - 1, sandY + 1), Cell.Air) == Cell.Air)
            {
                Map[(sandX, sandY)] = Cell.Air;
                Map[(sandX - 1, sandY + 1)] = Cell.Sand;
                sandX = sandX - 1;
                sandY = sandY + 1;
            }
            else if (Map.GetValueOrDefault((sandX + 1, sandY + 1), Cell.Air) == Cell.Air)
            {
                Map[(sandX, sandY)] = Cell.Air;
                Map[(sandX + 1, sandY + 1)] = Cell.Sand;
                sandX = sandX + 1;
                sandY = sandY + 1;
            }
            else
            {
                rest = true;
            }
        }

        if (falling)
            break;

        sandUnits++;
    }
    Console.WriteLine(sandUnits);
    Console.ReadLine();
}

void P2()
{
    for (int i = Map.Count - 1; i >= 0; i--)
    {
        if (Map[Map.Keys.ToArray()[i]] == Cell.Sand)
            Map.Remove(Map.Keys.ToArray()[i]);
    }
    int sandUnits = 0;
    while (Map[(500, 0)] == Cell.Air)
    {
        int sandX = 500;
        int sandY = 0;
        Map[(sandX, sandY)] = Cell.Sand;
        bool rest = false;
        bool falling = false;
        while (!rest && !falling)
        {
            if (sandY == RockMaxY + 1)
            {
                Map[(sandX, sandY + 1)] = Cell.Rock;
                Map[(sandX - 1, sandY + 1)] = Cell.Rock;
                Map[(sandX + 1, sandY + 1)] = Cell.Rock;
            }
            if (Map.GetValueOrDefault((sandX, sandY + 1), Cell.Air) == Cell.Air)
            {
                Map[(sandX, sandY)] = Cell.Air;
                Map[(sandX, sandY + 1)] = Cell.Sand;
                sandY = sandY + 1;
            }
            else if (Map.GetValueOrDefault((sandX - 1, sandY + 1), Cell.Air) == Cell.Air)
            {
                Map[(sandX, sandY)] = Cell.Air;
                Map[(sandX - 1, sandY + 1)] = Cell.Sand;
                sandX = sandX - 1;
                sandY = sandY + 1;
            }
            else if (Map.GetValueOrDefault((sandX + 1, sandY + 1), Cell.Air) == Cell.Air)
            {
                Map[(sandX, sandY)] = Cell.Air;
                Map[(sandX + 1, sandY + 1)] = Cell.Sand;
                sandX = sandX + 1;
                sandY = sandY + 1;
            }
            else
            {
                rest = true;
            }
        }

        sandUnits++;
    }
    Console.WriteLine(sandUnits);
    Console.ReadLine();
}

P1();
P2();

public enum Cell
{
    Air,
    Sand,
    Rock,
}