using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day11
{
    private record Point(double X, double Y);
    private record Pair(Point Left, Point Right);

    [Example(solver: nameof(PartOne), solution: 374)]
    public static readonly string PartOneExample = 
            """
            ...#......
            .......#..
            #.........
            ..........
            ......#...
            .#........
            .........#
            ..........
            .......#..
            #...#.....
            """;

    public static double PartTwoHelperOne(IEnumerable<string> s) => Solver(s, 10);

    [Example(solver: nameof(PartTwoHelperOne), solution: 1030)]
    public static readonly string PartTwoExampleOne = 
            """
            ...#......
            .......#..
            #.........
            ..........
            ......#...
            .#........
            .........#
            ..........
            .......#..
            #...#.....
            """;

    public static double PartTwoHelperTwo(IEnumerable<string> s) => Solver(s, 100);

    [Example(solver: nameof(PartTwoHelperTwo), solution: 8410)]
    public static readonly string PartTwoExampleTwo = 
            """
            ...#......
            .......#..
            #.........
            ..........
            ......#...
            .#........
            .........#
            ..........
            .......#..
            #...#.....
            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input) => Solver(input);

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input) => Solver(input, 1000000);

    private static double Solver(IEnumerable<string> input, double expansionAmount = 1)
    {
        Queue<Point> expandedGalaxies = ParseExpandedGalaxies(input, expansionAmount: expansionAmount);

        return DistanceBetweenExpandedGalaxies(expandedGalaxies);
    }

    private static Queue<Point> ParseExpandedGalaxies(IEnumerable<string> input, double expansionAmount = 1)
    {
        string[] image = input.ToArray();

        int xWidth = input.All(p => p.Length == input.First().Length) ? input.First().Length : throw new Exception();
        int yWidth = input.Count();

        HashSet<int> emptyColumns = [];
        HashSet<int> emptyRows = [];

        List<Point> galaxies = [];

        for(int x = 0; x < xWidth; x++)
        {
            emptyColumns.Add(x);
        }

        for (int y = 0; y < yWidth; y++)
        {
            bool isEmptyRow = true;

            for(int x = 0; x < xWidth; x++)
            {
                var c = image[y][x];
                
                if (c == '#')
                {
                    galaxies.Add(new Point(x, y));
                    isEmptyRow = false;

                    emptyColumns.Remove(x);
                }
            }

            if (isEmptyRow)
            {
                emptyRows.Add(y);
            }
        }

        Queue<Point> expandedGalaxies = [];

        foreach(var galaxy in galaxies)
        {
            expandedGalaxies.Enqueue(new Point
            (
                galaxy.X + (expansionAmount - 1) * emptyColumns.Where(v => v < galaxy.X).Count(),
                galaxy.Y + (expansionAmount - 1) * emptyRows.Where(v => v < galaxy.Y).Count()
            ));
        }

        return expandedGalaxies;
    }

    private static double DistanceBetweenExpandedGalaxies(Queue<Point> expandedGalaxies)
    {
        List<Pair> galaxyPairs = [];

        while(0 < expandedGalaxies.Count)
        {
            var first = expandedGalaxies.Dequeue();

            foreach(var second in expandedGalaxies)
            {
                galaxyPairs.Add(new Pair(first, second));
            }
        }

        double steps = 0;

        foreach(var pair in galaxyPairs)
        {
            steps += Math.Abs(pair.Left.X - pair.Right.X) + Math.Abs(pair.Left.Y - pair.Right.Y);
        }

        return steps;
    }
}
