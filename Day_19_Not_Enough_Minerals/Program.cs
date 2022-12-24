using AdventOfCodeUtilities;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

List<string> inputList = AoC.GetInputLines();
Blueprint[] Blueprints = inputList.Select(x => new Blueprint(x)).ToArray();

int recurse(ref int MaxGeodesOverall, Dictionary<(int, string), int> Cache, Blueprint blueprint, int maxMinutes, int minute, Stats stats)
{
    (int, string) cacheKey = (minute, stats.Key());
    if (Cache.ContainsKey(cacheKey))
        return Cache[cacheKey];

    int ret = int.MinValue;
    if (minute < maxMinutes)
    {
        // how many geodes would we get if we make a geode robot every remaining minute
        // prune if we can't possibly beat the currently found maximum
        int remainingMinutes = maxMinutes - minute;
        int maximumAdditionalGeodeRobots = remainingMinutes;
        int n = (maximumAdditionalGeodeRobots - 1) + stats.GeodeRobots;
        int maximumAdditionalGeodes = ((n * n) + n) / 2;
        if (stats.Geodes + maximumAdditionalGeodes < MaxGeodesOverall)
        {
            // Not possible to make enough geodes to beat our current maximum
            return 0;
        }

        Stats prevStats = stats.Copy();

        stats.Ore += stats.OreRobots;
        stats.Clay += stats.ClayRobots;
        stats.Obsidian += stats.ObsidianRobots;
        stats.Geodes += stats.GeodeRobots;

        // If we can make a geode robot, do it - it will always be beneficial to make geode robots earlier
        // Additionally, we should never build any robot where we would be producing surplus e.g. no point in having more
        //   Obsidian robots than the obsidian cost of a Geode robot, and no point in having more Ore robots than the
        //   maximum ore cost of any of the robots, as we'll get n per turn, and consume a maximum of n per turn
        if (prevStats.Ore >= blueprint.GeodeRobot_Ore && prevStats.Obsidian >= blueprint.GeodeRobot_Obsidian)
        {
            Stats newStats = stats.Copy();
            newStats.Ore -= blueprint.GeodeRobot_Ore;
            newStats.Obsidian -= blueprint.GeodeRobot_Obsidian;
            newStats.GeodeRobots++;
            ret = Math.Max(ret, recurse(ref MaxGeodesOverall, Cache, blueprint, maxMinutes, minute + 1, newStats));
        }
        else
        {
            bool canAffordAll = true;
            if (prevStats.Ore >= blueprint.ObsidianRobot_Ore && prevStats.Clay >= blueprint.ObsidianRobot_Clay && prevStats.ObsidianRobots < blueprint.GeodeRobot_Obsidian)
            {
                Stats newStats = stats.Copy();
                newStats.Ore -= blueprint.ObsidianRobot_Ore;
                newStats.Clay -= blueprint.ObsidianRobot_Clay;
                newStats.ObsidianRobots++;
                ret = Math.Max(ret, recurse(ref MaxGeodesOverall, Cache, blueprint, maxMinutes, minute + 1, newStats));
            }
            else
                canAffordAll = false;
            if (prevStats.Ore >= blueprint.ClayRobot_Ore && prevStats.ClayRobots < blueprint.ObsidianRobot_Clay)
            {
                Stats newStats = stats.Copy();
                newStats.Ore -= blueprint.ClayRobot_Ore;
                newStats.ClayRobots++;
                ret = Math.Max(ret, recurse(ref MaxGeodesOverall, Cache, blueprint, maxMinutes, minute + 1, newStats));
            }
            else
                canAffordAll = false;
            if (prevStats.Ore >= blueprint.OreRobot_Ore && prevStats.OreRobots < blueprint.MaxRobot_Ore)
            {
                Stats newStats = stats.Copy();
                newStats.Ore -= blueprint.OreRobot_Ore;
                newStats.OreRobots++;
                ret = Math.Max(ret, recurse(ref MaxGeodesOverall, Cache, blueprint, maxMinutes, minute + 1, newStats));
            }
            else
                canAffordAll = false;
            // Make sure to allow for saving resources to build more expensive robots, but no point in doing this
            //   if we can afford all of the robots, as it's just a waste of CPU at the point
            if (!canAffordAll)
                ret = Math.Max(ret, recurse(ref MaxGeodesOverall, Cache, blueprint, maxMinutes, minute + 1, stats.Copy()));
        }
    }
    else
    {
        ret = stats.Geodes;
        if (ret > MaxGeodesOverall)
            MaxGeodesOverall = ret;
    }
    Cache[cacheKey] = ret;
    return ret;
}

void P1()
{
    int sum = 0;
    int complete = 0;
    Parallel.ForEach(Blueprints, b =>
    {
        int MaxGeodesOverall = 0;
        int max = recurse(ref MaxGeodesOverall, new Dictionary<(int, string), int>(), b, 24, 0, new Stats(0, 0, 0, 0, 1, 0, 0, 0));
        sum += b.ID * max;
        complete++;
        Console.WriteLine($"{((100.0 * complete) / Blueprints.Count()):0.##}%");
    });
    Console.WriteLine(sum);
    Console.ReadLine();
}

void P2()
{
    int product = 1;
    int complete = 0;
    Parallel.ForEach(Blueprints.Take(3), b =>
    {
        int MaxGeodesOverall = 0;
        int max = recurse(ref MaxGeodesOverall, new Dictionary<(int, string), int>(), b, 32, 0, new Stats(0, 0, 0, 0, 1, 0, 0, 0));
        product *= max;
        complete++;
        Console.WriteLine($"{((100.0 * complete) / 3):0.##}%");
    });
    Console.WriteLine(product);
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

    public int MaxRobot_Ore;

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

        MaxRobot_Ore = (new int[] { OreRobot_Ore, ClayRobot_Ore, ObsidianRobot_Ore, GeodeRobot_Ore }).Max();

        Total_ObsidianRobot_Ore = ObsidianRobot_Ore + ObsidianRobot_Clay * ClayRobot_Ore;
        Total_GeodeRobot_Ore = GeodeRobot_Ore + GeodeRobot_Obsidian * Total_ObsidianRobot_Ore;
    }
}