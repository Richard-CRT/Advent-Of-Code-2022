using AdventOfCodeUtilities;
using System.Security.Cryptography.X509Certificates;

List<string> inputList = AoCUtilities.GetInputLines();
Blueprint[] Blueprints = inputList.Select(x => new Blueprint(x)).ToArray();

Dictionary<(int, string), int> Cache = new Dictionary<(int, string), int>();
int recurse(Blueprint blueprint, int minute, Stats stats)
{
    (int, string) cacheKey = (minute, stats.Key());
    if (Cache.ContainsKey(cacheKey))
        return Cache[cacheKey];

    int ret;
    if (minute < 24)
    {
        Stats prevStats = stats.Copy();

        stats.Ore += stats.OreRobots;
        stats.Clay += stats.ClayRobots;
        stats.Obsidian += stats.ObsidianRobots;
        stats.Geodes += stats.GeodeRobots;

        List<Stats> possibleStats = new List<Stats>();
        bool canAffordAll = true;
        if (prevStats.Ore >= blueprint.OreRobot_Ore)
        {
            Stats newStats = stats.Copy();
            newStats.Ore -= blueprint.OreRobot_Ore;
            newStats.OreRobots++;
            possibleStats.Add(newStats);
        }
        else
            canAffordAll = false;
        if (prevStats.Ore >= blueprint.ClayRobot_Ore)
        {
            Stats newStats = stats.Copy();
            newStats.Ore -= blueprint.ClayRobot_Ore;
            newStats.ClayRobots++;
            possibleStats.Add(newStats);
        }
        else
            canAffordAll = false;
        if (prevStats.Ore >= blueprint.ObsidianRobot_Ore && prevStats.Clay >= blueprint.ObsidianRobot_Clay)
        {
            Stats newStats = stats.Copy();
            newStats.Ore -= blueprint.ObsidianRobot_Ore;
            newStats.Clay -= blueprint.ObsidianRobot_Clay;
            newStats.ObsidianRobots++;
            possibleStats.Add(newStats);
        }
        else
            canAffordAll = false;
        if (prevStats.Ore >= blueprint.GeodeRobot_Ore && prevStats.Obsidian >= blueprint.GeodeRobot_Obsidian)
        {
            Stats newStats = stats.Copy();
            newStats.Ore -= blueprint.GeodeRobot_Ore;
            newStats.Obsidian -= blueprint.GeodeRobot_Obsidian;
            newStats.GeodeRobots++;
            possibleStats.Add(newStats);
        }
        else
            canAffordAll = false;
        if (!canAffordAll)
            possibleStats.Add(stats.Copy());

        if (possibleStats.Count == 0)
            throw new NotImplementedException();

        ret = possibleStats.Select(pS => recurse(blueprint, minute + 1, pS)).Max();
    }
    else
    {
        ret = stats.Geodes;
    }
    Cache[cacheKey] = ret;
    return ret;
}

void P1()
{
    foreach (Blueprint blueprint in Blueprints)
    {
        Console.WriteLine(blueprint.Total_GeodeRobot_Ore);

    }
    //Console.WriteLine(recurse(Blueprints[0], 0, new Stats(0, 0, 0, 0, 1, 0, 0, 0)));
    Console.ReadLine();
}

void P2()
{
    int result = 0;
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Stats
{
    public int Ore;
    public int Clay;
    public int Obsidian;
    public int Geodes;

    public int OreRobots;
    public int ClayRobots;
    public int ObsidianRobots;
    public int GeodeRobots;

    public Stats(int ore, int clay, int obsidian, int geodes, int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots)
    {
        Ore = ore;
        Clay = clay;
        Obsidian = obsidian;
        Geodes = geodes;
        OreRobots = oreRobots;
        ClayRobots = clayRobots;
        ObsidianRobots = obsidianRobots;
        GeodeRobots = geodeRobots;
    }

    public Stats Copy()
    {
        return new Stats(Ore, Clay, Obsidian, Geodes, OreRobots, ClayRobots, ObsidianRobots, GeodeRobots);
    }

    public string Key()
    {
        return $"{Ore},{Clay},{Obsidian},{Geodes}|{OreRobots},{ClayRobots},{ObsidianRobots},{GeodeRobots}";
    }
}

public class Blueprint
{
    public int ID;

    public int OreRobot_Ore;
    public int ClayRobot_Ore;
    public int ObsidianRobot_Ore;
    public int ObsidianRobot_Clay;
    public int GeodeRobot_Ore;
    public int GeodeRobot_Obsidian;

    public int Total_ObsidianRobot_Ore;
    public int Total_GeodeRobot_Ore;

    public Blueprint(string s)
    {
        string[] split = s.Split(' ');

        ID = int.Parse(split[1].Substring(0, split[1].Length - 1));
        OreRobot_Ore = int.Parse(split[6]);
        ClayRobot_Ore = int.Parse(split[12]);
        ObsidianRobot_Ore = int.Parse(split[18]);
        ObsidianRobot_Clay = int.Parse(split[21]);
        GeodeRobot_Ore = int.Parse(split[27]);
        GeodeRobot_Obsidian = int.Parse(split[30]);

        Total_ObsidianRobot_Ore = ObsidianRobot_Ore + ObsidianRobot_Clay * ClayRobot_Ore;
        Total_GeodeRobot_Ore = GeodeRobot_Ore + GeodeRobot_Obsidian * Total_ObsidianRobot_Ore;
    }
}