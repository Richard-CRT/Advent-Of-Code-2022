// 11:30

//#define EXAMPLE

using AdventOfCodeUtilities;
using System;
using System.Collections.Generic;
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

// Determine the side dimension
Side.Dim = int.MaxValue;
for (int i = 0; i < emptyLineIndex; i++)
{
    int trialLength = inputList[i].Trim().Length;
    if (trialLength < Side.Dim)
        Side.Dim = trialLength;
}

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

List<(Side, Direction, Side, Direction)> relations = new List<(Side, Direction, Side, Direction)>();
// Preprocess the easy part 2 wrappings where the adjacent sides are already there
foreach (var kvp in Sides)
{
    (int xCoord, int yCoord) = kvp.Key;
    Side side = kvp.Value;

    if (Sides.ContainsKey((xCoord - 1, yCoord)))
        relations.Add((side, Direction.Left, Sides[(xCoord - 1, yCoord)], Direction.Right));

    if (Sides.ContainsKey((xCoord + 1, yCoord)))
        relations.Add((side, Direction.Right, Sides[(xCoord + 1, yCoord)], Direction.Left));

    if (Sides.ContainsKey((xCoord, yCoord - 1)))
        relations.Add((side, Direction.Up, Sides[(xCoord, yCoord - 1)], Direction.Down));

    if (Sides.ContainsKey((xCoord, yCoord + 1)))
        relations.Add((side, Direction.Down, Sides[(xCoord, yCoord + 1)], Direction.Up));
}

// Technically speaking the input cube net is the same for everyone
// so not necessary to be completely agnostic to cube net
List<(int[,], List<(int, int, Direction, int, int, Direction)>)> validCubeNets = new List<(int[,], List<(int, int, Direction, int, int, Direction)>)>();
validCubeNets.AddRange(new List<(int[,], List<(int, int, Direction, int, int, Direction)>)>
{
    (new int[,]
    {
        { 1, 1, 1 },
        { 0, 1, 0 },
        { 0, 1, 0 },
        { 0, 1, 0 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (1, 0, Direction.Up, 1, 3, Direction.Down),
        (0, 0, Direction.Left, 1, 2, Direction.Left),
        (2, 0, Direction.Right, 1, 2, Direction.Right),
        (0, 0, Direction.Up, 1, 3, Direction.Left),
        (2, 0, Direction.Up, 1, 3, Direction.Right),
        (0, 0, Direction.Down, 1, 1, Direction.Left),
        (2, 0, Direction.Down, 1, 1, Direction.Right),
    }),
    (new int[,]
    {
        { 1, 1, 0 },
        { 0, 1, 1 },
        { 0, 1, 0 },
        { 0, 1, 0 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (1, 0, Direction.Up, 1, 3, Direction.Down),
        (0, 0, Direction.Left, 1, 2, Direction.Left),
        (0, 0, Direction.Up, 1, 3, Direction.Left),
        (0, 0, Direction.Down, 1, 1, Direction.Left),
        (2, 1, Direction.Up, 1, 0, Direction.Right),
        (2, 1, Direction.Right, 1, 3, Direction.Right),
        (2, 1, Direction.Down, 1, 2, Direction.Right),
    }),
    (new int[,]
    {
        { 1, 1, 0 },
        { 0, 1, 0 },
        { 0, 1, 1 },
        { 0, 1, 0 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (1, 0, Direction.Up, 1, 3, Direction.Down),
        (0, 0, Direction.Left, 1, 2, Direction.Left),
        (0, 0, Direction.Up, 1, 3, Direction.Left),
        (0, 0, Direction.Down, 1, 1, Direction.Left),
        (2, 2, Direction.Up, 1, 1, Direction.Right),
        (2, 2, Direction.Right, 1, 0, Direction.Right),
        (2, 2, Direction.Down, 1, 3, Direction.Right),
    }),
    (new int[,]
    {
        { 1, 1, 0 },
        { 0, 1, 0 },
        { 0, 1, 0 },
        { 0, 1, 1 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (1, 0, Direction.Up, 1, 3, Direction.Down),
        (0, 0, Direction.Left, 1, 2, Direction.Left),
        (0, 0, Direction.Up, 1, 3, Direction.Left),
        (0, 0, Direction.Down, 1, 1, Direction.Left),
        (2, 3, Direction.Up, 1, 2, Direction.Right),
        (2, 3, Direction.Right, 1, 1, Direction.Right),
        (2, 3, Direction.Down, 1, 0, Direction.Right),
    }),
    (new int[,]
    {
        { 1, 0, 0 },
        { 1, 1, 1 },
        { 0, 1, 0 },
        { 0, 1, 0 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (0, 0, Direction.Left, 1, 3, Direction.Down),
        (0, 0, Direction.Right, 1, 1, Direction.Up),
        (0, 0, Direction.Up, 2, 1, Direction.Up),
        (0, 1, Direction.Left, 1, 3, Direction.Left),
        (0, 1, Direction.Down, 1, 2, Direction.Left),
        (2, 1, Direction.Right, 1, 3, Direction.Right),
        (2, 1, Direction.Down, 1, 2, Direction.Right),
    }),
    (new int[,]
    {
        { 1, 0, 0 },
        { 1, 1, 0 },
        { 0, 1, 1 },
        { 0, 1, 0 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (0, 0, Direction.Right, 1, 1, Direction.Up),
        (0, 0, Direction.Up, 2, 2, Direction.Right),
        (0, 0, Direction.Left, 1, 3, Direction.Down),
        (1, 1, Direction.Right, 2, 2, Direction.Up),
        (2, 2, Direction.Down, 1, 3, Direction.Right),
        (1, 3, Direction.Left, 0, 1, Direction.Left),
        (0, 1, Direction.Down, 1, 2, Direction.Left),
    }),
    (new int[,]
    {
        { 1, 0, 0 },
        { 1, 1, 0 },
        { 0, 1, 0 },
        { 0, 1, 1 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (0, 0, Direction.Right, 1, 1, Direction.Up),
        (0, 0, Direction.Up, 2, 3, Direction.Down),
        (0, 0, Direction.Left, 1, 3, Direction.Down),
        (0, 1, Direction.Left, 1, 3, Direction.Left),
        (0, 1, Direction.Down, 1, 2, Direction.Left),
        (1, 1, Direction.Right, 2, 3, Direction.Right),
        (1, 2, Direction.Right, 2, 3, Direction.Up),
    }),
    (new int[,]
    {
        { 1, 0, 0 },
        { 1, 1, 0 },
        { 0, 1, 1 },
        { 0, 0, 1 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (0, 0, Direction.Left, 2, 3, Direction.Right),
        (0, 0, Direction.Up, 2, 2, Direction.Right),
        (0, 0, Direction.Right, 1, 1, Direction.Up),
        (1, 1, Direction.Right, 2, 2, Direction.Up),
        (2, 3, Direction.Down, 0, 1, Direction.Left),
        (2, 3, Direction.Left, 1, 2, Direction.Down),
        (1, 2, Direction.Left, 0, 1, Direction.Down),
    }),
    (new int[,]
    {
        { 0, 1, 0 },
        { 1, 1, 1 },
        { 0, 1, 0 },
        { 0, 1, 0 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (1, 0, Direction.Left, 0, 1, Direction.Up),
        (1, 0, Direction.Up, 1, 3, Direction.Down),
        (1, 0, Direction.Right, 2, 1, Direction.Up),
        (0, 1, Direction.Left, 1, 3, Direction.Left),
        (2, 1, Direction.Right, 1, 3, Direction.Right),
        (0, 1, Direction.Down, 1, 2, Direction.Left),
        (2, 1, Direction.Down, 1, 2, Direction.Right),
    }),
    (new int[,]
    {
        { 0, 1, 0 },
        { 1, 1, 0 },
        { 0, 1, 1 },
        { 0, 1, 0 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (1, 0, Direction.Left, 0, 1, Direction.Up),
        (1, 0, Direction.Up, 1, 3, Direction.Down),
        (1, 0, Direction.Right, 2, 2, Direction.Right),
        (0, 1, Direction.Left, 1, 3, Direction.Left),
        (0, 1, Direction.Down, 1, 2, Direction.Left),
        (2, 2, Direction.Down, 1, 3, Direction.Right),
        (2, 2, Direction.Up, 1, 1, Direction.Left),
    }),
    (new int[,]
    {
        { 1, 0 },
        { 1, 0 },
        { 1, 1 },
        { 0, 1 },
        { 0, 1 },
    }, new List<(int, int, Direction, int, int, Direction)> {
        (0, 0, Direction.Right, 1, 2, Direction.Right),
        (0, 0, Direction.Up, 1, 3, Direction.Right),
        (0, 0, Direction.Left, 1, 4, Direction.Right),
        (0, 1, Direction.Right, 1, 2, Direction.Up),
        (0, 1, Direction.Left, 1, 4, Direction.Down),
        (0, 2, Direction.Left, 1, 4, Direction.Left),
        (0, 2, Direction.Down, 1, 3, Direction.Left),
    }),
});

// Compare with valid cubes to get the rest of the relations from the valid cube nets list
(int[,], List<(int, int, Direction, int, int, Direction)>)? match = null;
for (int j = 0; j < validCubeNets.Count; j++)
{
    // For each of the valid cubes, produce a flipped version
    // Then take all 4 rotations of both the original and the flipped
    // Compare the input with all 8 possibilities
    (int[,] originalCubeNet, List<(int, int, Direction, int, int, Direction)> originalRelations) = validCubeNets[j];

    List<(int[,], List<(int, int, Direction, int, int, Direction)>)> versions = new();

    int[,] flippedOriginalCubeNet = new int[originalCubeNet.GetLength(0), originalCubeNet.GetLength(1)];
    for (int y = 0; y < flippedOriginalCubeNet.GetLength(0); y++)
    {
        for (int x = 0; x < flippedOriginalCubeNet.GetLength(1); x++)
        {
            flippedOriginalCubeNet[y, x] = originalCubeNet[y, flippedOriginalCubeNet.GetLength(1) - 1 - x];
        }
    }
    List<(int, int, Direction, int, int, Direction)> flippedOriginalRelations = new();
    foreach ((int s1x, int s1y, Direction s1d, int s2x, int s2y, Direction s2d) in originalRelations)
    {
        Direction newS1d = s1d;
        if (newS1d == Direction.Left)
            newS1d = Direction.Right;
        else if (newS1d == Direction.Right)
            newS1d = Direction.Left;

        Direction newS2d = s2d;
        if (newS2d == Direction.Left)
            newS2d = Direction.Right;
        else if (newS2d == Direction.Right)
            newS2d = Direction.Left;

        flippedOriginalRelations.Add((flippedOriginalCubeNet.GetLength(1) - 1 - s1x, s1y, newS1d, flippedOriginalCubeNet.GetLength(1) - 1 - s2x, s2y, newS2d));
    }

    versions.Add((originalCubeNet, originalRelations));
    versions.Add((flippedOriginalCubeNet, flippedOriginalRelations));

    int[,] rotOriginal = originalCubeNet;
    int[,] rotFlipped = flippedOriginalCubeNet;
    List<(int, int, Direction, int, int, Direction)> rotOriginalRelations = originalRelations;
    List<(int, int, Direction, int, int, Direction)> rotFlippedRelations = flippedOriginalRelations;
    for (int i = 0; i < 3; i++)
    {
        int[,] newRotOriginal = new int[rotOriginal.GetLength(1), rotOriginal.GetLength(0)];
        int[,] newRotFlipped = new int[rotFlipped.GetLength(1), rotFlipped.GetLength(0)];
        for (int y = 0; y < rotOriginal.GetLength(0); y++)
        {
            for (int x = 0; x < rotFlipped.GetLength(1); x++)
            {
                newRotOriginal[x, rotOriginal.GetLength(0) - y - 1] = rotOriginal[y, x];
                newRotFlipped[x, rotFlipped.GetLength(0) - y - 1] = rotFlipped[y, x];
            }
        }

        List<(int, int, Direction, int, int, Direction)> newRotOriginalRelations = new List<(int, int, Direction, int, int, Direction)>();
        foreach ((int s1x, int s1y, Direction s1d, int s2x, int s2y, Direction s2d) in rotOriginalRelations)
        {
            Direction newS1d = (Direction)((int)s1d + 1);
            if ((int)newS1d == 4) newS1d = (Direction)0;
            else if ((int)newS1d == -1) newS1d = (Direction)3;

            Direction newS2d = (Direction)((int)s2d + 1);
            if ((int)newS2d == 4) newS2d = (Direction)0;
            else if ((int)newS2d == -1) newS2d = (Direction)3;

            newRotOriginalRelations.Add((rotOriginal.GetLength(0) - s1y - 1, s1x, newS1d, rotOriginal.GetLength(0) - s2y - 1, s2x, newS2d));
        }
        List<(int, int, Direction, int, int, Direction)> newRotFlippedRelations = new List<(int, int, Direction, int, int, Direction)>();
        foreach ((int s1x, int s1y, Direction s1d, int s2x, int s2y, Direction s2d) in rotFlippedRelations)
        {
            Direction newS1d = (Direction)((int)s1d + 1);
            if ((int)newS1d == 4) newS1d = (Direction)0;
            else if ((int)newS1d == -1) newS1d = (Direction)3;

            Direction newS2d = (Direction)((int)s2d + 1);
            if ((int)newS2d == 4) newS2d = (Direction)0;
            else if ((int)newS2d == -1) newS2d = (Direction)3;

            newRotFlippedRelations.Add((rotFlipped.GetLength(0) - s1y - 1, s1x, newS1d, rotFlipped.GetLength(0) - s2y - 1, s2x, newS2d));
        }

        rotOriginal = newRotOriginal;
        rotFlipped = newRotFlipped;
        rotOriginalRelations = newRotOriginalRelations;
        rotFlippedRelations = newRotFlippedRelations;

        versions.Add((rotOriginal, rotOriginalRelations));
        versions.Add((rotFlipped, rotFlippedRelations));
    }

    for (int i = 0; i < versions.Count; i++)
    {
        (int[,] versionCubeNet, List<(int, int, Direction, int, int, Direction)> versionRelations) = versions[i];
        bool versionMatch = true;
        for (int y = 0; y < versionCubeNet.GetLength(0); y++)
        {
            for (int x = 0; x < versionCubeNet.GetLength(1); x++)
            {
                if (
                    (versionCubeNet[y, x] == 1 && !Sides.ContainsKey((x, y))) ||
                    (versionCubeNet[y, x] == 0 && Sides.ContainsKey((x, y)))
                    )
                {
                    versionMatch = false;
                    break;
                }
            }
            if (!versionMatch) break;
        }
        if (versionMatch)
        {
            match = versions[i];
            break;
        }
    }
    if (match != null)
        break;
}

if (match == null)
    throw new Exception("Invalid cube");

var tmp = (((int[,], List<(int, int, Direction, int, int, Direction)>))match).Item2;
foreach ((int s1x, int s1y, Direction s1d, int s2x, int s2y, Direction s2d) in tmp)
{
    relations.Add((Sides[(s1x, s1y)], s1d, Sides[(s2x, s2y)], s2d));
}

// Preprocess the complex part 2 wrapping relations
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


        else throw new Exception();
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
