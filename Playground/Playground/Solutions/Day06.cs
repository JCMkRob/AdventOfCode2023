using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day06
{
    private record RaceRecord(double Time, double Distance);


    [Example(solver: nameof(PartOne), solution: 288)]
    public static readonly string PartOneExample = 
            """
            Time:      7  15   30
            Distance:  9  40  200
            """;

    [Example(solver: nameof(PartTwo), solution: 71503)]
    public static readonly string PartTwoExample = 
            """
            Time:      7  15   30
            Distance:  9  40  200
            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        var records = ParseInputPartOne(input);

        return records.Select(WaysToBeatTheRecord).Aggregate((a , b) => a * b);
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        var records = ParseInputPartTwo(input);

        return records.Select(WaysToBeatTheRecord).Aggregate((a , b) => a * b);
    }

    private static double WaysToBeatTheRecord(RaceRecord r)
    {
        double negativeRoot = (r.Time - Math.Sqrt(Math.Pow(r.Time, 2) - 4 * r.Distance)) / 2;
        double positiveRoot = (r.Time + Math.Sqrt(Math.Pow(r.Time, 2) - 4 * r.Distance)) / 2;

        double upperRoot = UpperRoot(r, positiveRoot);
        double lowerRoot = LowerRoot(r, negativeRoot);

        return upperRoot - lowerRoot + 1;
    }

    private static double LowerRoot(RaceRecord r, double button)
    {
        double floor = double.Floor(button);
        double ceiling = double.Ceiling(button);

        if (floor == ceiling) ceiling++;
        
        bool lowerRootValid = CheckRoot(r, floor);
        bool upperRootValid = CheckRoot(r, ceiling);

        if (lowerRootValid) return floor;
        if (upperRootValid) return ceiling;
        
        throw new Exception();
    }

    private static double UpperRoot(RaceRecord r, double button)
    {
        double floor = double.Floor(button);
        double ceiling = double.Ceiling(button);

        if (floor == ceiling) floor--;

        bool lowerRootValid = CheckRoot(r, floor);
        bool upperRootValid = CheckRoot(r, ceiling);

        if (upperRootValid) return ceiling;
        if (lowerRootValid) return floor;

        throw new Exception();
    }

    private static bool CheckRoot(RaceRecord r, double button) => 0 > (Math.Pow(button, 2) - (button * r.Time) + r.Distance);
  
    private static IEnumerable<RaceRecord> ParseInputPartOne(IEnumerable<string> input)
    {
        var raceRecords = input.ToArray();

        var time = raceRecords[0].Split(':')[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(double.Parse).ToArray();
        var dist = raceRecords[1].Split(':')[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(double.Parse).ToArray();

        for(int index = 0; index < time.Length; index++)
        {
            yield return new(Time: time[index], Distance: dist[index]);
        }
    } 

    private static IEnumerable<RaceRecord> ParseInputPartTwo(IEnumerable<string> input)
    {
        var raceRecords = input.ToArray();

        var time = raceRecords[0].Split(':')[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).Aggregate((a, b) => a + b);
        var dist = raceRecords[1].Split(':')[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).Aggregate((a, b) => a + b);

        
        yield return new(Time: double.Parse(time), Distance: double.Parse(dist));
    } 
}