// 09:50

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoCUtilities.GetInputLines();
List<Monkey> Monkeys = inputList.Select(m => new Monkey(m)).ToList();
Monkeys.ForEach(m => m.Setup());
Monkey RootMonkey = Monkey.AllMonkeys["root"];

void P1()
{
    Console.WriteLine(RootMonkey.Yell());
    Console.ReadLine();
}

void P2()
{
    Monkey m = Monkey.AllMonkeys["humn"]; ;
    while (m.Parent != null)
    {
        m.Parent.HumanPathMonkey = m;
        m.Parent.NonHumanPathMonkey = m == m.Parent.Monkey1 ? m.Parent.Monkey2 : m.Parent.Monkey1;
        m = m.Parent;
    }

    if (RootMonkey.HumanPathMonkey != null && RootMonkey.NonHumanPathMonkey != null)
    {
        Int64 RootHumanPartMustBe = RootMonkey.NonHumanPathMonkey.Yell();
        Int64 HumanMustBe = RootMonkey.HumanPathMonkey.Target(RootHumanPartMustBe);
        Console.WriteLine(HumanMustBe);
        Console.ReadLine();
    }
    else
        throw new Exception();
}

P1();
P2();

public class Monkey
{
    public static Dictionary<string, Monkey> AllMonkeys = new Dictionary<string, Monkey>();

    public string Name;
    private string Function;
    public Int64? Value = null;
    public Monkey? Monkey1 = null;
    public Monkey? Monkey2 = null;

    public Monkey? HumanPathMonkey = null;
    public Monkey? NonHumanPathMonkey = null;
    public char? Operation = null;

    public Monkey? Parent = null;

    public Monkey(string s)
    {
        string[] split = s.Split(':');
        Name = split[0];
        AllMonkeys[Name] = this;
        Function = split[1].Trim();
    }

    public void Setup()
    {
        if (Function.Length == 11)
        {
            string[] split = Function.Split(' ');
            Monkey1 = Monkey.AllMonkeys[split[0]];
            Monkey2 = Monkey.AllMonkeys[split[2]];
            Monkey1.Parent = this;
            Monkey2.Parent = this;
            Operation = split[1][0];
        }
        else
        {
            Value = Int64.Parse(Function);
        }
    }

    public override string ToString()
    {
        if (Value != null)
            return $"{Name}: {Value}";
        else if (Monkey1 != null && Monkey2 != null && Operation != null)
            return $"{Name}: {Monkey1.Name} {Operation} {Monkey2.Name}";
        else
            throw new NotImplementedException();
    }

    public Int64 Yell()
    {
        if (Value != null)
            return (Int64)Value;
        else if (Monkey1 != null && Monkey2 != null && Operation != null)
        {
            switch (Operation)
            {
                case '+': return Monkey1.Yell() + Monkey2.Yell();
                case '-': return Monkey1.Yell() - Monkey2.Yell();
                case '*': return Monkey1.Yell() * Monkey2.Yell();
                case '/': return Monkey1.Yell() / Monkey2.Yell();
                case '=': return Monkey1.Yell() == Monkey2.Yell() ? 1 : 0;
                default: throw new NotImplementedException();
            }
        }
        else
            throw new NotImplementedException();
    }

    public Int64 Target(Int64 target)
    {
        if (this == Monkey.AllMonkeys["humn"])
            return target;
        if (this.HumanPathMonkey == null || this.NonHumanPathMonkey == null)
            throw new Exception();
        else
        {
            Int64 nonHumanPathYell = this.NonHumanPathMonkey.Yell();
            switch (Operation)
            {
                case '+': return HumanPathMonkey.Target(target - nonHumanPathYell);
                case '-':
                    {
                        if (HumanPathMonkey == Monkey1)
                            return HumanPathMonkey.Target(target + nonHumanPathYell);
                        else
                            return HumanPathMonkey.Target(nonHumanPathYell - target);
                    }
                case '*': return HumanPathMonkey.Target(target / nonHumanPathYell);
                case '/':
                    {
                        if (HumanPathMonkey == Monkey1)
                            return HumanPathMonkey.Target(target * nonHumanPathYell);
                        else
                            return HumanPathMonkey.Target(nonHumanPathYell / target);
                    }
                default: throw new NotImplementedException();
            }
        }
    }
}