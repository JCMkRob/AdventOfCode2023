using AdventOfCode.Support;
using Microsoft.VisualBasic;

namespace AdventOfCode.Solutions;

public static class Day13
{
    [Example(solver: nameof(PartOne), solution: 5)]
    public static readonly string PartOneExampleOne = 
            """
            #.##..##.
            ..#.##.#.
            ##......#
            ##......#
            ..#.##.#.
            ..##..##.
            #.#.##.#.
            """;

    [Example(solver: nameof(PartOne), solution: 400)]
    public static readonly string PartOneExampleTwo = 
            """
            #...##..#
            #....#..#
            ..##..###
            #####.##.
            #####.##.
            ..##..###
            #....#..#
            """;

    [Example(solver: nameof(PartTwo), solution: 400)]
    public static readonly string PartTwoExample = 
            """
            #.##..##.
            ..#.##.#.
            ##......#
            ##......#
            ..#.##.#.
            ..##..##.
            #.#.##.#.

            #...##..#
            #....#..#
            ..##..###
            #####.##.
            #####.##.
            ..##..###
            #....#..#
            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        var score = 0;
        List<string> lines = [];

        foreach(var line in input)
        {
            if (!string.IsNullOrEmpty(line))
            {
                lines.Add(line);
            }
            else 
            {
                score += ScoreMap([.. lines]);
                lines.Clear();
            }
        }

        if (0 < lines.Count)
        {
            score += ScoreMap([.. lines]);
            lines.Clear();
        }

        return score;
        // columns to the left of the LoR + 100 * rows above the LoR
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        List<string> lines = [];

        foreach(var line in input)
        {
            if (!string.IsNullOrEmpty(line))
            {
                lines.Add(line);
            }
            else 
            {
                Console.WriteLine(SmudgeScoreMap([.. lines]));
                lines.Clear();
            }
        }

        if (0 < lines.Count)
        {
            Console.WriteLine(SmudgeScoreMap([.. lines]));
            lines.Clear();
        }

        return 0;
    }

    private static int ScoreMap(string[] map)
    {
        var score = 0;

        if (!map.TryGetReflection(out var invertedMap))
        {
            return 0;
        }

        var verticalLine = LineOfSymmetry(map);
        var horizontalLine = LineOfSymmetry(invertedMap);

        if (horizontalLine.width < verticalLine.width)
        {
            score += verticalLine.index;
        }

        if (verticalLine.width < horizontalLine.width)
        {
            score += 100 * horizontalLine.index;
        }

        return score;
    }

    private static double SmudgeScoreMap(string[] map)
    {

        if (!map.TryGetReflection(out var invertedMap))
        {
            return 0;
        }

        var verticalLine = CXXXXX(map);
        var horizontalLine = CXXXXX(invertedMap);

        return Math.Min(verticalLine, horizontalLine);
    }

    private static (int index, int width) LineOfSymmetry(string[] map)
    {
        var firstLine = map[0];
        var remainingLines = map[1..];

        var linesOfSymmetry = FindLinesOfSymmetry(firstLine);

        foreach(var remaining in remainingLines)
        {
            ValidateLinesOfSymetry(remaining, linesOfSymmetry);
        }

        if (0 == linesOfSymmetry.Count)
        {
            return (0, 0);
        }
        else
        {
            return linesOfSymmetry.ToList().OrderBy(LoS => LoS.width).ThenBy(LoS => LoS.index).Last();
        }    
    }

    // index is the value AFTER the line, to the right or below the line.
    private static HashSet<(int index, int width)> FindLinesOfSymmetry(string mapLine)
    {
        HashSet<(int index, int width)> linesOfSymmetry = [];
        Queue<int> seeds = [];

        for(int index = 0; index < (mapLine.Length - 1); index++)
        {
            if (mapLine[index] == mapLine[index + 1])
            {
                seeds.Enqueue(index + 1);
            }
        }

        // Can refactor while loops to for loops with bounds.
        while(0 < seeds.Count)
        {
            var index = seeds.Dequeue();
            var width = 0;

            while (true)
            {
                width++;

                var left = index - width;
                var right = index + width - 1;

                if ((left < 0) || (right == mapLine.Length) || 
                    (mapLine[left] != mapLine[right]))
                {
                    // Gone too far, back track one and break;
                    width--;
                    break;
                }
            }

            linesOfSymmetry.Add((index, width));
        }

        return linesOfSymmetry;
    }

    private static void ValidateLinesOfSymetry(string mapLine, HashSet<(int index, int width)> linesOfSymmetry)
    {
        foreach(var line in linesOfSymmetry.ToArray())
        {
            for(int offset = 1; offset <= line.width; offset++)
            {
                var left = line.index - offset;
                var right = line.index + offset - 1;

                if (mapLine[left] != mapLine[right])
                {
                    linesOfSymmetry.Remove(line);
                    break;
                }
            }
        }
    }

    private static double CXXXXX(string[] map)
    {
        List<(int index, int width)> allLinesOfsymmetry = [];
        
        foreach(var line in map)
        {
            allLinesOfsymmetry.AddRange([.. FindLinesOfSymmetry(line)]);
        }

        var g = allLinesOfsymmetry.GroupBy(x => x).Select(kv => (kv.Key, kv.Count()));

        return map.Length - allLinesOfsymmetry.GroupBy(x => x).Select(kv => kv.Count()).Order().Last();
    }
}
