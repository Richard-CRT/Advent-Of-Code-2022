// 11:10

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

HashSet<(int, int)> Elves = new();
for (int y = 0; y < inputList.Count; y++)
{
    for (int x = 0; x < inputList[y].Count(); x++)
    {
        if (inputList[y][x] == '#')
            Elves.Add((x, y));
    }
}

void P1_2()
{
    Direction nextDirection = Direction.North;
    bool movementOccurred = true;
    int roundIndex = 0;
    while (movementOccurred)
    {
        Dictionary<(int, int), int> proposedElves = new();
        Dictionary<(int, int), (int, int)> proposedMoveByElve = new();
        foreach (var t in Elves)
        {
            (int x, int y) = t;
            if (
                Elves.Contains((x - 1, y)) ||
                Elves.Contains((x - 1, y - 1)) ||
                Elves.Contains((x, y - 1)) ||
                Elves.Contains((x + 1, y - 1)) ||
                Elves.Contains((x + 1, y)) ||
                Elves.Contains((x + 1, y + 1)) ||
                Elves.Contains((x, y + 1)) ||
                Elves.Contains((x - 1, y + 1))
                )
            {
                Direction iterDirection = nextDirection;
                for (int i = 0; i < 4; i++)
                {
                    bool foundValidDirection = false;
                    switch (iterDirection)
                    {
                        case Direction.North:
                            if (!Elves.Contains((x - 1, y - 1)) && !Elves.Contains((x, y - 1)) && !Elves.Contains((x + 1, y - 1)))
                            {
                                var proposedT = (x, y - 1);
                                proposedElves[proposedT] = proposedElves.GetValueOrDefault(proposedT, 0) + 1;
                                proposedMoveByElve[t] = proposedT;
                                foundValidDirection = true;
                            }
                            break;
                        case Direction.East:
                            if (!Elves.Contains((x + 1, y - 1)) && !Elves.Contains((x + 1, y)) && !Elves.Contains((x + 1, y + 1)))
                            {
                                var proposedT = (x + 1, y);
                                proposedElves[proposedT] = proposedElves.GetValueOrDefault(proposedT, 0) + 1;
                                proposedMoveByElve[t] = proposedT;
                                foundValidDirection = true;
                            }
                            break;
                        case Direction.South:
                            if (!Elves.Contains((x - 1, y + 1)) && !Elves.Contains((x, y + 1)) && !Elves.Contains((x + 1, y + 1)))
                            {
                                var proposedT = (x, y + 1);
                                proposedElves[proposedT] = proposedElves.GetValueOrDefault(proposedT, 0) + 1;
                                proposedMoveByElve[t] = proposedT;
                                foundValidDirection = true;
                            }
                            break;
                        case Direction.West:
                            if (!Elves.Contains((x - 1, y - 1)) && !Elves.Contains((x - 1, y)) && !Elves.Contains((x - 1, y + 1)))
                            {
                                var proposedT = (x - 1, y);
                                proposedElves[proposedT] = proposedElves.GetValueOrDefault(proposedT, 0) + 1;
                                proposedMoveByElve[t] = proposedT;
                                foundValidDirection = true;
                            }
                            break;
                    }

                    if (foundValidDirection)
                        break;

                    iterDirection = (Direction)(((int)iterDirection + 1) % 4);
                }
                if (!proposedMoveByElve.ContainsKey(t))
                {
                    proposedElves[t] = proposedElves.GetValueOrDefault(t, 0) + 1;
                    proposedMoveByElve[t] = t;
                }
            }
            else
            {
                proposedElves[t] = proposedElves.GetValueOrDefault(t, 0) + 1;
                proposedMoveByElve[t] = t;
            }
        }

        HashSet<(int, int)> newElves = new();

        movementOccurred = false;
        foreach (var t in Elves)
        {
            var proposedT = proposedMoveByElve[t];
            int howManyElvesProposedThis = proposedElves[proposedT];
            if (howManyElvesProposedThis == 0)
                throw new Exception();
            if (howManyElvesProposedThis == 1)
            {
                newElves.Add(proposedT);
                if (t != proposedT)
                    movementOccurred = true;
            }
            else
            {
                // multiple elves proposed this so don't move
                newElves.Add(t);
            }
        }

        Elves = newElves;

        if (roundIndex == 10 - 1)
        {
            int minX = Elves.MinBy(t => t.Item1).Item1;
            int maxX = Elves.MaxBy(t => t.Item1).Item1;
            int minY = Elves.MinBy(t => t.Item2).Item2;
            int maxY = Elves.MaxBy(t => t.Item2).Item2;

            int emptyCount = 0;
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (!Elves.Contains((x, y)))
                        emptyCount++;
                }
            }

            //Print();
            Console.WriteLine(emptyCount);
            Console.ReadLine();
        }

        nextDirection = (Direction)(((int)nextDirection + 1) % 4);
        roundIndex++;
    }

    //Print();
    Console.WriteLine(roundIndex);
    Console.ReadLine();
}

void Print()
{
    int minX = Elves.MinBy(t => t.Item1).Item1;
    int maxX = Elves.MaxBy(t => t.Item1).Item1;
    int minY = Elves.MinBy(t => t.Item2).Item2;
    int maxY = Elves.MaxBy(t => t.Item2).Item2;
    for (int y = minY; y <= maxY; y++)
    {
        for (int x = minX; x <= maxX; x++)
        {
            if (Elves.Contains((x, y)))
                Console.Write("██");
            else
                Console.Write("░░");
        }
        Console.WriteLine();
    }
    //Console.ReadLine();
}

P1_2();

public enum Direction
{
    West = 2,
    South = 1,
    East = 3,
    North = 0,
}
