using AdventOfCodeUtilities;

List<string> inputList = AoC.GetInputLines();
int p1count = 0;
int p2count = 0;
foreach (string str in inputList)
{
    string[] split = str.Split(',');
    string r1 = split[0];
    string r2 = split[1];
    string[] r1Split = r1.Split('-');
    string[] r2Split = r2.Split('-');
    int r1l = int.Parse(r1Split[0]);
    int r1h = int.Parse(r1Split[1]);
    int r2l = int.Parse(r2Split[0]);
    int r2h = int.Parse(r2Split[1]);
    bool r2containsr1 = r1h >= r2h && r1l <= r2l;
    bool r1containsr2 = r2h >= r1h && r2l <= r1l;
    bool contains = r2containsr1 || r1containsr2;
    if (contains)
        p1count++;
    if (contains || (r1l <= r2l && r1h >= r2l) || (r1h >= r2h && r1l <= r2h))
        p2count++;
}

void P1()
{
    Console.WriteLine(p1count);
    Console.ReadLine();
}

void P2()
{
    Console.WriteLine(p2count);
    Console.ReadLine();
}

P1();
P2();