using AdventOfCodeUtilities;

List<string> inputList = AoCUtilities.GetInputLines();
List<int> elfTotals = new List<int>();
int runningTotal = 0;
foreach (string str in inputList)
{
    if (str == "")
    {
        elfTotals.Add(runningTotal);
        runningTotal = 0;
    }
    else
    {
        runningTotal += int.Parse(str);
    }
}

void P1()
{
    Console.WriteLine(elfTotals.Max());
    Console.ReadLine();
}

void P2()
{
    Console.WriteLine(elfTotals.OrderByDescending(x => x).ToList().GetRange(0, 3).Sum());
    Console.ReadLine();
}

P1();
P2();