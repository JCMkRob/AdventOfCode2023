using System.Numerics;

namespace AdventOfCode.Support;


// Note, once the file gets large, split into separate files with a folder called: TypeExtensions 
public static class DictionaryExtensions
{
    public static void ProductAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) 
        where TKey : notnull 
        where TValue : INumber<TValue>
    {
        if (dictionary.TryGetValue(key, out TValue? _))
        {
            dictionary[key] *= value;
        }
        else 
        {
            dictionary.Add(key, value);
        }
    }

    public static void ListAdd<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value) where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out List<TValue>? v))
        {
            v.Add(value);
        }
        else 
        {
            dictionary.Add(key, [value]);
        }
    }

    public static void SumAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) 
        where TKey : notnull 
        where TValue : INumber<TValue>
    {
        if (dictionary.TryGetValue(key, out TValue? _))
        {
            dictionary[key] += value;
        }
        else 
        {
            dictionary.Add(key, value);
        }
    }

    public static T Cycle<T>(Func<(int hash, T T)> next, double requestedCycles, 
        Func<T, T, T> add, 
        Func<T, T, T> substract)
    {
        Dictionary<int, (double index, T value)> history = [];

        T? runningTotal = default;

        foreach (double index in requestedCycles.IncreasingTo())
        {
            var (hash, value) = next();

            if (history.TryGetValue(hash, out var beforeItem))
            { 
                var beforeIndex = beforeItem.index;

                var repeatingSegmentSize = history.Count - beforeIndex;
                var repeatingSegmentCount = (requestedCycles - beforeIndex) / repeatingSegmentSize;

                var remainderOffset = (requestedCycles - beforeIndex) % repeatingSegmentSize;
                var remainderIndex = beforeIndex + remainderOffset - 1;

                T initialAmount = history.Where(h => h.Value.index == (beforeIndex - 1)).FirstOrDefault().Value.value;
                T repeatingAmount = substract(beforeItem.value, initialAmount);
                T remainderAmount = history.Where(h => h.Value.index == remainderIndex).FirstOrDefault().Value.value;

                foreach(var _ in (repeatingSegmentCount - 1).IncreasingTo())
                {
                    repeatingAmount = add(repeatingAmount, repeatingAmount);
                }

                return add(initialAmount, remainderAmount);
            }
            else 
            {
                if (runningTotal == null)
                {
                    runningTotal = value;
                }
                else 
                {
                    runningTotal = add(runningTotal, value);
                }

                history.Add(hash, (index, value));
            }
        }

        throw new Exception();
    }
}

public static class Grid
{
// https://ericlippert.com/2010/06/28/computing-a-cartesian-product-with-linq/
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences) 
    { 
        IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() }; 
        
        return sequences.Aggregate(emptyProduct, (accumulator, sequence) => 
            from accseq in accumulator 
            from item in sequence 
            select accseq.Concat(new[] {item}));
    }

    public static bool TryGetReflection(this string[] input, out string[] output)
    {
        output = [];

        // Check that there is at least one string in the array, and is is non-empty.
        if (input.Length < 1 || input[0].Length < 1) return false;

        int yWidth = input.Length;
        int xWidth = input[0].Length;

        // Check that all strings in the array are of the same length.
        if (input.Any(l => l.Length != xWidth)) return false;

        List<string> reflectedInput = [];

        for (int x = 0; x < xWidth; x++)
        {
            string reflected = "";

            for (int y = 0; y < yWidth; y++)
            {
                reflected += input[y][x];
            }

            reflectedInput.Add(reflected);
        }

        output =  [.. reflectedInput];

        return true;
    }

    public static bool TryGetRotation(this string[] input, out string[] output, bool clockWise = true)
    {
        output = [];

        // Check that there is at least one string in the array, and is is non-empty.
        if (input.Length < 1 || input[0].Length < 1) return false;

        int yWidth = input.Length;
        int xWidth = input[0].Length;

        // Check that all strings in the array are of the same length.
        if (input.Any(l => l.Length != xWidth)) return false;

        List<string> rotatedInput = [];

        var xIndexes = clockWise ?  xWidth.IncreasingTo() : xWidth.DecreasingFrom();
        var yIndexes = clockWise ?  yWidth.DecreasingFrom() : xWidth.IncreasingTo();

        foreach(int x in xIndexes)
        {
            string reflected = "";
            
            foreach(int y in yIndexes)
            {
                reflected += input[y][x];
            }

            rotatedInput.Add(reflected);
        }
        
        output =  [.. rotatedInput];

        return true;
    }

    public static IEnumerable<T> IncreasingTo<T>(this T value) where T : INumber<T>
    {
        for(T index = T.Zero; index < value; index++)
        {
            yield return index;
        }
    }

    public static IEnumerable<T> DecreasingFrom<T>(this T value) where T : INumber<T>
    {
        for(T index = --value; T.Zero <= index; index--)
        {
            yield return index;
        }
    }

    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    
    public static TData[][] ApplyToLines<TData, TIndex>(this TData[][] data, 
        Func<TData[], TData[]> function,
        Orientation orientation = Orientation.Horizontal, 
        bool reverseX = false,  bool reverseY = false) where TIndex : INumber<TIndex>
    {
        // Check that there is at least one string in the array, and is is non-empty.
        if (data.Length < 1 || data[0].Length < 1) return data;

        var yWidth = data.Length;
        int xWidth = data[0].Length;

        // Check that all arrays are of the same length.
        if (data.Any(l => l.Length != xWidth)) throw new Exception("Rows are not of equal width.");

        var xRange = reverseX ? xWidth.DecreasingFrom() : xWidth.IncreasingTo();
        var yRange = reverseY ? yWidth.DecreasingFrom() : yWidth.IncreasingTo();

        TData[][] output = new TData[yWidth][];

        foreach(int row in yRange)
        {
            output[row] = new TData[xWidth];
        }

        if (orientation == Orientation.Horizontal)
        {
            foreach(var y in yRange)
            {
                List<TData> row = [];

                foreach(var x in xRange)
                {
                    row.Add(data[y][x]);
                }

                Queue<TData> queue = new(function([.. row]));

                foreach(var x in xRange)
                {
                    output[y][x] = queue.Dequeue();
                }
            }
        }

        if (orientation == Orientation.Vertical)
        {
            foreach(var x in xRange)
            {
                List<TData> row = [];

                foreach(var y in yRange)
                {
                    row.Add(data[y][x]);
                }

                Queue<TData> queue = new(function([.. row]));

                foreach(var y in yRange)
                {
                    output[y][x] = queue.Dequeue();
                }
            }
        }

        return [.. output];
    }

    public static IEnumerable<(T x, T y)> Traverse<T>(
        T xWidth, T yWidth, 
        Orientation orientation = Orientation.Horizontal, 
        bool reverseX = false,  bool reverseY = false) where T : INumber<T>
    {
        var xRange = reverseX ? xWidth.DecreasingFrom() : xWidth.IncreasingTo();
        var yRange = reverseY ? yWidth.DecreasingFrom() : yWidth.IncreasingTo();

        if (orientation == Orientation.Horizontal)
        {
            foreach(var y in yRange)
            {
                foreach(var x in xRange)
                {
                    yield return (x, y);
                }
            }
        }

        if (orientation == Orientation.Vertical)
        {
            foreach(var x in xRange)
            {
                foreach(var y in yRange)
                {
                    yield return (x, y);
                }
            }
        }
    }
}