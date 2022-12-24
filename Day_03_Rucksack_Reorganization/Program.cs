using AdventOfCodeUtilities;
using System.Diagnostics;

List<string> inputList = AoC.GetInputLines();
List<Rucksack> Rucksacks = inputList.Select(s => new Rucksack(s)).ToList();

void P1()
{
    int prioritySum = 0;
    foreach (Rucksack rucksack in Rucksacks)
    {
        Item? dupe = rucksack.GetDuplicate();
        Debug.Assert(dupe != null);
        prioritySum += dupe.Priority;
    }
    Console.WriteLine(prioritySum);
    Console.ReadLine();
}

void P2()
{
    int prioritySum = 0;
    for (int i = 0; i < Rucksacks.Count; i += 3)
    {
        prioritySum += Rucksacks[i].GetCommon(Rucksacks[i + 1], Rucksacks[i + 2]).Priority;
    }
    Console.WriteLine(prioritySum);
    Console.ReadLine();
}

P1();
P2();

public class Rucksack
{
    public List<Item> Items1 { get; set; } = new List<Item>();
    public List<Item> Items2 { get; set; } = new List<Item>();

    public Rucksack(string s)
    {
        Debug.Assert(s.Length % 2 == 0);
        Items1 = s.Take(s.Length / 2).Select(c => Item.Factory(c)).ToList();
        Items2 = s.Skip(s.Length / 2).Take(s.Length / 2).Select(c => Item.Factory(c)).ToList();
    }

    public Item? GetDuplicate()
    {
        foreach (Item item in Items1)
            if (Items2.Contains(item))
                return item;
        return null;
    }

    public Item GetCommon(Rucksack oR1, Rucksack oR2)
    {
        return Items1.Union(Items2).Intersect(oR1.Items1.Union(oR1.Items2).Intersect(oR2.Items1.Union(oR2.Items2))).First();
    }
}

public class Item
{
    public static Dictionary<char, Item> Cache = new Dictionary<char, Item>();
    public static Item Factory(char c)
    {
        if (!Cache.ContainsKey(c))
            Cache.Add(c, new Item(c));
        return Cache[c];
    }

    public char Identifier { get; set; }
    public int Priority { get; set; }

    public Item(char c)
    {
        Identifier = c;
        Priority = 0;
        if (Identifier >= 'a' && Identifier <= 'z')
            Priority = Identifier - 'a' + 1;
        else if (Identifier >= 'A' && Identifier <= 'Z')
            Priority = Identifier - 'A' + 27;
    }
}