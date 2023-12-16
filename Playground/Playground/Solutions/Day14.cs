using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day14
{
    [Example(solver: nameof(PartOne), solution: 136)]
    public static readonly string PartOneExample = 
            """
            O....#....
            O.OO#....#
            .....##...
            OO.#O....O
            .O.....O#.
            O.#..O.#.#
            ..O..#O..O
            .......O..
            #....###..
            #OO..#....
            """;

    [Example(solver: nameof(PartTwo), solution: 64)]
    public static readonly string PartTwoExample = 
            """
            O....#....
            O.OO#....#
            .....##...
            OO.#O....O
            .O.....O#.
            O.#..O.#.#
            ..O..#O..O
            .......O..
            #....###..
            #OO..#....
            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        char[][] inputArray = [ .. input.Select<string, char[]>(s => [ .. s])];
        
        var outputArray = Tilt(inputArray, CardinalDirection.North);

        var load = TotalLoad(outputArray);
        
        return load;
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        char[][] arrayPointer = [ .. input.Select<string, char[]>(s => [ .. s])];
        var requestedCycles = 1000000000;

        List<(string hash, double load)> historyList = [];
        HashSet<string> historyHash = [];

        while(true)
        {
            arrayPointer = Cycle(arrayPointer);

            string hashCode = string.Join("", arrayPointer.Select(s => new string(s)));

            if (historyHash.Contains(hashCode))
            { 
                var before = historyList.FindIndex(h => h.hash == hashCode);

                var repeatingSegmentSize = historyList.Count - before;
                var remainder = (requestedCycles - before) % repeatingSegmentSize;
                var index = before + remainder - 1;

                return historyList[index].load;
            }
            else 
            {
                var load = TotalLoad(arrayPointer);

                historyList.Add((hashCode, load));
                historyHash.Add(hashCode); 
            }
        }
    }

    private static char[][] Cycle(char[][] inputArray)
    {
        var northArray = Tilt(inputArray, CardinalDirection.North);

        var westArray = Tilt(northArray, CardinalDirection.West);

        var southArray = Tilt(westArray, CardinalDirection.South);

        var eastArray = Tilt(southArray, CardinalDirection.East);

        return eastArray;
    }
    
    private enum CardinalDirection 
    {
        North,
        West,
        East,
        South
    }

    private static double TotalLoad(char[][] input)
    {
        int length = input.Length;
        double load = 0;

        for(int index = 0; index < length; index++)
        {
            load += input[index].Where(c => c == 'O').Count() * (length - index);
        }

        return load;
    }

    private static char[] TiltLine(char[] cArray)
    {
        string outputLine = new(cArray);

        int? emptySpace = null;

        foreach(var index in outputLine.Length.IncreasingTo())
        {
            char c = outputLine[index];

            if ((c == 'O') && (emptySpace is int emptyRoll))
            {
                outputLine = outputLine.Remove(emptyRoll, count: 1);
                outputLine = outputLine.Insert(emptyRoll, "O");

                outputLine = outputLine.Remove(index, count: 1);
                outputLine = outputLine.Insert(index, ".");

                emptySpace++;
            }
            else if ((c == '#') && (emptySpace != null))
            {
                emptySpace = null;
            }
            else if ((c == '.') && (emptySpace == null))
            {
                emptySpace = index;
            }
        }

        return [.. outputLine];
    }

    private static char[][] Tilt(char[][] input, CardinalDirection direction)
    {
        if (direction == CardinalDirection.North)
        {
            return Grid.ApplyToLines<char, int>(input, TiltLine, Grid.Orientation.Vertical, reverseX: false, reverseY: false);
        }
        if (direction == CardinalDirection.West)
        {
            return Grid.ApplyToLines<char, int>(input, TiltLine, Grid.Orientation.Horizontal, reverseX: false, reverseY: false);
        }
        if (direction == CardinalDirection.South)
        {
            return Grid.ApplyToLines<char, int>(input, TiltLine, Grid.Orientation.Vertical, reverseX: false, reverseY: true);
        }
        if (direction == CardinalDirection.East)
        {
            return Grid.ApplyToLines<char, int>(input, TiltLine, Grid.Orientation.Horizontal, reverseX: true, reverseY: false);
        }

        throw new Exception();
    }
}
