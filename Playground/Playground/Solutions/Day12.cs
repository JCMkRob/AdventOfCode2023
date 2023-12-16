using System.Collections;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day12
{
    private class DoubleContainer(double Value)
    {
        public double Value { get; } = Value;
    }

    private enum SpringState
    {
        Operational,
        Damaged,
        Unknown 
    }

    private class SpringRow(string s)
    {
        private Queue<SpringState> Springs { get; init; } = new Queue<SpringState>(s.Select(c => FromChar(c)));

        private static SpringState FromChar(char c) => c switch
        {
            '.' => SpringState.Operational,
            '#' => SpringState.Damaged,
            '?' => SpringState.Unknown,
            _ => throw new Exception(),
        };

        public bool Empty() => Springs.Count != 0;
        public SpringState Next() => Springs.Dequeue();
        public SpringState Peek() => Springs.Peek();
    }

    [Example(solver: nameof(PartOne), solution: 1)]
    public static readonly string PartOneExampleOne = 
        """
        ???.### 1,1,3
        """;

    [Example(solver: nameof(PartOne), solution: 4)]

    public static readonly string PartOneExampleTwo = 
        """
        .??..??...?##. 1,1,3
        """;
    [Example(solver: nameof(PartOne), solution: 1)]

    public static readonly string PartOneExampleThree = 
        """
        ?#?#?#?#?#?#?#? 1,3,1,6
        """;

    [Example(solver: nameof(PartOne), solution: 1)]
    public static readonly string PartOneExampleFour = 
        """
        ????.#...#... 4,1,1
        """;

    [Example(solver: nameof(PartOne), solution: 4)]
    public static readonly string PartOneExampleFive = 
        """
        ????.######..#####. 1,6,5
        """;

    [Example(solver: nameof(PartOne), solution: 10)]
    public static readonly string PartOneExampleSix = 
        """
        ?###???????? 3,2,1
        """;

    [Example(solver: nameof(PartOne), solution: 21)]
    public static readonly string PartOneExampleSeven = 
        """
        ???.### 1,1,3
        .??..??...?##. 1,1,3
        ?#?#?#?#?#?#?#? 1,3,1,6
        ????.#...#... 4,1,1
        ????.######..#####. 1,6,5
        ?###???????? 3,2,1
        """;
            
    [Example(solver: nameof(PartTwo), solution: 1)]
    public static readonly string PartTwoExampleOne = 
        """
        ???.### 1,1,3
        """;

    [Example(solver: nameof(PartTwo), solution: 16384)]
    public static readonly string PartTwoExampleTwo = 
        """
        .??..??...?##. 1,1,3
        """;

    [Example(solver: nameof(PartTwo), solution: 1)]
    public static readonly string PartTwoExampleThree = 
        """
        ?#?#?#?#?#?#?#? 1,3,1,6
        """;

    [Example(solver: nameof(PartTwo), solution: 16)]
    public static readonly string PartTwoExampleFour = 
        """
        ????.#...#... 4,1,1
        """;

    [Example(solver: nameof(PartTwo), solution: 2500)]
    public static readonly string PartTwoExampleFive = 
        """
        ????.######..#####. 1,6,5
        """;

    [Example(solver: nameof(PartTwo), solution: 506250)]
    public static readonly string PartTwoExampleSix = 
        """
        ?###???????? 3,2,1
        """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        var mappedInput = input.Select(s => s.Split(' ')).Select(a => (pattern: a.First(), checksum: a.Last().Split(',').Select(int.Parse).ToArray()));

        double count = 0;

        foreach(var (pattern, checksum) in mappedInput)
        {
            var options = RecursiveOptions(pattern, checksum);

            foreach(var option in options)
            {
                if (Validate(option, checksum))
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input) => PartTwoSolver(input);
    public static double PartTwoSolver(IEnumerable<string> input, int unfoldTimes = 5)
    {
        var mappedInput = input.Select(s => s.Split(' ')).Select(a => (pattern: a.First(), checksum: a.Last().Split(',').Select(int.Parse).ToArray()));

        double totalWork = input.Count();
        double completedWork = 0;

        double count = 0;

        DateTime startTime = DateTime.Now;

        foreach(var (pattern, checksum) in mappedInput)
        {
            Console.WriteLine($"Progress: {completedWork++/totalWork:P1}");
            Console.WriteLine($"Starting: {pattern} {string.Join(',', checksum)}");

            double firstCount = 0;
            double secondCount = 0;

            var foldingPattern = pattern;
            var foldingChecksum = checksum;

            var options = RecursiveOptions(foldingPattern, checksum);

            foreach(var option in options)
            {
                if (Validate(option, checksum))
                {
                    firstCount++;
                }
            }

            foldingPattern += '?' + pattern;
            foldingChecksum = [.. foldingChecksum, .. checksum];
            
            options = RecursiveOptions(foldingPattern, foldingChecksum).ToArray();

            foreach(var option in options)
            {
                if (Validate(option, foldingChecksum))
                {
                    secondCount++;
                }
            }

            var powerBase = secondCount / firstCount;
            var answer = firstCount * Math.Pow(powerBase, unfoldTimes - 1);

            count += answer;
        }

        DateTime currentTime = DateTime.Now;
        TimeSpan executionTime = currentTime - startTime;

        Console.WriteLine($"Execution Time: {executionTime.Minutes} minutes, {executionTime.Seconds} seconds.");
        return count;
    }

    private static IEnumerable<int> MatchIndexes(string s, int gearSpanWidth)
    {
        int length = s.Length;

        int unknownCount = 0;
        int brokenCount = 0;

        for (int index = 0; index < length; index++)
        {
            if (gearSpanWidth <= index)
            {
                if ((unknownCount + brokenCount) == gearSpanWidth) 
                {
                    yield return index - gearSpanWidth;
                }

                char previous = s[index - gearSpanWidth];

                if (previous == '?') unknownCount--;
                if (previous == '#') brokenCount--;
            }

            char current = s[index];

            if (current == '?') unknownCount++;
            if (current == '#') brokenCount++;
        }

        if ((unknownCount + brokenCount) == gearSpanWidth) 
        {
            yield return length - gearSpanWidth;
        }
    }

    private static IEnumerable<string> RecursiveOptions(string pattern, int[] brokenGears)
    {
        var gear = brokenGears[0];
        var options = MatchIndexes(pattern, gear).ToArray();
        
        foreach(var index in options)
        {
            var current = pattern[ .. index] + new string('#', gear);
            var next = pattern[(index + gear) ..];

            if (brokenGears.Length == 1)
            {
                yield return current + next;
            }
            else 
            {
                foreach(var recursive in RecursiveOptions(next, brokenGears[1..]))
                {
                    yield return current + recursive;
                }
            }
        }
    }

    private static bool Validate(string option, int[] knownBrokenGearCounts)
    {
        int count = 0;
        int index = 0;
        int length = knownBrokenGearCounts.Length;

        int[] unknownBrokenGearCounts = new int[length];

        bool Set()
        {
            if (0 < count)
            {
                if (length == index) 
                {
                    return false;
                }

                unknownBrokenGearCounts[index++] = count;
                count = 0;
            }

            return true;
        }

        for(int i = 0; i < option.Length; i++)
        {
            if (option[i] == '#')
            {
                count++;
            }
            else 
            {
                if (!Set()) return false;
            }
        }

        if (!Set()) return false;

        return Enumerable.SequenceEqual(unknownBrokenGearCounts, knownBrokenGearCounts);
    }
}
