// P1 started at 05:00
// P2 stopped at 08:00 and resumed at 11:11

using AdventOfCodeUtilities;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

List<string> inputList = AoCUtilities.GetInputLines();
List<Valve> Valves = inputList.Select(x => Valve.Factory(x)).ToList();

foreach (Valve startValve in Valves)
{
    foreach (Valve valve in Valves)
        valve.MinMinutes = int.MaxValue;

    startValve.MinMinutes = 0;
    HashSet<Valve> visitedValves = new HashSet<Valve>();
    HashSet<Valve> measuredValves = new HashSet<Valve> { startValve };
    while (visitedValves.Count < Valves.Count)
    {
        Valve? currentValve = measuredValves.MinBy(t => t.MinMinutes);
        if (currentValve == null || currentValve.MinMinutes == null)
            throw new NotImplementedException();

        visitedValves.Add(currentValve);
        measuredValves.Remove(currentValve);
        Valve.Costs[(startValve.ID, currentValve.ID)] = (int)currentValve.MinMinutes;

        foreach (Valve neighbour in currentValve.Neighbours)
        {
            if (neighbour.MinMinutes == null || currentValve.MinMinutes + 1 < neighbour.MinMinutes)
            {
                neighbour.MinMinutes = currentValve.MinMinutes + 1;
                measuredValves.Add(neighbour);
            }
        }
    }
}

Valves = Valves.Where(x => x.FlowRate > 0).ToList();

Dictionary<string, int> IntCache = new Dictionary<string, int>();
Dictionary<string, List<(List<Valve>, HashSet<string>, int)>> SetCache = new Dictionary<string, List<(List<Valve>, HashSet<string>, int)>>();

int recurseInt(HashSet<string> unvisitedValves, int minutesRemaining, Valve currentValve)
{
    string cacheKey = $"{string.Join(',', unvisitedValves)}|{minutesRemaining}|{currentValve}";
    if (IntCache.ContainsKey(cacheKey))
        return IntCache[cacheKey];

    int releaseByThisValve = currentValve.FlowRate * minutesRemaining;
    int maxReleaseByChildren = 0;
    foreach (string unvisitedValveS in unvisitedValves)
    {
        Valve unvisitedValve = Valve.Valves[unvisitedValveS];
        int costFromCurrentValveToUnvisitedValve = Valve.Costs[(currentValve.ID, unvisitedValve.ID)];

        int newMinutesRemaining = minutesRemaining - (costFromCurrentValveToUnvisitedValve + 1);

        // Need the minutes to move there, plus 1 for opening, plus 1 for it to actually perform
        if (newMinutesRemaining > 0)
        {
            HashSet<string> unvisitedValvesCopy = new HashSet<string>(unvisitedValves);

            unvisitedValvesCopy.Remove(unvisitedValveS);
            int trial = recurseInt(unvisitedValves: unvisitedValvesCopy, minutesRemaining: newMinutesRemaining, currentValve: unvisitedValve);
            if (trial > maxReleaseByChildren)
            {
                maxReleaseByChildren = trial;
            }
        }
    }

    int val = releaseByThisValve + maxReleaseByChildren;
    IntCache[cacheKey] = val;
    return val;
}

List<(List<Valve>, HashSet<string>, int)> recurseList(HashSet<string> unvisitedValves, int minutesRemaining, Valve currentValve)
{
    string cacheKey = $"{string.Join(',', unvisitedValves)}|{minutesRemaining}|{currentValve}";
    if (SetCache.ContainsKey(cacheKey))
        return SetCache[cacheKey];
    int releaseByThisValve = currentValve.FlowRate * minutesRemaining;
    List<(List<Valve>, HashSet<string>, int)> paths;
    if (currentValve.ID != "AA")
        paths = new List<(List<Valve>, HashSet<string>, int)> { (new List<Valve>() { currentValve }, new HashSet<string>() { currentValve.ID }, releaseByThisValve) };
    else
        paths = new List<(List<Valve>, HashSet<string>, int)> { (new List<Valve>(), new HashSet<string>(), releaseByThisValve) };
    foreach (string unvisitedValveS in unvisitedValves)
    {
        Valve unvisitedValve = Valve.Valves[unvisitedValveS];
        int costFromCurrentValveToUnvisitedValve = Valve.Costs[(currentValve.ID, unvisitedValve.ID)];

        int newMinutesRemaining = minutesRemaining - (costFromCurrentValveToUnvisitedValve + 1);

        // Need the minutes to move there, plus 1 for opening, plus 1 for it to actually perform
        if (newMinutesRemaining > 0)
        {
            HashSet<string> unvisitedValvesCopy = new HashSet<string>(unvisitedValves);

            unvisitedValvesCopy.Remove(unvisitedValveS);
            List<(List<Valve>, HashSet<string>, int)> trial = recurseList(unvisitedValves: unvisitedValvesCopy, minutesRemaining: newMinutesRemaining, currentValve: unvisitedValve);
            foreach ((List<Valve> l, HashSet<string> h, int release) in trial)
            {
                // Have to make copy here, or we end up modifying the cache which is bad!
                List<Valve> lCopy = new List<Valve>(l);
                HashSet<string> hCopy = new HashSet<string>(h);
                if (currentValve.ID != "AA")
                {
                    lCopy.Insert(0, currentValve);
                    hCopy.Add(currentValve.ID);
                }
                paths.Add((lCopy, hCopy, releaseByThisValve + release));
            }
        }
    }
    SetCache[cacheKey] = paths;
    return paths;
}

int P1Best = 0;

void P1()
{
    HashSet<string> unvisitedValves = new HashSet<string>(Valves.Select(x => x.ID));
    unvisitedValves.Remove("AA");
    //List<(List<Valve>, HashSet<string>, int)> paths = recurseList(unvisitedValves: unvisitedValves, minutesRemaining: 30, currentValve: Valve.Valves["AA"]);
    //P1Best = paths.MaxBy(x => x.Item3).Item3;
    P1Best = recurseInt(unvisitedValves: unvisitedValves, minutesRemaining: 30, currentValve: Valve.Valves["AA"]);
    Console.WriteLine(P1Best);
    Console.ReadLine();
}

void P2()
{
    HashSet<string> unvisitedValves = new HashSet<string>(Valves.Select(x => x.ID));
    unvisitedValves.Remove("AA");
    HashSet<string> unvisitedValvesCopy = new HashSet<string>(unvisitedValves);
    int maxRelease = 0;
    int maxYRelease = 0;
    int maxERelease = 0;
    List<Valve> bestYPath = new List<Valve>();
    List<Valve> bestEPath = new List<Valve>();
    List<(List<Valve>, HashSet<string>, int)> paths = recurseList(unvisitedValves: unvisitedValvesCopy, minutesRemaining: 26, currentValve: Valve.Valves["AA"]);
    int onePerc = paths.Count / 100;
    for (int i = 0; i < paths.Count; i++)
    {
        if (i % onePerc == 0)
            Console.WriteLine($"{i / onePerc}%");
        for (int j = 0; j < paths.Count; j++)
        {
            (List<Valve> yPath, HashSet<string> ySet, int yRelease) = paths[i];
            (List<Valve> ePath, HashSet<string> eSet, int eRelease) = paths[j];
            if (i != j)
            {
                int trial = yRelease + eRelease;
                if (trial > P1Best)
                {
                    bool intersect = false;
                    foreach (string s in ySet)
                    {
                        if (eSet.Contains(s))
                        {
                            intersect = true;
                            break;
                        }
                    }
                    if (!intersect)
                    {
                        if (trial > maxRelease)
                        {
                            maxRelease = trial;
                            maxYRelease = yRelease;
                            maxERelease = eRelease;
                            bestYPath = yPath;
                            bestEPath = ePath;
                        }
                    }
                }
            }
        }
    }
    bestYPath.Insert(0, Valve.Valves["AA"]);
    bestEPath.Insert(0, Valve.Valves["AA"]);
    //Console.WriteLine(string.Join(", ", bestYPath));
    //Console.WriteLine(maxYRelease);
    //Console.WriteLine(string.Join(", ", bestEPath));
    //Console.WriteLine(maxERelease);
    Console.WriteLine(maxRelease);
    Console.ReadLine();
}

P1();
P2();

public class Valve
{
    public static Dictionary<string, Valve> Valves = new Dictionary<string, Valve>();
    public static Dictionary<(string, string), int> Costs = new Dictionary<(string, string), int>();

    public string ID;
    public int FlowRate;
    public List<Valve> Neighbours = new List<Valve>();

    public int? MinMinutes = null;

    public static Valve SafeGet(string id)
    {
        if (!Valve.Valves.ContainsKey(id))
            Valve.Valves[id] = new Valve(id);
        return Valve.Valves[id];
    }

    public static Valve Factory(string s)
    {
        string[] split = s.Split(' ');
        string ID = split[1];

        if (!Valve.Valves.ContainsKey(ID))
            Valve.Valves[ID] = new Valve(ID);


        Valve.Valves[ID].FlowRate = int.Parse(split[4].Substring(5, split[4].Length - 6));
        for (int i = 9; i < split.Length; i++)
        {
            Valve.Valves[ID].Neighbours.Add(Valve.SafeGet(split[i].Trim(',')));
        }

        return Valve.Valves[ID];
    }

    public Valve(string id)
    {
        ID = id;
    }

    public override string ToString()
    {
        return $"{ID}";
    }
}