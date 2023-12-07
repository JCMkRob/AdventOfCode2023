using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day08
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

    // TODO: double
    
    [Example<double>(solver: nameof(SomeDumbFunction), solution: 30)]
    public static double[] d = [ 1, 2, 3];
    public static double SomeDumbFunction(double[] input)
    {
        return 0;
    }
}
