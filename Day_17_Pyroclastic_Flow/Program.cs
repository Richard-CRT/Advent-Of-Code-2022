// 10:40

using AdventOfCodeUtilities;
using System;
using System.Text.RegularExpressions;

List<string> inputList = AoCUtilities.GetInputLines();
List<Direction> Jets = inputList[0].Select(x => (Direction)x).ToList();
int NextJetNum = 0;

Dictionary<(int, int), Cell> Map = new Dictionary<(int, int), Cell>();
int Height = 0;

int NextPieceNum = 0;
List<Dictionary<(int, int), Cell>> Pieces = new List<Dictionary<(int, int), Cell>>()
{
    new Dictionary<(int, int), Cell> { { (0,0), Cell.Rock }, { (1,0), Cell.Rock }, { (2,0), Cell.Rock }, { (3,0), Cell.Rock }},
    new Dictionary<(int, int), Cell> { { (1,0), Cell.Rock }, { (0,1), Cell.Rock }, { (1,1), Cell.Rock }, { (2,1), Cell.Rock }, { (1,2), Cell.Rock }},
    new Dictionary<(int, int), Cell> { { (0,0), Cell.Rock }, { (1,0), Cell.Rock }, { (2,0), Cell.Rock }, { (2,1), Cell.Rock }, { (2,2), Cell.Rock }},
    new Dictionary<(int, int), Cell> { { (0,0), Cell.Rock }, { (0,1), Cell.Rock }, { (0,2), Cell.Rock }, { (0,3), Cell.Rock }},
    new Dictionary<(int, int), Cell> { { (0,0), Cell.Rock }, { (1,0), Cell.Rock }, { (0,1), Cell.Rock }, { (1,1), Cell.Rock }},
};

void P1_2()
{
    Dictionary<(int, int, string), (int, int)> History = new Dictionary<(int, int, string), (int, int)>();
    Int64 smartHeight = -1;
    int i = 0;
    while(smartHeight == -1)
    {
        Dictionary<(int, int), Cell> fallingPiece = Pieces[NextPieceNum];
        int fallingPieceLowerEdgeY = Height + 3;
        int fallingPieceLeftEdgeX = 2;
        int fallingPieceWidth = fallingPiece.Keys.MaxBy(x => x.Item1).Item1 + 1;

        if (smartHeight == -1)
        {
            string topRowKey = "";
            for (int x = 0; x < 7; x++)
            {
                topRowKey += (char)Map.GetValueOrDefault((x, Height - 1), Cell.Air);
            }
            //Console.WriteLine(topRowKey);
            (int, int, string) historyKey = (NextPieceNum, NextJetNum, topRowKey);
            //Console.WriteLine();

            if (History.ContainsKey(historyKey))
            {
                (int, int) repeatsAt = (i, Height);
                (int, int) repeatedCycle = History[historyKey];

                //Console.WriteLine($"Repeats at i{repeatsAt.Item1}h{repeatsAt.Item2} previously seen at cycle i{repeatedCycle.Item1}h{repeatedCycle.Item2}");
                int initialCycles = repeatedCycle.Item1;
                int initialOffsetHeight = repeatedCycle.Item2;
                int cycleLength = repeatsAt.Item1 - repeatedCycle.Item1;
                // This part not technically necessary, as it means we keep going further than absolutely necessary
                // but it means we get a nice multiple that doesn't require us to do more processing afterwards
                // so faster to write, and still quick to execute
                if ((1000000000000 - initialCycles) % cycleLength == 0)
                {
                    int initialHeight = repeatedCycle.Item2;
                    int cycleHeight = repeatsAt.Item2 - repeatedCycle.Item2;
                    Int64 fullCycles = (1000000000000 - initialCycles) / cycleLength;
                    smartHeight = initialHeight + (fullCycles * cycleHeight);
                    if (i > 2022)
                        break;
                }
            }
            else
            {
                History.Add(historyKey, (i, Height));
            }
        }

        //Print(fallingPiece, fallingPieceLeftEdgeX, fallingPieceLowerEdgeY);

        NextPieceNum = (NextPieceNum + 1) % Pieces.Count;
        while (true)
        {
            int trialFallingPieceLeftEdgeX = fallingPieceLeftEdgeX;
            switch (Jets[NextJetNum])
            {
                case Direction.Left:
                    trialFallingPieceLeftEdgeX--; break;
                case Direction.Right:
                    trialFallingPieceLeftEdgeX++; break;
            }
            if (trialFallingPieceLeftEdgeX >= 0 && trialFallingPieceLeftEdgeX + fallingPieceWidth <= 7)
            {
                bool xcollide = false;
                foreach ((int fallingPieceX, int fallingPieceY) in fallingPiece.Keys)
                {
                    int trialX = trialFallingPieceLeftEdgeX + fallingPieceX;
                    int trialY = fallingPieceLowerEdgeY + fallingPieceY;
                    if (trialY < 0 || Map.GetValueOrDefault((trialX, trialY), Cell.Air) == Cell.Rock)
                    {
                        xcollide = true;
                        break;
                    }
                }
                if (!xcollide)
                    fallingPieceLeftEdgeX = trialFallingPieceLeftEdgeX;
            }
            NextJetNum = (NextJetNum + 1) % Jets.Count;
            int trialFallingPieceLowerEdgeY = fallingPieceLowerEdgeY - 1;
            bool collide = false;
            foreach ((int fallingPieceX, int fallingPieceY) in fallingPiece.Keys)
            {
                int trialX = fallingPieceLeftEdgeX + fallingPieceX;
                int trialY = trialFallingPieceLowerEdgeY + fallingPieceY;
                if (trialY < 0 || Map.GetValueOrDefault((trialX, trialY), Cell.Air) == Cell.Rock)
                {
                    collide = true;
                    break;
                }
            }
            if (!collide)
                fallingPieceLowerEdgeY = trialFallingPieceLowerEdgeY;
            else
            {
                foreach (var kvp in fallingPiece)
                {
                    (int fallingPieceX, int fallingPieceY) = kvp.Key;
                    int realX = fallingPieceLeftEdgeX + fallingPieceX;
                    int realY = fallingPieceLowerEdgeY + fallingPieceY;
                    if (realY + 1 > Height)
                        Height = realY + 1;
                    Map[(realX, realY)] = kvp.Value;
                }
                break;
            }
            //Print(fallingPiece, fallingPieceLeftEdgeX, fallingPieceLowerEdgeY);
        }

        i++;

        if (i == 2022)
        {
            Console.WriteLine(Height);
            Console.ReadLine();
            if (smartHeight != -1)
                break;
        }
    }
    if (smartHeight != -1)
        Console.WriteLine(smartHeight);
    Console.ReadLine();
}

void Print(Dictionary<(int, int), Cell> fallingPiece, int fallingPieceLeftEdgeX, int fallingPieceLowerEdgeY)
{
    for (int y = Height + 3 + 5; y >= 0; y--)
    {
        for (int x = 0; x < 7; x++)
        {
            bool found = false;
            foreach ((int fallingPieceX, int fallingPieceY) in fallingPiece.Keys)
            {
                if (fallingPieceLeftEdgeX + fallingPieceX == x && fallingPieceLowerEdgeY + fallingPieceY == y)
                {
                    Console.Write("▒");
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Console.Write((char)Map.GetValueOrDefault((x, y), Cell.Air));
            }
        }
        Console.WriteLine();
    }
    Console.ReadLine();
}

P1_2();

public enum Cell
{
    Air = '.',
    Rock = '█',
}

public enum Direction
{
    Left = '<',
    Right = '>'
}