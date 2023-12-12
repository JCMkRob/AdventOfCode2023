using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day09
{
    [Example(solver: nameof(PartOne), solution: 18)]
    public static readonly string PartOneExampleOne = 
            """
            0 3 6 9 12 15
            """;

    [Example(solver: nameof(PartOne), solution: 28)]
    public static readonly string PartOneExampleTwo = 
            """
            1 3 6 10 15 21
            """;

    [Example(solver: nameof(PartOne), solution: 68)]
    public static readonly string PartOneExampleThree = 
            """
            10 13 16 21 30 45
            """;    

    [Example(solver: nameof(PartOne), solution: 114)]
    public static readonly string PartOneExampleFour = 
            """
            0 3 6 9 12 15
            1 3 6 10 15 21
            10 13 16 21 30 45
            """;   

    [Example(solver: nameof(PartTwo), solution: -3)]
    public static readonly string PartTwoExampleOne = 
            """
            0 3 6 9 12 15
            """;

    [Example(solver: nameof(PartTwo), solution: 0)]
    public static readonly string PartTwoExampleTwo = 
            """
            1 3 6 10 15 21
            """;

    [Example(solver: nameof(PartTwo), solution: 5)]
    public static readonly string PartTwoExampleThree = 
            """
            10 13 16 21 30 45
            """;    

    [Example(solver: nameof(PartTwo), solution: 2)]
    public static readonly string PartTwoExampleFour = 
            """
            0 3 6 9 12 15
            1 3 6 10 15 21
            10 13 16 21 30 45
            """; 

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        return input.Select(Forcast).Sum();
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        return input.Select(Precast).Sum();
    }

    public static double Forcast(string s)
    {
        double[] current = s.Split(' ').Where(c => !string.IsNullOrEmpty(c)).Select(double.Parse).ToArray();
        
        List<double> ends = [];

        for(int depth = 0; depth < s.Length - 1; depth++)
        {
            ends.Add(current.Last());

            current = current.Zip(current.Skip(1), (a, b) => b - a).ToArray();
            
            if (current.All(d => d == 0)) break;
        }

        return ends.Sum();
    }

    public static double Precast(string s)
    {
        double[] current = s.Split(' ').Where(c => !string.IsNullOrEmpty(c)).Select(double.Parse).ToArray();
        
        List<double> starts = [];

        for(int depth = 0; depth < s.Length - 1; depth++)
        {
            starts.Add(current.First());

            current = current.Zip(current.Skip(1), (a, b) => b - a).ToArray();
            
            if (current.All(d => d == 0)) break;
        }

        starts.Reverse();

        return starts.Aggregate((a, b) => b - a);
    }
}
