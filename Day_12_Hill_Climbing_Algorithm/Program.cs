// 07:03

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
int Height = inputList.Count;
int Width = inputList[0].Length;
Node[,] Map = new Node[Height, Width];
Node[,] Map2 = new Node[Height, Width];
Node? StartNode = null;
Node? EndNode = null;
Node? EndNode2 = null;
for (int y = 0; y < Height; y++)
{
    for (int x = 0; x < Width; x++)
    {
        Map[y, x] = new Node(y, x, inputList[y][x]);
        Map2[y, x] = new Node(y, x, inputList[y][x]);
        if (inputList[y][x] == 'S')
            StartNode = Map[y, x];
        else if (inputList[y][x] == 'E')
        {
            EndNode = Map[y, x];
            EndNode2 = Map2[y, x];
        }
    }
}
for (int y = 0; y < Height; y++)
{
    for (int x = 0; x < Width; x++)
    {
        if (x > 0)
        {
            Map[y, x].Neighbours.Add((Map[y, x - 1].Elevation - Map[y, x].Elevation, Map[y, x - 1]));
            Map2[y, x].Neighbours.Add((Map2[y, x - 1].Elevation - Map2[y, x].Elevation, Map2[y, x - 1]));
        }
        if (x < Width - 1)
        {
            Map[y, x].Neighbours.Add((Map[y, x + 1].Elevation - Map[y, x].Elevation, Map[y, x + 1]));
            Map2[y, x].Neighbours.Add((Map2[y, x + 1].Elevation - Map2[y, x].Elevation, Map2[y, x + 1]));
        }
        if (y > 0)
        {
            Map[y, x].Neighbours.Add((Map[y - 1, x].Elevation - Map[y, x].Elevation, Map[y - 1, x]));
            Map2[y, x].Neighbours.Add((Map2[y - 1, x].Elevation - Map2[y, x].Elevation, Map2[y - 1, x]));
        }
        if (y < Height - 1)
        {
            Map[y, x].Neighbours.Add((Map[y + 1, x].Elevation - Map[y, x].Elevation, Map[y + 1, x]));
            Map2[y, x].Neighbours.Add((Map2[y + 1, x].Elevation - Map2[y, x].Elevation, Map2[y + 1, x]));
        }
    }
}
if (StartNode == null || EndNode == null || EndNode2 == null)
    throw new NotImplementedException();

void P1()
{
    StartNode.MinDistance = 0;
    HashSet<Node> VisitedNodes = new HashSet<Node>();
    HashSet<Node> DistancedNodes = new HashSet<Node> { StartNode };
    while (!VisitedNodes.Contains(EndNode))
    {
        Node? currentNode = DistancedNodes.MinBy(t => t.MinDistance);
        if (currentNode == null)
            throw new NotImplementedException();

        VisitedNodes.Add(currentNode);
        DistancedNodes.Remove(currentNode);

        foreach ((int elevationChange, Node neighbour) in currentNode.Neighbours)
        {
            if (elevationChange <= 1 && (neighbour.MinDistance == null || currentNode.MinDistance + 1 < neighbour.MinDistance))
            {
                neighbour.MinDistance = currentNode.MinDistance + 1;
                DistancedNodes.Add(neighbour);
            }
        }
    }

    Console.WriteLine(EndNode.MinDistance);
    Console.ReadLine();
}

void P2()
{
    EndNode2.MinDistance = 0;
    HashSet<Node> VisitedNodes = new HashSet<Node>();
    HashSet<Node> DistancedNodes = new HashSet<Node> { EndNode2 };
    int MinDistance = int.MaxValue;
    while (DistancedNodes.Count > 0)
    {
        Node? currentNode = DistancedNodes.MinBy(t => t.MinDistance);
        if (currentNode == null)
            throw new NotImplementedException();

        VisitedNodes.Add(currentNode);
        DistancedNodes.Remove(currentNode);

        if (currentNode.Elevation == 0 && currentNode.MinDistance != null && currentNode.MinDistance < MinDistance)
            MinDistance = (int)currentNode.MinDistance;

        foreach ((int elevationChange, Node neighbour) in currentNode.Neighbours)
        {
            if (elevationChange >= -1 && (neighbour.MinDistance == null || currentNode.MinDistance + 1 < neighbour.MinDistance))
            {
                neighbour.MinDistance = currentNode.MinDistance + 1;
                DistancedNodes.Add(neighbour);
            }
        }
    }

    Console.WriteLine(MinDistance);
    Console.ReadLine();
}

P1();
P2();

public class Node
{
    public int X;
    public int Y;
    public int Elevation;
    public int? MinDistance = null;
    public List<(int, Node)> Neighbours = new List<(int, Node)>();

    public Node(int x, int y, char c)
    {
        X = x;
        Y = y;
        if (c == 'S')
            Elevation = 0;
        else if (c == 'E')
            Elevation = 25;
        else
            Elevation = c - 'a';
    }
}