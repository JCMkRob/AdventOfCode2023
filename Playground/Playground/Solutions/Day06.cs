using System.CodeDom.Compiler;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day06
{
    public static double PartOneTest(string s) => PartOne(s.Split("\n").Select(s => s.TrimEnd()));

    [Example(solver: nameof(PartOneTest), solution: -1)]
    public static readonly string PartOneExample = 
            """

            """;

    public static double PartTwoTest(string s) => PartTwo(s.Split("\n").Select(s => s.TrimEnd()));

    [Example(solver: nameof(PartTwoTest), solution: -1)]
    public static readonly string PartTwoExample = 
            """

            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> strings)
    {
        return 0;
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> strings)
    {
        return 0;
    }
}