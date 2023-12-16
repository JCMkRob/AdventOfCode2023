using System.Reflection.Emit;
using System.Text;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day15
{
    [Example(solver: nameof(PartOne), solution: 52)]
    public static readonly string PartOneExampleOne = 
        """
        HASH
        """;
    
    [Example(solver: nameof(PartOne), solution: 1320)]
    public static readonly string PartOneExampleTwo = 
        """
        rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
        """;

    [Example(solver: nameof(PartTwo), solution: 145)]
    public static readonly string PartTwoExample = 
        """
        rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
        """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        return string.Join("", input).Split(',').Select(Hash).Sum();
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        Dictionary<int, List<(string label, int lens)>> dictionary = [];

        var operations = string.Join("", input).Split(',');

        foreach(var operation in operations)
        {
            dictionary.Map(operation);
        }

        return dictionary.Select(kv => FocusingPower(kv.Key, kv.Value)).Sum();
    }

    private static int Hash(string s)
    {
        return Encoding.ASCII.GetBytes(s).Select(Convert.ToInt32).Aggregate(seed: 0, (a, b) => (a + b) * 17 % 256);
    }

    private enum Operation 
    {
        Equals,
        Dash
    }

    private static void Map(this Dictionary<int, List<(string label, int lens)>> dictionary, string s)
    {
        if (s.Contains('-'))
        {
            dictionary.DashSign(s);
        }
        if (s.Contains('='))
        {
            dictionary.EqualsSign(s);
        }
    }

    private static void DashSign(this Dictionary<int, List<(string label, int lens)>> dictionary, string s)
    {
        string label = s[0..^1];
        int hash = Hash(label);

        if (dictionary.TryGetValue(hash, out var box))
        {
            if ((box.FindIndex(l => l.label == label) is int index) && (0 <= index))
            {
                box.RemoveAt(index);
            }
        }
    }

    private static void EqualsSign(this Dictionary<int, List<(string label, int lens)>> dictionary, string s)
    {
        var split = s.Split('=');

        string label = split[0];
        int lens = int.Parse(split[1]);
        int hash = Hash(label);

        if (dictionary.TryGetValue(hash, out var box))
        {
            if ((box.FindIndex(l => l.label == label) is int index) && (0 <= index))
            {
                box.RemoveAt(index);
                box.Insert(index, (label, lens));
            }
            else 
            {
                box.Add((label, lens));
            }
        }
        else 
        {
            dictionary.Add(hash, [ (label, lens) ]);
        }
    }

    private static double FocusingPower(int hash, List<(string label, int lens)> box)
    {
        double power = 0;

        foreach(int index in box.Count.IncreasingTo())
        {
            power += (hash + 1) * (index + 1) * box[index].lens;
        }

        return power;
    }
}
