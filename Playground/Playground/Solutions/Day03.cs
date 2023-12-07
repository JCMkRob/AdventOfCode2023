using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day03
{
    // NOTE: This is where I had a 'learning experience' with records.
    // These are kept as internal private classes as the names may be re-used and these don't define ennough behaviour to be worth pilling out.
    private class ValueContainer(double value) 
    {
        public double Value {get; set;} = value;
    }

    private class SymbolContainer(char symbol)
    {
        public char Value {get; set;} = symbol;
    }

    [Example(solver: nameof(PartOne), solution: 4361)]
    public static readonly string PartOneExample = 
            """
            467..114..
            ...*......
            ..35..633.
            ......#...
            617*......
            .....+.58.
            ..592.....
            ......755.
            ...$.*....
            .664.598..
            """;


    [Example(solver: nameof(PartTwo), solution: 467835)]
    public static readonly string PartTwoExample = 
            """
            467..114..
            ...*......
            ..35..633.
            ......#...
            617*......
            .....+.58.
            ..592.....
            ......755.
            ...$.*....
            .664.598..
            """;


    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        Dictionary<(double x, double y), List<ValueContainer>> valuesCoordinates = [];
        Dictionary<(double x, double y), List<SymbolContainer>> symbolsCoordinates = [];

        Generate(input, valuesCoordinates, symbolsCoordinates);

        HashSet<ValueContainer> MissingPartNumbers = [];

        foreach(var key in symbolsCoordinates.Keys)
        {
            if (valuesCoordinates.TryGetValue(key, out var partNumberList))
            {
                foreach (var partNumber in partNumberList)
                {
                    MissingPartNumbers.Add(partNumber);
                }
            }
        }

        return MissingPartNumbers.Sum(p => p.Value);
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        Dictionary<(double x, double y), List<ValueContainer>> valuesCoordinates = [];
        Dictionary<(double x, double y), List<SymbolContainer>> symbolsCoordinates = [];

        Generate(input, valuesCoordinates, symbolsCoordinates);

        Dictionary<SymbolContainer, List<ValueContainer>> gearsWithRatios = [];

        foreach(var s_cord in symbolsCoordinates)
        {
            if (valuesCoordinates.TryGetValue(s_cord.Key, out var valueList))
            {
                foreach (var symbol in s_cord.Value)
                {
                    if (symbol.Value == '*')
                    {
                        if (gearsWithRatios.TryGetValue(symbol, out var list))
                        {
                            foreach(var value in valueList)
                            {
                                list.Add(value);
                            }
                        }
                        else 
                        {
                            gearsWithRatios.Add(symbol, new List<ValueContainer> (valueList));
                        }
                    }
                }
            }
        }

        double result = 0;

        foreach(var gear in gearsWithRatios)
        {
            var values = gear.Value.Distinct().ToArray();

            if (values.Length == 2)
            {
                result += values[0].Value * values[1].Value;
            }
        }

        return result;
    }

    private static void Generate(IEnumerable<string> strings,
        Dictionary<(double x, double y), List<ValueContainer>> valuesCoordinates, 
        Dictionary<(double x, double y), List<SymbolContainer>>  symbolsCoordinates)
    {
        int y = 0;
        string value = "";

        foreach (string s in strings)
        {
            for (int x = 0; x < s.Length; x++)
            {
                char c = s[x];

                if (char.IsDigit(c)) 
                {
                    value += c;
                }
                else
                {
                    if (c != '.') 
                    {
                        AddSymbol(c, x, y, symbolsCoordinates);
                    }
                    if (0 < value.Length)
                    {
                        AddValue(value, x, y, valuesCoordinates);
                        value = "";
                    }
                }
            }

            if (0 < value.Length) 
            {
                AddValue(value, s.Length, y, valuesCoordinates);
                value = "";
            }
            
            y++;
        }
    }

    private static void AddValue(string s, double x_cord, double y_cord, Dictionary<(double x, double y), List<ValueContainer>> dict)
    {
        ValueContainer value = new(double.Parse(s));

        for (double x = x_cord - 1; x_cord - s.Length <= x; x--)
        {
            if (dict.TryGetValue(new (x, y_cord), out var list))
            {
                list.Add(value);
            }
            else 
            {
                dict.Add(new (x, y_cord), [ value ]);
            }
        }
    }

    private static void AddSymbol(char c, double x_cord, double y_cord, Dictionary<(double x, double y), List<SymbolContainer>> dict)
    {
        SymbolContainer symbol = new(c);

        for (double y = y_cord - 1; y <= y_cord + 1; y++)
        {
            for (double x = x_cord - 1; x <= x_cord + 1; x++)
            {
                if (dict.TryGetValue(new (x, y), out var list))
                {
                    list.Add(symbol);
                }
                else 
                {
                    dict.Add(new (x, y), [ symbol ]);
                }
            }
        }
    }
}