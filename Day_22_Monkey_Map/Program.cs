// 11:30

//#define EXAMPLE

using AdventOfCodeUtilities;
using System.Drawing;
using System.Text.RegularExpressions;

List<string> inputList = AoCUtilities.GetInputLines();

int emptyLineIndex = inputList.IndexOf("");

string sInstructions = inputList[emptyLineIndex + 1];
List<int> Distances = new List<int>();
List<int> Turns = new List<int>();
MatchCollection mc = AoCUtilities.RegexMatch(sInstructions, @"(\d+|R|L)");
foreach (Match m in mc)
{
    string s = m.Groups[1].Value;
    if (s == "R")
        Turns.Add(1);
    else if (s == "L")
        Turns.Add(-1);
    else
        Distances.Add(int.Parse(s));
}

#if EXAMPLE
Side.Dim = 4;
#else
Side.Dim = 50;
#endif

Dictionary<(int, int), Side> Sides = new Dictionary<(int, int), Side>();

for (int yCoord = 0; yCoord < 6; yCoord++)
{
    int y = yCoord * Side.Dim;
    if (y + Side.Dim <= inputList.Count)
    {
        for (int xCoord = 0; xCoord < 6; xCoord++)
        {
            int x = xCoord * Side.Dim;
            if (x + Side.Dim <= inputList[y].Length)
            {
                if (inputList[y][x] != ' ')
                    Sides[(xCoord, yCoord)] = new Side(inputList, xCoord, yCoord);
            }
        }
    }
}

// Preprocess the dumb part 1 wrapping
foreach (var kvp in Sides)
{
    (int xCoord, int yCoord) = kvp.Key;
    Side side = kvp.Value;

    Side leftSide;
    if (Sides.ContainsKey((xCoord - 1, yCoord)))
        leftSide = Sides[(xCoord - 1, yCoord)];
    else
        leftSide = Sides.Where(kvp => kvp.Key.Item2 == yCoord).MaxBy(kvp => kvp.Key.Item1).Value;

    Side rightSide;
    if (Sides.ContainsKey((xCoord + 1, yCoord)))
        rightSide = Sides[(xCoord + 1, yCoord)];
    else
        rightSide = Sides.Where(kvp => kvp.Key.Item2 == yCoord).MinBy(kvp => kvp.Key.Item1).Value;

    Side upSide;
    if (Sides.ContainsKey((xCoord, yCoord - 1)))
        upSide = Sides[(xCoord, yCoord - 1)];
    else
        upSide = Sides.Where(kvp => kvp.Key.Item1 == xCoord).MaxBy(kvp => kvp.Key.Item2).Value;

    Side downSide;
    if (Sides.ContainsKey((xCoord, yCoord + 1)))
        downSide = Sides[(xCoord, yCoord + 1)];
    else
        downSide = Sides.Where(kvp => kvp.Key.Item1 == xCoord).MinBy(kvp => kvp.Key.Item2).Value;

    for (int x = 0; x < Side.Dim; x++)
    {
        side.P1WrapKey[(x, -1)] = (upSide, x, Side.Dim - 1, Direction.Up);
        side.P1WrapKey[(x, Side.Dim)] = (downSide, x, 0, Direction.Down);
    }
    for (int y = 0; y < Side.Dim; y++)
    {
        side.P1WrapKey[(-1, y)] = (leftSide, Side.Dim - 1, y, Direction.Left);
        side.P1WrapKey[(Side.Dim, y)] = (rightSide, 0, y, Direction.Right);
    }
}

// Preprocess the easy part 2 wrapping where the adjacent sides are already there
foreach (var kvp in Sides)
{
    (int xCoord, int yCoord) = kvp.Key;
    Side side = kvp.Value;

    if (Sides.ContainsKey((xCoord - 1, yCoord)))
    {
        for (int y = 0; y < Side.Dim; y++)
            side.P2WrapKey[(-1, y)] = (Sides[(xCoord - 1, yCoord)], Side.Dim - 1, y, Direction.Left);
    }
    if (Sides.ContainsKey((xCoord + 1, yCoord)))
    {
        for (int y = 0; y < Side.Dim; y++)
            side.P2WrapKey[(Side.Dim, y)] = (Sides[(xCoord + 1, yCoord)], 0, y, Direction.Right);
    }
    if (Sides.ContainsKey((xCoord, yCoord - 1)))
    {
        for (int x = 0; x < Side.Dim; x++)
            side.P2WrapKey[(x, -1)] = (Sides[(xCoord, yCoord - 1)], x, Side.Dim - 1, Direction.Up);
    }
    if (Sides.ContainsKey((xCoord, yCoord + 1)))
    {
        for (int x = 0; x < Side.Dim; x++)
            side.P2WrapKey[(x, Side.Dim)] = (Sides[(xCoord, yCoord + 1)], x, 0, Direction.Down);
    }
}


#if EXAMPLE
List<(Side, Direction, Side, Direction)> relations = new List<(Side, Direction, Side, Direction)>()
{
    (Sides[(2,0)], Direction.Left, Sides[(1,1)], Direction.Up),
    (Sides[(2,0)], Direction.Up, Sides[(0,1)], Direction.Up),
    (Sides[(2,0)], Direction.Right, Sides[(3,2)], Direction.Right),
    (Sides[(0,1)], Direction.Left, Sides[(3,2)], Direction.Down),
    (Sides[(0,1)], Direction.Down, Sides[(2,2)], Direction.Down),
    (Sides[(2,2)], Direction.Left, Sides[(1,1)], Direction.Down),
    (Sides[(2,1)], Direction.Right, Sides[(3,2)], Direction.Up),
};
#else
List<(Side, Direction, Side, Direction)> relations = new List<(Side, Direction, Side, Direction)>()
{
    (Sides[(1,1)], Direction.Left, Sides[(0,2)], Direction.Up),
    (Sides[(1,1)], Direction.Right, Sides[(2,0)], Direction.Down),
    (Sides[(1,2)], Direction.Right, Sides[(2,0)], Direction.Right),
    (Sides[(0,3)], Direction.Right, Sides[(1,2)], Direction.Down),
    (Sides[(0,2)], Direction.Left, Sides[(1,0)], Direction.Left),
    (Sides[(0,3)], Direction.Left, Sides[(1,0)], Direction.Up),
    (Sides[(0,3)], Direction.Down, Sides[(2,0)], Direction.Up),
};
#endif

// Preprocess the complex part 2 wrapping which requires knowledge of the input
foreach ((Side sideA, Direction sideASide, Side sideB, Direction sideBSide) in relations)
{
    Side side1;
    Direction side1Side;
    Side side2;
    Direction side2Side;
    if (
        sideBSide == Direction.Left && sideASide == Direction.Up ||
        sideBSide == Direction.Left && sideASide == Direction.Down ||
        sideBSide == Direction.Left && sideASide == Direction.Right ||
        sideBSide == Direction.Right && sideASide == Direction.Up ||
        sideBSide == Direction.Right && sideASide == Direction.Down ||
        sideBSide == Direction.Down && sideASide == Direction.Up
        )
    {
        side1 = sideB;
        side1Side = sideBSide;
        side2 = sideA;
        side2Side = sideASide;
    }
    else
    {
        side1 = sideA;
        side1Side = sideASide;
        side2 = sideB;
        side2Side = sideBSide;
    }

    for (int i = 0; i < Side.Dim; i++)
    {
        (int, int) target1;
        (int, int) target2;
        switch (side1Side)
        {
            case Direction.Up: target1 = (i, -1); break;
            case Direction.Down: target1 = (i, Side.Dim); break;
            case Direction.Left: target1 = (-1, i); break;
            case Direction.Right: target1 = (Side.Dim, i); break;
            default: throw new Exception();
        }
        switch (side2Side)
        {
            case Direction.Up: target2 = (i, -1); break;
            case Direction.Down: target2 = (i, Side.Dim); break;
            case Direction.Left: target2 = (-1, i); break;
            case Direction.Right: target2 = (Side.Dim, i); break;
            default: throw new Exception();
        }

        // LL
        // UU
        // RR
        // DD

        // LU
        // LD
        // LR

        // RU
        // RD

        // DU

        if (side1Side == Direction.Left && side2Side == Direction.Left)
        {
            side1.P2WrapKey[target1] = (side2, 0, Side.Dim - 1 - i, Direction.Right);
            side2.P2WrapKey[target2] = (side1, 0, Side.Dim - 1 - i, Direction.Right);
        }
        else if (side1Side == Direction.Up && side2Side == Direction.Up)
        {
            side1.P2WrapKey[target1] = (side2, Side.Dim - 1 - i, 0, Direction.Down);
            side2.P2WrapKey[target2] = (side1, Side.Dim - 1 - i, 0, Direction.Down);
        }
        else if (side1Side == Direction.Right && side2Side == Direction.Right)
        {
            side1.P2WrapKey[target1] = (side2, Side.Dim - 1, Side.Dim - 1 - i, Direction.Left);
            side2.P2WrapKey[target2] = (side1, Side.Dim - 1, Side.Dim - 1 - i, Direction.Left);
        }
        else if (side1Side == Direction.Down && side2Side == Direction.Down)
        {
            side1.P2WrapKey[target1] = (side2, Side.Dim - 1 - i, Side.Dim - 1, Direction.Up);
            side2.P2WrapKey[target2] = (side1, Side.Dim - 1 - i, Side.Dim - 1, Direction.Up);
        }



        else if (side1Side == Direction.Left && side2Side == Direction.Up)
        {
            side1.P2WrapKey[target1] = (side2, i, 0, Direction.Down);
            side2.P2WrapKey[target2] = (side1, 0, i, Direction.Right);
        }
        else if (side1Side == Direction.Left && side2Side == Direction.Down)
        {
            side1.P2WrapKey[target1] = (side2, Side.Dim - 1 - i, Side.Dim - 1, Direction.Up);
            side2.P2WrapKey[target2] = (side1, Side.Dim - 1, Side.Dim - 1 - i, Direction.Right);
        }
        else if (side1Side == Direction.Left && side2Side == Direction.Right)
        {
            side1.P2WrapKey[target1] = (side2, Side.Dim - 1, i, Direction.Left);
            side2.P2WrapKey[target2] = (side1, 0, i, Direction.Right);
        }


        else if (side1Side == Direction.Right && side2Side == Direction.Up)
        {
            side1.P2WrapKey[target1] = (side2, Side.Dim - 1 - i, 0, Direction.Down);
            side2.P2WrapKey[target2] = (side1, Side.Dim - 1, Side.Dim - 1 - i, Direction.Left);
        }
        else if (side1Side == Direction.Right && side2Side == Direction.Down)
        {
            side1.P2WrapKey[target1] = (side2, i, Side.Dim - 1, Direction.Up);
            side2.P2WrapKey[target2] = (side1, Side.Dim - 1, i, Direction.Left);
        }


        else if (side1Side == Direction.Down && side2Side == Direction.Up)
        {
            side1.P2WrapKey[target1] = (side2, i, 0, Direction.Down);
            side2.P2WrapKey[target2] = (side1, i, Side.Dim - 1, Direction.Up);
        }
        else
            throw new Exception();
    }
}

void P1_2(int p)
{
    Side currentSide = Sides[(2, 0)];
    int x = 0;
    int y = 0;
    Direction direction = Direction.Right;

    bool distance_nTurn = true;

    int distanceI = 0;
    int turnI = 0;

    while (distanceI < Distances.Count || turnI < Turns.Count)
    {
        if (distance_nTurn)
        {
            int distance = Distances[distanceI];
            for (int i = 0; i < distance; i++)
            {
                int newX = x;
                int newY = y;
                Side newSide = currentSide;
                Direction newDirection = direction;
                switch (direction)
                {
                    case Direction.Up: newY--; break;
                    case Direction.Down: newY++; break;
                    case Direction.Left: newX--; break;
                    case Direction.Right: newX++; break;
                    default: throw new Exception();
                }

                if (newX == Side.Dim || newX == -1 || newY == Side.Dim || newY == -1)
                {
                    if (p == 1)
                        (newSide, newX, newY, newDirection) = currentSide.P1WrapKey[(newX, newY)];
                    else if (p == 2)
                        (newSide, newX, newY, newDirection) = currentSide.P2WrapKey[(newX, newY)];
                }

                if (newSide.SideMap[newY, newX] == Cell.Wall)
                {
                    break;
                }

                x = newX;
                y = newY;
                currentSide = newSide;
                direction = newDirection;
            }
            distanceI++;
        }
        else
        {
            direction = (Direction)((int)direction + Turns[turnI]);
            if ((int)direction == 4)
                direction = (Direction)0;
            else if ((int)direction == -1)
                direction = (Direction)3;
            turnI++;
        }
        distance_nTurn = !distance_nTurn;
    }

    (int mapX, int mapY) = currentSide.SideCoordinatesToMapCoordinates[y, x];
    int row = mapY + 1;
    int column = mapX + 1;
    int result = (1000 * row) + (4 * column) + (int)direction;
    Console.WriteLine(result);
    Console.ReadLine();
}

P1_2(1);
P1_2(2);

public enum Direction
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3,
}

public enum Cell
{
    Nothing = ' ',
    Wall = '█',
    Space = '░',
}

public class Side
{
    public static int Dim;

    public int X;
    public int Y;
    public Cell[,] SideMap = new Cell[Dim, Dim];
    public (int, int)[,] SideCoordinatesToMapCoordinates = new (int, int)[Dim, Dim];

    public Dictionary<(int, int), (Side, int, int, Direction)> P1WrapKey = new Dictionary<(int, int), (Side, int, int, Direction)>();
    public Dictionary<(int, int), (Side, int, int, Direction)> P2WrapKey = new Dictionary<(int, int), (Side, int, int, Direction)>();

    public Side(List<string> inputList, int coordX, int coordY)
    {
        X = coordX;
        Y = coordY;
        int minX = coordX * Dim;
        int minY = coordY * Dim;
        for (int y = 0; y < Dim; y++)
        {
            for (int x = 0; x < Dim; x++)
            {
                switch (inputList[minY + y][minX + x])
                {
                    case '.': SideMap[y, x] = Cell.Space; break;
                    case '#': SideMap[y, x] = Cell.Wall; break;
                    default:
                        SideMap[y, x] = Cell.Nothing; break;
                }
                SideCoordinatesToMapCoordinates[y, x] = (minX + x, minY + y);
            }
        }

        P1WrapKey = new Dictionary<(int, int), (Side, int, int, Direction)>();
    }

    public void Print()
    {
        Print(-1, -1, Direction.Up);
    }

    public void Print(int cx, int cy, Direction cDirection)
    {
        for (int y = 0; y < Dim; y++)
        {
            for (int x = 0; x < Dim; x++)
            {
                if (cx == x && cy == y)
                {
                    switch (cDirection)
                    {
                        case Direction.Left: Console.Write('◄'); break;
                        case Direction.Right: Console.Write('►'); break;
                        case Direction.Up: Console.Write('▲'); break;
                        case Direction.Down: Console.Write('▼'); break;
                        default: throw new Exception();
                    }
                }
                else
                    Console.Write((char)SideMap[y, x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}