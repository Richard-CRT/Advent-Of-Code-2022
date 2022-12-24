// 08:24

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

string input = AoC.GetInput();
MatchCollection monkeyMatches = AoC.RegexMatch(input, @"Monkey (\d+):[^:]+:([\d, ]*)[^=]+= ([a-z\d]+) ([+*]) ([a-z\d]+)\W+Test: divisible by (\d+)\D+(\d+)\D+(\d+)");
List<Monkey> Monkeys = monkeyMatches.Select(m => new Monkey(m)).ToList();

void P1()
{
    for (int turn = 0; turn < 20; turn++)
    {
        Monkeys.ForEach(m => m.Turn());
    }
    Monkey[] activeMonkeys = Monkeys.OrderByDescending(x => x.InspectCount).Take(2).ToArray();
    Int64 result = activeMonkeys[0].InspectCount * activeMonkeys[1].InspectCount;
    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    for (int turn = 0; turn < 10_000; turn++)
    {
        Monkeys.ForEach(m => m.Turn(true));
    }
    Monkey[] activeMonkeys = Monkeys.OrderByDescending(x => x.InspectCount).Take(2).ToArray();
    Int64 result = activeMonkeys[0].InspectCount * activeMonkeys[1].InspectCount;
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
Monkeys = monkeyMatches.Select(m => new Monkey(m)).ToList();
P2();

public class Monkey
{
    public static Dictionary<int, Monkey> MonkeyMap = new Dictionary<int, Monkey>();
    public static int LowestCommonDivisor = 1;

    public int Id;
    public Queue<Int64> Items;
    public Func<Int64, Int64> Operation;
    public int DivisbleTestNum;
    public int TrueMonkeyId;
    public int FalseMonkeyId;
    public Int64 InspectCount = 0;
    public Monkey(Match match)
    {
        Id = int.Parse(match.Groups[1].Value);
        Monkey.MonkeyMap[Id] = this;
        string startingItemsString = match.Groups[2].Value;
        string[] split = startingItemsString.Split(',');
        Items = new Queue<Int64>(split.Select(x => Int64.Parse(x.Trim())));
        string operationOperand1 = match.Groups[3].Value;
        string operation = match.Groups[4].Value;
        string operationOperand2 = match.Groups[5].Value;
        if (operation == "+")
            if (operationOperand1 == "old" && operationOperand2 == "old")
                Operation = new Func<Int64, Int64>(x => x + x);
            else if (operationOperand1 == "old")
                Operation = new Func<Int64, Int64>(x => x + Int64.Parse(operationOperand2));
            else if (operationOperand2 == "old")
                Operation = new Func<Int64, Int64>(x => Int64.Parse(operationOperand1) + x);
            else
                Operation = new Func<Int64, Int64>(x => Int64.Parse(operationOperand1) + Int64.Parse(operationOperand2));
        else // *
            if (operationOperand1 == "old" && operationOperand2 == "old")
                Operation = new Func<Int64, Int64>(x => x * x);
            else if (operationOperand1 == "old")
                Operation = new Func<Int64, Int64>(x => x * Int64.Parse(operationOperand2));
            else if (operationOperand2 == "old")
                Operation = new Func<Int64, Int64>(x => Int64.Parse(operationOperand1) * x);
            else
                Operation = new Func<Int64, Int64>(x => Int64.Parse(operationOperand1) * Int64.Parse(operationOperand2));
        DivisbleTestNum = int.Parse(match.Groups[6].Value);
        // This is actually overcomplicated - LCM of a set of primes is just the product
        Monkey.LowestCommonDivisor = AoC.LCM(Monkey.LowestCommonDivisor, DivisbleTestNum);
        TrueMonkeyId = int.Parse(match.Groups[7].Value);
        FalseMonkeyId = int.Parse(match.Groups[8].Value);
    }

    public void Turn(bool p2 = false)
    {
        while (Items.Count > 0)
        {
            InspectCount++;
            Int64 item = Items.Dequeue();
            item = Operation(item);
            if (!p2)
                item /= 3;
            item %= Monkey.LowestCommonDivisor;
            if (item % DivisbleTestNum == 0)
                Monkey.MonkeyMap[TrueMonkeyId].Items.Enqueue(item);
            else
                Monkey.MonkeyMap[FalseMonkeyId].Items.Enqueue(item);
        }
    }
}