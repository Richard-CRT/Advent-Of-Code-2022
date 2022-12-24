using AdventOfCodeUtilities;
using System.Numerics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Node> Nodes = inputList.Select(x => new Node(x)).ToList();
List<Node> Nodes2 = inputList.Select(x => new Node(x)).ToList();

Node ZeroNode = Nodes.Where(x => x.Value == 0).First();

Nodes.First().Previous = Nodes.Last();
Nodes.First().Next = Nodes[1];
Nodes.Last().Next = Nodes.First();
Nodes.Last().Previous = Nodes[Nodes.Count - 2];
for (int i = 1; i <= Nodes.Count - 2; i++)
{
    Nodes[i].Previous = Nodes[i - 1];
    Nodes[i].Next = Nodes[i + 1];
}


Node ZeroNode2 = Nodes2.Where(x => x.Value == 0).First();

Nodes2.First().Previous = Nodes2.Last();
Nodes2.First().Next = Nodes2[1];
Nodes2.Last().Next = Nodes2.First();
Nodes2.Last().Previous = Nodes2[Nodes2.Count - 2];
for (int i = 1; i <= Nodes2.Count - 2; i++)
{
    Nodes2[i].Previous = Nodes2[i - 1];
    Nodes2[i].Next = Nodes2[i + 1];
}
Nodes2.ForEach(x => x.Value *= 811589153);

void P1()
{
    //Nodes.First().Print(Nodes.Count); Console.WriteLine();

    foreach (Node node in Nodes)
    {
        node.Shift(node.Value, Nodes.Count);

        //Nodes.First().Print(Nodes.Count); Console.WriteLine();
    }

    Node _1000 = ZeroNode.Get(1000, Nodes.Count);
    Node _2000 = _1000.Get(1000, Nodes.Count);
    Node _3000 = _2000.Get(1000, Nodes.Count);

    Int64 result = _1000.Value + _2000.Value + _3000.Value;
    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    //Nodes2.First().Print(Nodes2.Count); Console.WriteLine();

    for (int i = 0; i < 10; i++)
    {
        foreach (Node node in Nodes2)
        {
            node.Shift(node.Value, Nodes2.Count);
        }

        //Nodes2.First().Print(Nodes2.Count); Console.WriteLine();
    }

    Node _1000 = ZeroNode2.Get(1000, Nodes2.Count);
    Node _2000 = _1000.Get(1000, Nodes2.Count);
    Node _3000 = _2000.Get(1000, Nodes2.Count);

    Int64 result = _1000.Value + _2000.Value + _3000.Value;
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Node
{
    public Int64 Value;
    public Node Previous;
    public Node Next;

    public Node(string s)
    {
        Value = int.Parse(s);
        Previous = this;
        Next = this;
    }

    public void Shift(Int64 hops, Int64 len)
    {
        if (hops > 0)
        {
            this.Previous.Next = this.Next;
            this.Next.Previous = this.Previous;

            Node n = Get(hops, len - 1); // Must mod by 1 less than count when shifting

            this.Next = n.Next;
            this.Previous = n;

            n.Next.Previous = this;
            n.Next = this;
        }
        else if (hops < 0)
        {
            this.Previous.Next = this.Next;
            this.Next.Previous = this.Previous;

            Node n = Get(hops, len - 1); // Must mod by 1 less than count when shifting

            this.Next = n;
            this.Previous = n.Previous;

            n.Previous.Next = this;
            n.Previous = this;
        }
    }

    public Node Get(Int64 hops, Int64 len)
    {
        hops = hops % len;
        if (hops > 0)
        {
            Node n = this;
            for (Int64 i = 0; i < hops; i++)
            {
                n = n.Next;
            }
            return n;
        }
        else if (hops < 0)
        {
            Node n = this;
            for (Int64 i = 0; i > hops; i--)
            {
                n = n.Previous;
            }
            return n;
        }
        return this;
    }

    public void Print(int depth)
    {
        Console.Write($"{Value}, ");
        if (depth > 1)
            this.Next.Print(depth - 1);
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}