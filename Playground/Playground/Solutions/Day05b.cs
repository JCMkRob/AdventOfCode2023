using System.CodeDom.Compiler;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day05
{
    public static double PartOneTest(string s) => PartOne(s.Split("\n").Select(s => s.TrimEnd()));

    [Example(solver: nameof(PartOneTest), solution: 35)]
    public static readonly string PartOneExample = 
            """
            seeds: 79 14 55 13

            seed-to-soil map:
            50 98 2
            52 50 48

            soil-to-fertilizer map:
            0 15 37
            37 52 2
            39 0 15

            fertilizer-to-water map:
            49 53 8
            0 11 42
            42 0 7
            57 7 4

            water-to-light map:
            88 18 7
            18 25 70

            light-to-temperature map:
            45 77 23
            81 45 19
            68 64 13

            temperature-to-humidity map:
            0 69 1
            1 0 69

            humidity-to-location map:
            60 56 37
            56 93 4
            """;

    public static double PartTwoTest(string s) => PartTwo(s.Split("\n").Select(s => s.TrimEnd()));

    [Example(solver: nameof(PartTwoTest), solution: 46)]
    public static readonly string PartTwoExample = 
            """
            seeds: 79 14 55 13

            seed-to-soil map:
            50 98 2
            52 50 48

            soil-to-fertilizer map:
            0 15 37
            37 52 2
            39 0 15

            fertilizer-to-water map:
            49 53 8
            0 11 42
            42 0 7
            57 7 4

            water-to-light map:
            88 18 7
            18 25 70

            light-to-temperature map:
            45 77 23
            81 45 19
            68 64 13

            temperature-to-humidity map:
            0 69 1
            1 0 69

            humidity-to-location map:
            60 56 37
            56 93 4
            """;

    public record Interval(double Start, double End)
    {
        public bool TryGetIntersection(Interval with, out Interval intersection)
        {
            // https://scicomp.stackexchange.com/questions/26258/the-easiest-way-to-find-intersection-of-two-intervals

            intersection = new Interval(0, 0);

            (double s, double e) Ia = new (Start, End);
            (double s, double e) Ib = new (with.Start, with.End);


            if ((Ib.s > Ia.e) || (Ia.s > Ib.e)) 
            {
                return false;
            }
            else
            {
                intersection = new Interval(Math.Max(Ia.s, Ib.s), Math.Min(Ia.e, Ib.e));

                return true;
            }
        }

        public IEnumerable<Interval> Without(Interval interval)
        {
            if ((End < interval.Start) || (interval.End < Start))
            {
                // No intersection
                return [ new Interval(Start, End) ];
            }
            else if ((interval.Start <= Start) && (End <= interval.End))
            {
                // Full intersection
                return [];
            }
            else if ((interval.Start <= Start) && (interval.End < End))
            {
                // Left partial intersection
                return [ new Interval(interval.End + 1, End) ];
            }
            else if ((Start < interval.Start) && (End <= interval.End))
            {
                // Right partial intersection
                return [ new Interval(Start, interval.Start - 1) ];
            }
            else 
            {
                // Center intersection
                return [ new Interval(Start, interval.Start - 1), new Interval(interval.End + 1, End) ];
            }
        }

        public Interval Shift(double value) => new(Start + value, End + value);
    }

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        return input.SolveUsing(PartOneAddSeeds);
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        return input.SolveUsing(PartTwoAddSeeds);
    }

    private static double SolveUsing(this IEnumerable<string> strings, Action<string, List<Interval>> addSeeds)
    {
        List<Interval> current = [];
        List<Interval> next = [];

        foreach(var s in strings)
        {
            if (s.StartsWith("seeds:"))
            {
                // Starting string of seeds
                addSeeds(s[7..], next);
            }
            else if (s.EndsWith(':'))
            {
                // The remaining intervals were not mapped and are thus mapped 1:1,
                // Add the mapped intervals to current and clear the list.
                current.AddRange(next);
                next.Clear();
            }
            else if (!string.IsNullOrEmpty(s))
            {
                // Map slices forward
                var parsed = s.Split(' ').Select(double.Parse).ToArray();

                (double to, double from, double range) map = new (parsed[0], parsed[1], parsed[2]);

                Interval fromInterval = new(map.from, map.from + map.range - 1);

                foreach(var interval in current.ToArray())
                {
                    if (interval.TryGetIntersection(fromInterval, out var intersection))
                    {
                        // There is an intersection, shift the overlap forward ...
                        next.Add(intersection.Shift(map.to - map.from));

                        // Remove the interval ...
                        current.Remove(interval);

                        foreach(var remaining in interval.Without(intersection))
                        {
                            // re-add the remaining interval that wasn't shifted forward.
                            current.Add(remaining);
                        }
                    }
                }
            }
        }
        
        current.AddRange(next);
        next.Clear();

        return current.Select(i => i.Start).Min();
    }

    private static bool TryGetIntersection((double s, double e) Ia, (double s, double e) Ib, out (double s, double e) Io)
    {
        Io = (0, 0);

        if ((Ib.s > Ia.e) || (Ia.s > Ib.e)) 
        {
            return false;
        }
        else
        {
            Io = (Math.Max(Ia.s, Ib.s), Math.Max(Ia.s, Ib.s));

            return true;
        }
    }

    private static void PartOneAddSeeds(string s, List<Interval> list)
    {   
        foreach(double d in s.Trim().Split(' ').Select(double.Parse))
        {
            list.Add(new (d, d));
        }
    }
    private static void PartTwoAddSeeds(string s, List<Interval> list)
    {   
        double[] numbers = s.Trim().Split(' ').Select(double.Parse).ToArray();
        
        // TODO: Make a traversal that goes by group, and one that goes by a moving window. Think there is an existing function for moving window.
        for (int i = 0; i < numbers.Length - 1; i += 2)
        {
            list.Add(new (numbers[i], numbers[i] + numbers[i + 1] - 1));
        }
    }
}