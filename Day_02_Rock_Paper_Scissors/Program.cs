using AdventOfCodeUtilities;

List<string> inputList = AoCUtilities.GetInputLines();

void P1()
{
    int score = 0;
    foreach (string str in inputList)
    {
        int otherChoice = str[0] - 'A' + 1;
        int yourChoice = str[2] - 'X' + 1;

        score += yourChoice;
        if (yourChoice == otherChoice)
        {
            score += 3;
        }
        else if (yourChoice - otherChoice == 1 || (yourChoice == 1 && otherChoice == 3))
        {
            score += 6;
        }
    }

    Console.WriteLine(score);
    Console.ReadLine();
}

void P2()
{
    int score = 0;
    foreach (string str in inputList)
    {
        int otherChoice = str[0] - 'A' + 1;
        int end = str[2] - 'X' + 1;

        int yourChoice;
        if (end == 1)
            yourChoice = otherChoice - 1;
        else if (end == 2)
        {
            score += 3;
            yourChoice = otherChoice;
        }
        else
        {
            score += 6;
            yourChoice = otherChoice + 1;
        }

        if (yourChoice == 0)
            yourChoice = 3;
        else if (yourChoice == 4)
            yourChoice = 1;

        score += yourChoice;
    }

    Console.WriteLine(score);
    Console.ReadLine();
}

P1();
P2();