using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day17
{
    private record Point(int X, int Y)
    {
        public Point Up() => new(X, Y + 1);
        public Point Right()  => new(X + 1, Y);
        public Point Down() => new(X, Y - 1);
        public Point Left()  => new(X - 1, Y);
    }
    
    private enum Heading 
    {
        Up,
        Down,
        Left,
        Right
    }

    private record Edge(Point Start, Point Finish, int Cost, Heading Direction, int Length);

    private static bool ValidPoint(Point point, int xWidth, int yWidth) => (0 <= point.X) && (point.X < xWidth) && (0 <= point.Y) && (point.Y < yWidth);

    [Example(solver: nameof(PartOne), solution: 102)]
    public static readonly string PartOneExample = 
        """
        2413432311323
        3215453535623
        3255245654254
        3446585845452
        4546657867536
        1438598798454
        4457876987766
        3637877979653
        4654967986887
        4564679986453
        1224686865563
        2546548887735
        4322674655533
        """;

    [Example(solver: nameof(PartTwo), solution: -1)]
    public static readonly string PartTwoExample = 
        """

        """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        var inputArray = input.ToArray();

        int yWidth = inputArray.Length;
        int xWidth = inputArray[0].Length;

        var edgeDictionary = new Dictionary<Point, Edge[]>();

        foreach (int x in xWidth.IncreasingTo())
        {
            foreach (int y in yWidth.IncreasingTo())
            {
                var point = new Point(x, y);
                var edges = CreateEdgesForPoint(inputArray, point, size: 3, xWidth: xWidth, yWidth: yWidth);

                edgeDictionary.Add(point, edges.ToArray());
            }
        }

        var start = new Point(0, 0);
        var goal = new Point(xWidth - 1, yWidth - 1);

        var minHeatLost = AStar(start, goal, edgeDictionary);

        return minHeatLost;
    }



    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        return 0;
    }

    private static IEnumerable<Edge> CreateEdgesForPoint(string[] inputArray, Point point, int size, int xWidth, int yWidth)
    {
        var downPoint = point;
        var leftPoint = point;
        var rightPoint = point;
        var upPoint = point;

        var downCost = 0;
        var leftCost = 0;
        var rightCost = 0;
        var upCost = 0;

        foreach(var length in size.IncreasingTo())
        {
            downPoint = downPoint.Down();
            leftPoint = leftPoint.Left();
            rightPoint = rightPoint.Right();
            upPoint = upPoint.Up();

            if (ValidPoint(downPoint, xWidth, yWidth))
            {
                downCost += int.Parse($"{inputArray[downPoint.Y][downPoint.X]}");

                yield return new Edge(point, downPoint, downCost, Heading.Down, length + 1);
            }

            if (ValidPoint(leftPoint, xWidth, yWidth))
            {
                leftCost += int.Parse($"{inputArray[leftPoint.Y][leftPoint.X]}");

                yield return new Edge(point, leftPoint, leftCost, Heading.Left, length + 1);
            }

            if (ValidPoint(rightPoint, xWidth, yWidth))
            {
                rightCost += int.Parse($"{inputArray[rightPoint.Y][rightPoint.X]}");

                yield return new Edge(point, rightPoint, rightCost, Heading.Right, length + 1);
            }

            if (ValidPoint(upPoint, xWidth, yWidth))
            {
                upCost += int.Parse($"{inputArray[upPoint.Y][upPoint.X]}");

                yield return new Edge(point, upPoint, upCost, Heading.Up, length + 1);
            }
        }
    }

    private static int GetWithDefault(this Dictionary<Point, int> dictionary, Point point, int defaultValue = int.MaxValue)
    {
        if (dictionary.TryGetValue(point, out var cost))
        {
            return cost;
        }
        else 
        {
            return defaultValue;
        }
    }


    private static List<Point> ReconstructPath(Dictionary<Point, Edge> cameFrom, Point current)
    {
        List<Point> totalPath = [ current ];

        while(cameFrom.TryGetValue(current, out var previous))
        {
            totalPath.Add(current);
            current = previous.Start;
        }

        totalPath.Reverse();

        return totalPath;
    }
    
    private static bool ValidEdge(Dictionary<Point, Edge> cameFrom, Edge edge)
    {
        Point current = edge.Start;
        var currentDirection = edge.Direction;
        var currentLength = edge.Length;

        while(cameFrom.TryGetValue(current, out var previous))
        {
            if (previous.Direction == currentDirection)
            {
                currentLength += previous.Length;
                current = previous.Start;
            }
            else 
            {
                break;
            }
        }

        return currentLength <= 3;
    }

    private static int AStar(Point start, Point goal, Dictionary<Point, Edge[]> edgeDictionary)
    {
        // Queue<Point> discoveredPoints = [];
        // discoveredPoints.Enqueue(start);

        // Dictionary<Point, Edge> cameFrom = [];

        // // Default to infinity
        // Dictionary<Point, List<(int, Heading)>> shortestStartToPoint = [];

        // shortestStartToPoint.Add(start, [(0, Heading.Up), (0, Heading.Down), (0, Heading.Left), (0, Heading.Right) ]);

        // while(0 < discoveredPoints.Count)
        // {
        //     var current = discoveredPoints.Dequeue();

        //     if (current == goal)
        //     {
        //         var x = ReconstructPath(cameFrom, current);

        //         return shortestStartToPoint[goal];
        //     }

        //     if (!edgeDictionary.TryGetValue(current, out var edges)) continue;

        //     foreach(var edge in edges)
        //     {
        //         if (!ValidEdge(cameFrom, edge)) continue;

        //         // get the list and remove any that are not valid continuatious.
        //         var tentativeList = shortestStartToPoint.GetWithDefault(current) + edge.Cost;
        //         var bestPrevious = shortestStartToPoint.GetWithDefault(edge.Finish);
                
        //         if (tentative < bestPrevious)
        //         {
        //             cameFrom[edge.Finish] = edge;

        //             shortestStartToPoint[edge.Finish] = tentative;

        //             if (!discoveredPoints.Contains(edge.Finish))
        //             {
        //                 discoveredPoints.Enqueue(edge.Finish);
        //             }
        //         }
        //     }
        // }

        throw new Exception();
    }
}
