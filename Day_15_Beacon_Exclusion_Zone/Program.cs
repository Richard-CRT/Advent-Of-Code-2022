// 05:00
using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoCUtilities.GetInputLines();
List<Sensor> Sensors = inputList.Select(x => new Sensor(x)).ToList();

void P1()
{
    HashSet<int> impossibleXCoordinatesOnRow = new HashSet<int>();
    int row = 2000000;
    foreach (Sensor sensor in Sensors)
    {
        int val = Math.Abs(sensor.Y - row);
        if (val <= sensor.DistanceToDetectedBeacon)
        {
            // row `row` is within range of this particular sensor
            int extensionOfDiamondOnRow = sensor.DistanceToDetectedBeacon - val;
            for (int x = sensor.X - extensionOfDiamondOnRow; x <= sensor.X + extensionOfDiamondOnRow; x++)
            {
                if (sensor.DetectedBeacon.X != x || sensor.DetectedBeacon.Y != row)
                    impossibleXCoordinatesOnRow.Add(x);
            }
        }
    }
    Console.WriteLine(impossibleXCoordinatesOnRow.Count);
    Console.ReadLine();
}

void P2()
{
    int dim = 4000000 + 1;
    Int64 tuningFrequency = -1;
    for (int y = 0; y < dim; y++)
    {
        List<(int, int)> ranges = new List<(int, int)>();
        foreach (Sensor sensor in Sensors)
        {
            int val = Math.Abs(sensor.Y - y);
            if (val <= sensor.DistanceToDetectedBeacon)
            {
                int extensionOfDiamondOnRow = sensor.DistanceToDetectedBeacon - val;
                int minX = Math.Max(sensor.X - extensionOfDiamondOnRow, 0);
                int maxX = Math.Min(sensor.X + extensionOfDiamondOnRow, dim - 1);

                bool add = true;
                for (int i = ranges.Count - 1; i >= 0; i--)
                {
                    (int rangeMinX, int rangeMaxX) = ranges[i];
                    if (minX >= rangeMinX && maxX <= rangeMaxX)
                    {
                        add = false;
                        break;
                    }
                    if (rangeMaxX >= minX - 1 && maxX >= rangeMinX - 1)
                    {
                        minX = Math.Min(rangeMinX, minX);
                        maxX = Math.Max(rangeMaxX, maxX);
                        ranges.RemoveAt(i);
                    }
                }
                if (add)
                    ranges.Add((minX, maxX));
            }
        }
        if (ranges.Count == 2)
        {
            int x = ranges[0].Item2 + 1;
            tuningFrequency = x * (Int64)4000000 + y;
        }
        else if (ranges.Count == 1)
        {
            if (ranges[0].Item1 == 1)
            {
                int x = 0;
                tuningFrequency = x * (Int64)4000000 + y;
            }
            else if (ranges[0].Item2 == dim - 2)
            {
                int x = dim - 1;
                tuningFrequency = x * (Int64)4000000 + y;
            }

        }
        if (tuningFrequency != -1)
            break;
    }
    Console.WriteLine(tuningFrequency);
    Console.ReadLine();
}

P1();
P2();

class Sensor
{


    public int X;
    public int Y;
    public Beacon DetectedBeacon;
    private int? distanceToDetectedBeacon;
    public int DistanceToDetectedBeacon
    {
        get
        {
            if (distanceToDetectedBeacon == null)
                distanceToDetectedBeacon = Math.Abs(DetectedBeacon.X - X) + Math.Abs(DetectedBeacon.Y - Y);
            return (int)distanceToDetectedBeacon;
        }
    }

    public Sensor(string s)
    {
        string[] split = s.Split(' ');
        X = int.Parse(split[2].Substring(2, split[2].Length - 3));
        Y = int.Parse(split[3].Substring(2, split[3].Length - 3));

        int beaconX = int.Parse(split[8].Substring(2, split[8].Length - 3));
        int beaconY = int.Parse(split[9].Substring(2));
        if (Beacon.AllBeacons.ContainsKey((beaconX, beaconY)))
            Beacon.AllBeacons[(beaconX, beaconY)].DetectedBy.Add(this);
        else
            Beacon.AllBeacons[(beaconX, beaconY)] = new Beacon(this, beaconX, beaconY);
        DetectedBeacon = Beacon.AllBeacons[(beaconX, beaconY)];
    }

    public override string ToString()
    {
        return $"Sensor {X}, {Y} Detected Beacon [{DetectedBeacon}]";
    }
}

class Beacon
{
    public static Dictionary<(int, int), Beacon> AllBeacons = new Dictionary<(int, int), Beacon>();

    public int X;
    public int Y;
    public List<Sensor> DetectedBy = new List<Sensor>();

    public Beacon(Sensor detectedBy, int x, int y)
    {
        X = x;
        Y = y;
        DetectedBy.Add(detectedBy);
    }

    public override string ToString()
    {
        return $"Beacon {X}, {Y} Detected by {DetectedBy.Count}";
    }
}