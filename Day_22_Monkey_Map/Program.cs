// 11:30

//#define EXAMPLE

using AdventOfCodeUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

int emptyLineIndex = inputList.IndexOf("");

string sInstructions = inputList[emptyLineIndex + 1];
List<int> Distances = new List<int>();
List<int> Turns = new List<int>();
MatchCollection mc = AoC.RegexMatch(sInstructions, @"(\d+|R|L)");
foreach (Match m in mc)
{
    string st = m.Groups[1].Value;
    if (st == "R")
        Turns.Add(1);
    else if (st == "L")
        Turns.Add(-1);
    else
        Distances.Add(int.Parse(st));
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

for (int yCoord = 0; yCoord < emptyLineIndex; yCoord++)
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

// Preprocess the simple part 1 wrapping
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

void foldCube(Side s, char? previousInvariantAxis = null, (int, int, int)? bLCoord = null, (int, int, int)? bRCoord = null, (int, int, int)? tLCoord = null, (int, int, int)? tRCoord = null)
{
    s.BLCoord = bLCoord;
    s.BRCoord = bRCoord;
    s.TLCoord = tLCoord;
    s.TRCoord = tRCoord;

    if (s.InvariantAxis is null)
    {
        Debug.Assert(previousInvariantAxis is not null);

        // At this point we must know 2 of the corner coordinates, which defines a unit line, and a unit line connects 2 faces
        // it will be the face that ISN'T the previous invariant face

        char? varyingAxis = null;
        int knownCombination;
        if (tLCoord is not null && tRCoord is not null)
        {
            knownCombination = 0;
            if (tLCoord.Value.Item1 != tRCoord.Value.Item1)
                varyingAxis = 'x';
            else if (tLCoord.Value.Item2 != tRCoord.Value.Item2)
                varyingAxis = 'y';
            else if (tLCoord.Value.Item3 != tRCoord.Value.Item3)
                varyingAxis = 'z';
        }
        else if (tLCoord is not null && bLCoord is not null)
        {
            knownCombination = 1;
            if (tLCoord.Value.Item1 != bLCoord.Value.Item1)
                varyingAxis = 'x';
            else if (tLCoord.Value.Item2 != bLCoord.Value.Item2)
                varyingAxis = 'y';
            else if (tLCoord.Value.Item3 != bLCoord.Value.Item3)
                varyingAxis = 'z';
        }
        else if (bLCoord is not null && bRCoord is not null)
        {
            knownCombination = 2;
            if (bLCoord.Value.Item1 != bRCoord.Value.Item1)
                varyingAxis = 'x';
            else if (bLCoord.Value.Item2 != bRCoord.Value.Item2)
                varyingAxis = 'y';
            else if (bLCoord.Value.Item3 != bRCoord.Value.Item3)
                varyingAxis = 'z';
        }
        else if (bRCoord is not null && tRCoord is not null)
        {
            knownCombination = 3;
            if (bRCoord.Value.Item1 != tRCoord.Value.Item1)
                varyingAxis = 'x';
            else if (bRCoord.Value.Item2 != tRCoord.Value.Item2)
                varyingAxis = 'y';
            else if (bRCoord.Value.Item3 != tRCoord.Value.Item3)
                varyingAxis = 'z';
        }
        else throw new Exception();

        Debug.Assert(varyingAxis is not null);

        char invariantAxis = (new char[] { 'x', 'y', 'z' }).Except(new char[] { (char)varyingAxis, (char)previousInvariantAxis }).First();
        int invariantAxisVal;
        (int, int, int)? tmp = (new List<(int, int, int)?> { bLCoord, bRCoord, tLCoord, tRCoord }).Find(c => c is not null);
        Debug.Assert(tmp is not null);

        switch (invariantAxis)
        {
            case 'x': invariantAxisVal = tmp.Value.Item1; break;
            case 'y': invariantAxisVal = tmp.Value.Item2; break;
            case 'z': invariantAxisVal = tmp.Value.Item3; break;
            default: throw new Exception();
        }

        char toggleAxis = (new char[] { 'x', 'y', 'z' }).Except(new char[] { (char)varyingAxis, (char)invariantAxis }).First();
        int toggleAxisVal;

        switch (toggleAxis)
        {
            case 'x': toggleAxisVal = tmp.Value.Item1; break;
            case 'y': toggleAxisVal = tmp.Value.Item2; break;
            case 'z': toggleAxisVal = tmp.Value.Item3; break;
            default: throw new Exception();
        }

        int nToggleAxisVal = toggleAxisVal == 0 ? 1 : 0;

        int val1x = -1;
        int val1y = -1;
        int val1z = -1;
        int val2x = -1;
        int val2y = -1;
        int val2z = -1;

        Debug.Assert(varyingAxis != toggleAxis && toggleAxis != invariantAxis && varyingAxis != invariantAxis);

        switch (invariantAxis)
        {
            case 'x': val1x = invariantAxisVal; val2x = invariantAxisVal; break;
            case 'y': val1y = invariantAxisVal; val2y = invariantAxisVal; break;
            case 'z': val1z = invariantAxisVal; val2z = invariantAxisVal; break;
            default: throw new Exception();
        }
        switch (toggleAxis)
        {
            case 'x': val1x = nToggleAxisVal; val2x = nToggleAxisVal; break;
            case 'y': val1y = nToggleAxisVal; val2y = nToggleAxisVal; break;
            case 'z': val1z = nToggleAxisVal; val2z = nToggleAxisVal; break;
            default: throw new Exception();
        }
        switch (varyingAxis)
        {
            case 'x':
                switch (knownCombination)
                {
                    case 0:
                        Debug.Assert(s.TLCoord is not null && s.TRCoord is not null);
                        val1x = s.TLCoord.Value.Item1;
                        val2x = s.TRCoord.Value.Item1;
                        break;
                    case 1:
                        Debug.Assert(s.TLCoord is not null && s.BLCoord is not null);
                        val1x = s.TLCoord.Value.Item1;
                        val2x = s.BLCoord.Value.Item1;
                        break;
                    case 2:
                        Debug.Assert(s.BLCoord is not null && s.BRCoord is not null);
                        val1x = s.BLCoord.Value.Item1;
                        val2x = s.BRCoord.Value.Item1;
                        break;
                    case 3:
                        Debug.Assert(s.BRCoord is not null && s.TRCoord is not null);
                        val1x = s.BRCoord.Value.Item1;
                        val2x = s.TRCoord.Value.Item1;
                        break;
                    default: throw new Exception();
                }
                break;
            case 'y':
                switch (knownCombination)
                {
                    case 0:
                        Debug.Assert(s.TLCoord is not null && s.TRCoord is not null);
                        val1y = s.TLCoord.Value.Item2;
                        val2y = s.TRCoord.Value.Item2;
                        break;
                    case 1:
                        Debug.Assert(s.TLCoord is not null && s.BLCoord is not null);
                        val1y = s.TLCoord.Value.Item2;
                        val2y = s.BLCoord.Value.Item2;
                        break;
                    case 2:
                        Debug.Assert(s.BLCoord is not null && s.BRCoord is not null);
                        val1y = s.BLCoord.Value.Item2;
                        val2y = s.BRCoord.Value.Item2;
                        break;
                    case 3:
                        Debug.Assert(s.BRCoord is not null && s.TRCoord is not null);
                        val1y = s.BRCoord.Value.Item2;
                        val2y = s.TRCoord.Value.Item2;
                        break;
                    default: throw new Exception();
                }
                break;
            case 'z':
                switch (knownCombination)
                {
                    case 0:
                        Debug.Assert(s.TLCoord is not null && s.TRCoord is not null);
                        val1z = s.TLCoord.Value.Item3;
                        val2z = s.TRCoord.Value.Item3;
                        break;
                    case 1:
                        Debug.Assert(s.TLCoord is not null && s.BLCoord is not null);
                        val1z = s.TLCoord.Value.Item3;
                        val2z = s.BLCoord.Value.Item3;
                        break;
                    case 2:
                        Debug.Assert(s.BLCoord is not null && s.BRCoord is not null);
                        val1z = s.BLCoord.Value.Item3;
                        val2z = s.BRCoord.Value.Item3;
                        break;
                    case 3:
                        Debug.Assert(s.BRCoord is not null && s.TRCoord is not null);
                        val1z = s.BRCoord.Value.Item3;
                        val2z = s.TRCoord.Value.Item3;
                        break;
                    default: throw new Exception();
                }
                break;
            default: throw new Exception();
        }

        switch (knownCombination)
        {
            case 0:
                s.BLCoord = (val1x, val1y, val1z);
                s.BRCoord = (val2x, val2y, val2z);
                break;
            case 1:
                s.TRCoord = (val1x, val1y, val1z);
                s.BRCoord = (val2x, val2y, val2z);
                break;
            case 2:
                s.TLCoord = (val1x, val1y, val1z);
                s.TRCoord = (val2x, val2y, val2z);
                break;
            case 3:
                s.BLCoord = (val1x, val1y, val1z);
                s.TLCoord = (val2x, val2y, val2z);
                break;
            default: throw new Exception();
        }

        s.CornersDefined = true;
    }

    (int, int) leftSideCoord = (s.X - 1, s.Y);
    (int, int) rightSideCoord = (s.X + 1, s.Y);
    (int, int) upSideCoord = (s.X, s.Y - 1);
    (int, int) downSideCoord = (s.X, s.Y + 1);

    if (Sides.ContainsKey(leftSideCoord) && !Sides[leftSideCoord].CornersDefined)
        foldCube(Sides[leftSideCoord], previousInvariantAxis: s.InvariantAxis, tRCoord: s.TLCoord, bRCoord: s.BLCoord);
    if (Sides.ContainsKey(rightSideCoord) && !Sides[rightSideCoord].CornersDefined)
        foldCube(Sides[rightSideCoord], previousInvariantAxis: s.InvariantAxis, tLCoord: s.TRCoord, bLCoord: s.BRCoord);
    if (Sides.ContainsKey(upSideCoord) && !Sides[upSideCoord].CornersDefined)
        foldCube(Sides[upSideCoord], previousInvariantAxis: s.InvariantAxis, bLCoord: s.TLCoord, bRCoord: s.TRCoord);
    if (Sides.ContainsKey(downSideCoord) && !Sides[downSideCoord].CornersDefined)
        foldCube(Sides[downSideCoord], previousInvariantAxis: s.InvariantAxis, tLCoord: s.BLCoord, tRCoord: s.BRCoord);
}

Side s = Sides.Values.ToArray().First();
foldCube(s, tLCoord: (0, 1, 0), tRCoord: (1, 1, 0), bLCoord: (0, 0, 0), bRCoord: (1, 0, 0));

Direction? checkS2((int, int, int) s1_1, (int, int, int) s1_2, Side s2)
{
    if (
        (s1_1 == s2.TLCoord && s1_2 == s2.TRCoord) ||
        (s1_1 == s2.TRCoord && s1_2 == s2.TLCoord))
    {
        return Direction.Up;
    }
    else if (
        (s1_1 == s2.BLCoord && s1_2 == s2.TLCoord) ||
        (s1_1 == s2.TLCoord && s1_2 == s2.BLCoord))
    {
        return Direction.Left;
    }
    else if (
        (s1_1 == s2.BLCoord && s1_2 == s2.BRCoord) ||
        (s1_1 == s2.BRCoord && s1_2 == s2.BLCoord))
    {
        return Direction.Down;
    }
    else if (
        (s1_1 == s2.BRCoord && s1_2 == s2.TRCoord) ||
        (s1_1 == s2.TRCoord && s1_2 == s2.BRCoord))
    {
        return Direction.Right;
    }
    return null;
}

List<(Side, Direction, Side, Direction)> relations = new List<(Side, Direction, Side, Direction)>();

foreach (Side s1 in Sides.Values)
{
    foreach (Side s2 in Sides.Values)
    {
        if (s1 != s2)
        {
            // If we already have a relation between 2 sides don't both looking for another
            if (relations.FindIndex(t => (s1 == t.Item1 && s2 == t.Item3) || (s2 == t.Item1 && s1 == t.Item3)) == -1)
            {
                Direction? d;
                Debug.Assert(s1.BLCoord is not null && s1.BRCoord is not null && s1.TLCoord is not null && s1.TRCoord is not null);
                d = checkS2(s1.BLCoord.Value, s1.BRCoord.Value, s2);
                if (d is not null)
                    relations.Add((s1, Direction.Down, s2, (Direction)d));
                d = checkS2(s1.TRCoord.Value, s1.BRCoord.Value, s2);
                if (d is not null)
                    relations.Add((s1, Direction.Right, s2, (Direction)d));
                d = checkS2(s1.TLCoord.Value, s1.TRCoord.Value, s2);
                if (d is not null)
                    relations.Add((s1, Direction.Up, s2, (Direction)d));
                d = checkS2(s1.TLCoord.Value, s1.BLCoord.Value, s2);
                if (d is not null)
                    relations.Add((s1, Direction.Left, s2, (Direction)d));
            }
        }
    }
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
    Side currentSide = Sides.Values.First();
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

//P1_2(1);
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
    public (int, int, int)? TLCoord;
    public (int, int, int)? TRCoord;
    public (int, int, int)? BLCoord;
    public (int, int, int)? BRCoord;
    public bool CornersDefined = false;

    public char? InvariantAxis
    {
        get
        {
            if (TLCoord is null || TRCoord is null || BLCoord == null || BRCoord is null)
                return null;
            if (BLCoord.Value.Item1 == BRCoord.Value.Item1 && BRCoord.Value.Item1 == TLCoord.Value.Item1 && TLCoord.Value.Item1 == TRCoord.Value.Item1)
                return 'x';
            else if (BLCoord.Value.Item2 == BRCoord.Value.Item2 && BRCoord.Value.Item2 == TLCoord.Value.Item2 && TLCoord.Value.Item2 == TRCoord.Value.Item2)
                return 'y';
            else if (BLCoord.Value.Item3 == BRCoord.Value.Item3 && BRCoord.Value.Item3 == TLCoord.Value.Item3 && TLCoord.Value.Item3 == TRCoord.Value.Item3)
                return 'z';
            else
                throw new Exception();
        }
    }

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
