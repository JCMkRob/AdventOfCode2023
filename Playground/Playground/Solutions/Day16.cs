using System.Security.Cryptography;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day16
{
    private enum Heading 
    {
        Up,
        Down,
        Left,
        Right
    }

    private record Point(int X, int Y)
    {
        public Point North() => new(X, Y + 1);
        public Point East()  => new(X + 1, Y);
        public Point South() => new(X, Y - 1);
        public Point West()  => new(X - 1, Y);
    }


    [Example(solver: nameof(PartOne), solution: 46)]
    public static readonly string PartOneExample = 
        """
        .|...\....
        |.-.\.....
        .....|-...
        ........|.
        ..........
        .........\
        ..../.\\..
        .-.-/..|..
        .|....-|.\
        ..//.|....
        """;

    [Example(solver: nameof(PartTwo), solution: 51)]
    public static readonly string PartTwoExample = 
        """
        .|...\....
        |.-.\.....
        .....|-...
        ........|.
        ..........
        .........\
        ..../.\\..
        .-.-/..|..
        .|....-|.\
        ..//.|....
        """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        var inputArray = input.Reverse().ToArray();

        return Traverse(inputArray, (new (0, inputArray.Length - 1), Heading.Right));
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        var inputArray = input.Reverse().ToArray();

        int yWidth = inputArray.Length;
        int xWidth = inputArray[0].Length;

        HashSet<(Point point, Heading heading)> beamStarts = [];

        for(int x = 0; x < xWidth; x++)
        {
            beamStarts.Add((new (x, 0), Heading.Up));
            beamStarts.Add((new (x, yWidth - 1), Heading.Down));
        }

        for(int y = 0; y < yWidth; y++)
        {
            beamStarts.Add((new (0, y), Heading.Right));
            beamStarts.Add((new (xWidth - 1, y), Heading.Left));
        }

        double maxEnergized = 0;

        foreach(var beamStart in beamStarts)
        {   
            var energized = Traverse(inputArray, beamStart);

            maxEnergized = Math.Max(maxEnergized, energized);
        }

        return maxEnergized;
    }

    private static double Traverse(string[] s, (Point point, Heading heading) beamStart)
    {
        var inputArray = s;

        int yWidth = inputArray.Length;
        int xWidth = inputArray[0].Length;

        bool validPoint(Point point) => (0 <= point.X) && (point.X < xWidth) && (0 <= point.Y) && (point.Y < yWidth);

        HashSet<(Point point, Heading heading)> alreadySeen = [];
        Queue<(Point point, Heading heading)> beamProgress = [];

        beamProgress.Enqueue(beamStart);

        while(0 < beamProgress.Count)
        {
            (var point, var heading) = beamProgress.Dequeue();

            if (!validPoint(point)) continue;
            if (alreadySeen.Contains((point, heading))) continue;

            alreadySeen.Add((point, heading));

            var nextBeam = Proceed(heading, point, inputArray[point.Y][point.X]);

            foreach(var beam in nextBeam)
            {
                beamProgress.Enqueue(beam);
            }
        }

        return alreadySeen.Select(b => b.point).Distinct().Count();
    }

    private static IEnumerable<(Point, Heading)> Proceed(Heading heading, Point point, char c) => (heading, c) switch
    { 
        (Heading.Up, '|') => [(point.North(), Heading.Up)],
        (Heading.Down, '|') => [(point.South(), Heading.Down)],
        (Heading.Left, '|') => [(point.North(), Heading.Up), (point.South(), Heading.Down)],
        (Heading.Right, '|') => [(point.North(), Heading.Up), (point.South(), Heading.Down)],
        (Heading.Up, '-') => [(point.West(), Heading.Left), (point.East(), Heading.Right)],
        (Heading.Down, '-') => [(point.West(), Heading.Left), (point.East(), Heading.Right)],
        (Heading.Left, '-') => [(point.West(), Heading.Left)],
        (Heading.Right, '-') => [(point.East(), Heading.Right)],
        (Heading.Up, '\\') => [(point.West(), Heading.Left)],
        (Heading.Up, '/') => [(point.East(), Heading.Right)],
        (Heading.Down, '\\') => [(point.East(), Heading.Right)],
        (Heading.Down, '/') => [(point.West(), Heading.Left)],
        (Heading.Left, '\\') => [(point.North(), Heading.Up)],
        (Heading.Left, '/') => [(point.South(), Heading.Down)],
        (Heading.Right, '\\') => [(point.South(), Heading.Down)],
        (Heading.Right, '/') => [(point.North(), Heading.Up)],
        (Heading.Right, '.') => [(point.East(), Heading.Right)],
        (Heading.Left, '.') => [(point.West(), Heading.Left)],
        (Heading.Up, '.') => [(point.North(), Heading.Up)],
        (Heading.Down, '.') => [(point.South(), Heading.Down)],
        (_, _) => throw new Exception()
    }; 
}

