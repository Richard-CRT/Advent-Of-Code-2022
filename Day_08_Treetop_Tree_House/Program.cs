using AdventOfCodeUtilities;

List<string> inputList = AoC.GetInputLines();
int[,] Trees = new int[inputList.Count(), inputList[0].Length];
bool[,] TreesSeen = new bool[inputList.Count(), inputList[0].Length];
for (int y = 0; y < inputList.Count; y++)
{
    for (int x = 0; x < inputList[y].Length; x++)
    {
        Trees[y, x] = inputList[y][x] - '0';
        TreesSeen[y, x] = false;
    }
}
int Y = inputList.Count;
int X = inputList[0].Length;

void P1()
{
    int visibleCount = 0;
    for (int column = 0; column < X; column++)
    {
        int maxTreeHeight = -1;
        for (int y = 0; y < Y; y++)
        {
            if (Trees[y, column] > maxTreeHeight)
            {
                maxTreeHeight = Trees[y, column];
                if (!TreesSeen[y, column])
                {
                    visibleCount++;
                    TreesSeen[y, column] = true;
                }
            }
        }
    }
    for (int column = 0; column < X; column++)
    {
        int maxTreeHeight = -1;
        for (int y = Y - 1; y >= 0; y--)
        {
            if (Trees[y, column] > maxTreeHeight)
            {
                maxTreeHeight = Trees[y, column];
                if (!TreesSeen[y, column])
                {
                    visibleCount++;
                    TreesSeen[y, column] = true;
                }
            }
        }
    }
    for (int row = 0; row < Y; row++)
    {
        int maxTreeHeight = -1;
        for (int x = 0; x < X; x++)
        {
            if (Trees[row, x] > maxTreeHeight)
            {
                maxTreeHeight = Trees[row, x];
                if (!TreesSeen[row, x])
                {
                    visibleCount++;
                    TreesSeen[row, x] = true;
                }
            }
        }
    }
    for (int row = 0; row < Y; row++)
    {
        int maxTreeHeight = -1;
        for (int x = X - 1; x >= 0; x--)
        {
            if (Trees[row, x] > maxTreeHeight)
            {
                maxTreeHeight = Trees[row, x];
                if (!TreesSeen[row, x])
                {
                    visibleCount++;
                    TreesSeen[row, x] = true;
                }
            }
        }
    }

    Console.WriteLine(visibleCount);
    Console.ReadLine();
}

void P2()
{
    int maxScenicScore = -1;
    for (int ty = 1; ty < Y - 1; ty++)
    {
        for (int tx = 1; tx < X - 1; tx++)
        {
            int negXDistance = 0;
            for (int x = tx - 1; x >= 0; x--)
            {
                negXDistance++;
                if (Trees[ty, x] >= Trees[ty, tx])
                    break;
            }
            int posXDistance = 0;
            for (int x = tx + 1; x < X; x++)
            {
                posXDistance++;
                if (Trees[ty, x] >= Trees[ty, tx])
                    break;
            }
            int negYDistance = 0;
            for (int y = ty - 1; y >= 0; y--)
            {
                negYDistance++;
                if (Trees[y, tx] >= Trees[ty, tx])
                    break;
            }
            int posYDistance = 0;
            for (int y = ty + 1; y < Y; y++)
            {
                posYDistance++;
                if (Trees[y, tx] >= Trees[ty, tx])
                    break;
            }

            int scenicScore = posXDistance * negXDistance * posYDistance * negYDistance;
            if (scenicScore > maxScenicScore)
                maxScenicScore = scenicScore;
        }
    }
    Console.WriteLine(maxScenicScore);
    Console.ReadLine();
}

P1();
P2();