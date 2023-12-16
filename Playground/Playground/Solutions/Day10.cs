using System.ComponentModel;
using System.Security.Cryptography;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day10
{
    private record Point(int X, int Y)
    {
        public Point North() => new(X, Y + 1);
        public Point East()  => new(X + 1, Y);
        public Point South() => new(X, Y - 1);
        public Point West()  => new(X - 1, Y);
    }

    private record Edge(Point Beginning, Point Middle, Point End)
    {
        public bool Enters(Point point) => (Beginning == point) || (End == point);
        public Point? Next(Point point)
        {
            if (Beginning == point) return End;
            if (End == point) return Beginning;

            return null;
        }
    }

    [Example(solver: nameof(PartOne), solution: 4)]
    public static readonly string PartOneExampleOne = 
        """
        .....
        .S-7.
        .|.|.
        .L-J.
        .....
        """;
    
    [Example(solver: nameof(PartOne), solution: 8)]
    public static readonly string PartOneExampleTwo = 
        """
        ..F7.
        .FJ|.
        SJ.L7
        |F--J
        LJ...
        """;

    [Example(solver: nameof(PartTwo), solution: 4)]
    public static readonly string PartTwoExampleOne = 
        """
        ...........
        .S-------7.
        .|F-----7|.
        .||.....||.
        .||.....||.
        .|L-7.F-J|.
        .|..|.|..|.
        .L--J.L--J.
        ...........
        """;

    [Example(solver: nameof(PartTwo), solution: 8)]
    public static readonly string PartTwoExampleTwo = 
        """
        .F----7F7F7F7F-7....
        .|F--7||||||||FJ....
        .||.FJ||||||||L7....
        FJL7L7LJLJ||LJ.L-7..
        L--J.L7...LJS7F-7L7.
        ....F-J..F7FJ|L7L7L7
        ....L7.F7||L7|.L7L7|
        .....|FJLJ|FJ|F7|.LJ
        ....FJL-7.||.||||...
        ....L---J.LJ.LJLJ...
        """;

    [Example(solver: nameof(PartTwo), solution: 10)]
    public static readonly string PartTwoExampleThree = 
        """
        FF7FSF7F7F7F7F7F---7
        L|LJ||||||||||||F--J
        FL-7LJLJ||||||LJL-77
        F--JF--7||LJLJ7F7FJ-
        L---JF-JLJ.||-FJLJJ7
        |F|F-JF---7F7-L7L|7|
        |FFJF7L7F-JF7|JL---7
        7-L-JL7||F7|L7F-7F7|
        L.L7LFJ|||||FJL7||LJ
        L7JLJL-JLJLJL--JLJ.L
        """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        var map = MapInput(input, out Point start);
        var traverse = TraverseMap(map, start); 

        return traverse.Values.Max();
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        var inputArray = input.Reverse().ToArray();

        int xWidth = input.All(p => p.Length == inputArray[0].Length) ? inputArray[0].Length : throw new Exception();
        int yWidth = input.Count();
    
        var map = MapInput(input, out Point start);
        var traverse = TraverseMap(map, start); 

        HashSet<Point> pipeLoop = [.. traverse.Keys, start ]; 

        HashSet<Point> expandedPipeLoop = [];

        foreach(var pipe in pipeLoop)
        {
            var pipeMap = ExpandPoint(new Point(pipe.X * 3, pipe.Y * 3), inputArray[pipe.Y][pipe.X]);

            foreach(var point in pipeMap)
            {
                expandedPipeLoop.Add(point);
            }
        }

        HashSet<Point> external = [];
        Queue<Point> toBeChecked = [];

        void Add(Point point, Point adjacent)
        {
            if (!expandedPipeLoop.Contains(point))
            {
                external.Add(point);
                toBeChecked.Enqueue(adjacent);
            }
        }

        // Bound the region so the algorithm cannot escape.
        for(int x = 0; x < (xWidth * 3); x++)
        {
            Point point = new (x, 0);
            Add(point, point.North());
        }
        for(int x = 0; x < (xWidth * 3); x++)
        {
            Point point = new (x, yWidth * 3 - 1);
            Add(point, point.South());
        }
        for(int y = 1; y < (yWidth * 3 - 1); y++)
        {
            Point point = new (0, y);
            Add(point, point.East());
        }
        for(int y = 1; y < (yWidth * 3 - 1); y++)
        {
            Point point = new (xWidth * 3 - 1, y);
            Add(point, point.West());
        }

        // Check the bounded region by looking from the outside in.
        while(0 < toBeChecked.Count)
        {
            Point point = toBeChecked.Dequeue();

            // already seen the point.
            if (expandedPipeLoop.Contains(point) || external.Contains(point))
            {
                continue;
            }

            // haven't seen the point, has to be a neighbour of an external point, so add it.
            // Each point will only add it's neighbours once this way, and only viable options. 
            external.Add(point);

            // The only way points are added to toBeChecked is if they are not in the loop, and not already checked.
            if (!expandedPipeLoop.Contains(point) && !external.Contains(point.North()))
            {
                toBeChecked.Enqueue(point.North());
            }

            if (!expandedPipeLoop.Contains(point) && !external.Contains(point.East()))
            {
                toBeChecked.Enqueue(point.East());
            }

            if (!expandedPipeLoop.Contains(point) && !external.Contains(point.South()))
            {
                toBeChecked.Enqueue(point.South());
            }

            if (!expandedPipeLoop.Contains(point) && !external.Contains(point.West()))
            {
                toBeChecked.Enqueue(point.West());
            }
        }

        double count = 0;

        for(int y = 0; y < yWidth; y++)
        {
            for (int x = 0; x < xWidth; x++)
            {
                char c = inputArray[y][x];
                Point point = new(x * 3, y * 3);

                if (!external.Contains(point) && !expandedPipeLoop.Contains(point))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static Point[] ExpandPoint(Point point, char c) => c switch
    {
        '|' => [ point.North(), point, point.South() ],
        '-' => [ point.West(), point, point.East() ],
        'L' => [ point.North(), point, point.East() ],
        'J' => [ point.North(), point, point.West() ],
        '7' => [ point.West(), point, point.South() ],
        'F' => [ point.East(), point, point.South() ],
        'S' => [ point.East(), point.North(), point, point.South(), point.West(), ],
        _ => throw new Exception()
    };

    private static Edge MapPipe(Point point, char c) => c switch
    {
        '|' => new(Beginning: point.North(), Middle: point, End: point.South()),
        '-' => new(Beginning: point.East(), Middle: point, End: point.West()),
        'L' => new(Beginning: point.North(), Middle: point, End: point.East()),
        'J' => new(Beginning: point.North(), Middle: point, End: point.West()),
        '7' => new(Beginning: point.South(), Middle: point, End: point.West()),
        'F' => new(Beginning: point.South(), Middle: point, End: point.East()),
        _ => throw new Exception()
    };

    private static Dictionary<Point, Edge> MapInput(IEnumerable<string> input, out Point start)
    {
        // Reverse so that y goes down-up instead of up-down.
        string[] pipes = input.Reverse().ToArray();

        // TODO: This should be an extension for mapping.
        int xWidth = pipes.All(p => p.Length == pipes.First().Length) ? pipes.First().Length : throw new Exception();
        int yWidth = pipes.Length;
        
        Dictionary<Point, Edge> map = [];
        Point? foundStart = null;

        for(int y = 0; y < yWidth; y++)
        {
            for(int x = 0; x < xWidth; x++)
            {
                char pipe = pipes[y][x];
                Point point = new (x, y);

                switch (pipe)
                {
                    case 'S':
                        foundStart = point;
                        break;
                    case '.':
                        break;
                    default:
                        map.Add(point, MapPipe(point, pipe));
                        break;
                }
            }
        }

        if (foundStart == null) 
        {
            throw new Exception();
        }
        else 
        {  
            start = foundStart;
        }

        return map;
    }

    private static Dictionary<Point, double> TraverseMap(Dictionary<Point, Edge> map, Point start)
    {
        Dictionary<Point, double> traverse = [];
        List<Point> adjacent = [ start.North(), start.East(), start.South(), start.West() ];

        foreach(Point point in adjacent)
        {
            List<Point> history = [ point ];

            Point previous = start;
            Point current = point;

            while(true)
            {
                // Dead end, no edge found for point.
                if (!map.TryGetValue(current, out var edge))
                {
                    break;
                }

                // Dead end, edge does not go through previous point.
                if (!(edge.Enters(previous) && edge.Next(previous) is Point next)) 
                {
                    break;
                }

                if (next == start)
                {
                    double depth = 1;

                    foreach(var historicPoint in history)
                    {
                        // Already have this point in the dictionary ...
                        if (traverse.TryGetValue(historicPoint, out var currentDepth))
                        {
                            if (depth < currentDepth)
                            {
                                // ... and it's higher, so change it.
                                traverse[historicPoint] = depth;
                            }
                            else 
                            {
                                // ... and it's lower, so we've lapped.
                                break;
                            }
                        }
                        else 
                        {
                            traverse.Add(historicPoint, depth);
                        }

                        depth++;
                    }

                    break;
                }

                previous = current;
                current = next;

                history.Add(next);
            }
        }

        return traverse;
    }
}
