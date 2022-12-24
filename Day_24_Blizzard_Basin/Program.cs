// 10:17

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

Map map = new(inputList);
HashSet<string> AllMapsCache = new();
string key = map.ToString();
while (!AllMapsCache.Contains(key))
{
    AllMapsCache.Add(key);
    Map.AllMaps.Add(map);
    map = map.Tick();
    key = map.ToString();
}

void P1_2()
{
    int sum = 0;

    List<State> states = new List<State> { new State(1, 0, 0) };
    HashSet<string> Cache = new();
    int minute = 0;
    State? winningState = null;
    while (winningState == null)
    {
        //Console.WriteLine($"Minute {minute}");
        List<State> newStates = new();
        foreach (State state in states)
        {
            string cacheKey = state.ToString();
            if (!Cache.Contains(cacheKey))
            {
                //state.Print();

                if (state.End)
                {
                    winningState = state;
                    break;
                }

                newStates.AddRange(state.Tick(false));
                Cache.Add(cacheKey);
                if (winningState != null) break;
            }
        }
        //Console.ReadLine();
        states = newStates;
        minute++;
    }
    sum += minute - 1;
    Console.WriteLine(sum);
    Console.ReadLine();

    states = new List<State> { winningState };
    Cache = new();
    minute = 0;
    winningState = null;
    while (winningState == null)
    {
        //Console.WriteLine($"Minute {minute}");
        List<State> newStates = new();
        foreach (State state in states)
        {
            string cacheKey = state.ToString();
            if (!Cache.Contains(cacheKey))
            {
                //state.Print();

                if (state.Start)
                {
                    winningState = state;
                    break;
                }

                newStates.AddRange(state.Tick(true));
                Cache.Add(cacheKey);
                if (winningState != null) break;
            }
        }
        //Console.ReadLine();
        states = newStates;
        minute++;
    }
    //Console.WriteLine(minute - 1);
    sum += minute - 1;

    states = new List<State> { winningState };
    Cache = new();
    minute = 0;
    winningState = null;
    while (winningState == null)
    {
        //Console.WriteLine($"Minute {minute}");
        List<State> newStates = new();
        foreach (State state in states)
        {
            string cacheKey = state.ToString();
            if (!Cache.Contains(cacheKey))
            {
                //state.Print();

                if (state.End)
                {
                    winningState = state;
                    break;
                }

                newStates.AddRange(state.Tick(false));
                Cache.Add(cacheKey);
                if (winningState != null) break;
            }
        }
        //Console.ReadLine();
        states = newStates;
        minute++;
    }
    //Console.WriteLine(minute - 1);
    sum += minute - 1;

    Console.WriteLine(sum);
    Console.ReadLine();
}

P1_2();

public enum Direction
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3,
}

public class State
{
    public int PositionX;
    public int PositionY;
    public int MapIndex;

    public bool End
    {
        get { return PositionX == Map.DimX - 2 && PositionY == Map.DimY - 1; }
    }
    public bool Start
    {
        get { return PositionX == 1 && PositionY == 0; }
    }

    public State(int positionX, int positionY, int mapIndex)
    {
        PositionX = positionX;
        PositionY = positionY;
        MapIndex = mapIndex;
    }

    public List<State> Tick(bool reverse)
    {
        List<State> newStates = new();
        int newMapIndex = (MapIndex + 1) % Map.AllMaps.Count;
        Map newMap = Map.AllMaps[newMapIndex];
        // Check if allowed to stay where we are
        if (!newMap.Blizzards.TryGetValue((PositionX, PositionY), out List<Direction>? blizzards) || blizzards.Count == 0)
        {
            newStates.Add(new(PositionX, PositionY, newMapIndex));
        }

        // Special edge cases
        if (!reverse && PositionX == 1 && PositionY == 0)
        {
            if (newMap.Blizzards.GetValueOrDefault((PositionX, PositionY + 1), new List<Direction>()).Count == 0)
                newStates.Add(new(PositionX, PositionY + 1, newMapIndex));
        }
        else if (reverse && PositionX == 1 && PositionY == 1)
            newStates.Add(new(PositionX, PositionY - 1, newMapIndex));
        else if (reverse && PositionX == Map.DimX - 2 && PositionY == Map.DimY - 1)
        {
            if (newMap.Blizzards.GetValueOrDefault((PositionX, PositionY - 1), new List<Direction>()).Count == 0)
                newStates.Add(new(PositionX, PositionY - 1, newMapIndex));
        }
        else if (!reverse && PositionX == Map.DimX - 2 && PositionY == Map.DimY - 2)
            newStates.Add(new(PositionX, PositionY + 1, newMapIndex));
        else
        {
            // Normal operation
            if (PositionX > 1 && newMap.Blizzards.GetValueOrDefault((PositionX - 1, PositionY), new List<Direction>()).Count == 0)
                newStates.Add(new(PositionX - 1, PositionY, newMapIndex));
            if (PositionX < Map.DimX - 2 && newMap.Blizzards.GetValueOrDefault((PositionX + 1, PositionY), new List<Direction>()).Count == 0)
                newStates.Add(new(PositionX + 1, PositionY, newMapIndex));
            if (PositionY > 1 && newMap.Blizzards.GetValueOrDefault((PositionX, PositionY - 1), new List<Direction>()).Count == 0)
                newStates.Add(new(PositionX, PositionY - 1, newMapIndex));
            if (PositionY < Map.DimY - 2 && newMap.Blizzards.GetValueOrDefault((PositionX, PositionY + 1), new List<Direction>()).Count == 0)
                newStates.Add(new(PositionX, PositionY + 1, newMapIndex));
        }
        return newStates;
    }

    public override string ToString()
    {
        return $"{PositionX}|{PositionY}|{MapIndex}";
    }

    public void Print()
    {
        for (int y = 0; y < Map.DimY; y++)
        {
            for (int x = 0; x < Map.DimX; x++)
            {
                if (x == PositionX && y == PositionY)
                    Console.Write('E');
                else
                    Console.Write(Map.AllMaps[MapIndex].ToChar(x, y));
            }
            Console.WriteLine();
        }
    }
}

public class Map
{
    public static int DimX;
    public static int DimY;
    public static List<Map> AllMaps = new List<Map>();

    public Dictionary<(int, int), List<Direction>> Blizzards = new();

    public Map() { }

    public Map(List<string> inputList)
    {
        DimX = inputList[0].Count();
        DimY = inputList.Count;

        for (int y = 0; y < DimY; y++)
        {
            for (int x = 0; x < DimX; x++)
            {
                if (!Blizzards.ContainsKey((x, y)))
                    Blizzards[(x, y)] = new List<Direction>();
                switch (inputList[y][x])
                {
                    case '<': Blizzards[(x, y)].Add(Direction.Left); break;
                    case '>': Blizzards[(x, y)].Add(Direction.Right); break;
                    case '^': Blizzards[(x, y)].Add(Direction.Up); break;
                    case 'v': Blizzards[(x, y)].Add(Direction.Down); break;
                }
            }
        }
    }

    public Map Tick()
    {
        Map newMap = new Map();
        foreach (var kvp in Blizzards)
        {
            (int x, int y) = kvp.Key;
            List<Direction> ds = kvp.Value;
            foreach (Direction d in ds)
            {
                int newX = x;
                int newY = y;
                switch (d)
                {
                    case Direction.Up: newY--; break;
                    case Direction.Down: newY++; break;
                    case Direction.Left: newX--; break;
                    case Direction.Right: newX++; break;
                    default:
                        throw new Exception();
                }
                if (newX <= 0)
                    newX = DimX - 2;
                else if (newX >= DimX - 1)
                    newX = 1;
                if (newY <= 0)
                    newY = DimY - 2;
                else if (newY >= DimY - 1)
                    newY = 1;

                (int, int) newK = (newX, newY);
                if (!newMap.Blizzards.ContainsKey(newK))
                    newMap.Blizzards[newK] = new();
                newMap.Blizzards[newK].Add(d);
            }
        }
        return newMap;
    }

    public char ToChar(int x, int y)
    {
        if (
            (y == 0 && x == 1) ||
            (y == Map.DimY - 1 && x == Map.DimX - 2)
            )
            return '░';
        else if (
            y == 0 || y == DimY - 1 || x == 0 || x == DimX - 1
            )
            return '█';
        else
        {
            var k = (x, y);
            if (!Blizzards.ContainsKey(k) || Blizzards[k].Count == 0)
                return '░';
            else if (Blizzards[k].Count > 1)
                return Blizzards[k].Count.ToString()[0];
            else
            {
                switch (Blizzards[k][0])
                {
                    case Direction.Up: return '▲';
                    case Direction.Down: return '▼';
                    case Direction.Left: return '◄';
                    case Direction.Right: return '►';
                    default: throw new Exception();
                }
            }
        }
    }

    public override string ToString()
    {
        string s = "";
        for (int y = 0; y < Map.DimY; y++)
        {
            for (int x = 0; x < Map.DimX; x++)
            {
                s += this.ToChar(x, y);
            }
            s += "\n";
        }
        return s;
    }
}