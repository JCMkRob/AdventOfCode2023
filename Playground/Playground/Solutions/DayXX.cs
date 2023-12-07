using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

/// <summary>
/// Generic version of the class that I can use to build new DayXX.cs files from.
/// Just wanted a quick way to be able to create a 'blank' day with the boiler plate I want, faster than copy-pasting each time.
/// </summary>
public static class DayXX
{
    [Example(solver: nameof(PartOne), solution: -1)]
    public static readonly string PartOneExample = 
            """
            
            """;

    [Example(solver: nameof(PartTwo), solution: -1)]
    public static readonly string PartTwoExample = 
            """

            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        return 0;
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        return 0;
    }
}