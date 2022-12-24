using AdventOfCodeUtilities;

List<string> inputList = AoC.GetInputLines();
int stackCount = (inputList[0].Length + 1) / 4;
List<Crate>[] Lists = new List<Crate>[stackCount];
int i;
for (i = 0; inputList[i].Contains('['); i++)
{
    string s = inputList[i];
    for (int stackNum = 0; stackNum < stackCount; stackNum++)
    {
        int index = stackNum * 4;
        if (s[index] == '[')
        {
            Crate newCrate = new Crate(s[index + 1]);
            if (Lists[stackNum] == null)
                Lists[stackNum] = new List<Crate>();
            Lists[stackNum].Add(newCrate);
        }
    }
}
Stack<Crate>[] Stacks = new Stack<Crate>[stackCount];
for (int j = 0; j < stackCount; j++)
{
    Lists[j].Reverse();
    Stacks[j] = new Stack<Crate>(Lists[j]);
}
i += 2;
List<(int, int, int)> Instructions = new List<(int, int, int)>();
for (; i < inputList.Count; i++)
{
    string[] split = inputList[i].Split(' ');
    int count = int.Parse(split[1]);
    int start = int.Parse(split[3]);
    int dest = int.Parse(split[5]);
    Instructions.Add((count, start, dest));
}


void P1()
{
    Stack<Crate>[] StacksP1 = new Stack<Crate>[stackCount];
    for (int i = 0; i < stackCount; i++)
    {
        StacksP1[i] = new Stack<Crate>(Stacks[i].Reverse());
    }

    foreach ((int count,int start,int dest) in Instructions)
    {
        for (int i = 0; i < count; i++)
        {
            StacksP1[dest - 1].Push(StacksP1[start - 1].Pop());
        }
    }

    string result = "";
    for (int i = 0; i < stackCount; i++)
    {
        result += StacksP1[i].Peek();
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    foreach ((int count, int start, int dest) in Instructions)
    {
        Stack<Crate> tempStack = new Stack<Crate>();
        for (int i = 0; i < count; i++)
        {
            tempStack.Push(Stacks[start - 1].Pop());
        }
        for (int i = 0; i < count; i++)
        {
            Stacks[dest - 1].Push(tempStack.Pop());
        }
    }

    string result = "";
    for (int i = 0; i < stackCount; i++)
    {
        result += Stacks[i].Peek();
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Crate
{
    public char Identifier { get; set; }

    public Crate(char identifier)
    {
        Identifier = identifier;
    }

    public override string ToString()
    {
        return Identifier.ToString();
    }
}