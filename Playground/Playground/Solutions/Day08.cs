using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day08
{
    private enum Instruction
    {
        Left,
        Right
    }

    private class Cycle<T>(IEnumerable<T> items, IEnumerable<T>? refresh = null)
    {
        private IEnumerable<T> Refresh { get; set; } = refresh ?? items;
        private Queue<T> Items { get; set; } = new(items);

        public T Next() 
        {
            if (Items.Count == 0)
            {
                Items = new Queue<T> (Refresh);
            }

            return Items.Dequeue();
        }

        public T Peek() 
        {
            if (Items.Count == 0)
            {
                Items = new Queue<T> (Refresh);
            }

            return Items.Peek();
        }
    }

    [Example(solver: nameof(PartOne), solution: 2)]
    public static readonly string PartOneExampleOne = 
        """
        RL

        AAA = (BBB, CCC)
        BBB = (DDD, EEE)
        CCC = (ZZZ, GGG)
        DDD = (DDD, DDD)
        EEE = (EEE, EEE)
        GGG = (GGG, GGG)
        ZZZ = (ZZZ, ZZZ)
        """;

    [Example(solver: nameof(PartOne), solution: 6)]
    public static readonly string PartOneExampleTwo = 
        """
        LLR

        AAA = (BBB, BBB)
        BBB = (AAA, ZZZ)
        ZZZ = (ZZZ, ZZZ)
        """;

    [Example(solver: nameof(PartTwo), solution: 6)]
    public static readonly string PartTwoExample = 
        """
        LR

        11A = (11B, XXX)
        11B = (XXX, 11Z)
        11Z = (11B, XXX)
        22A = (22B, XXX)
        22B = (22C, 22C)
        22C = (22Z, 22Z)
        22Z = (22B, 22B)
        XXX = (XXX, XXX)
        """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        Dictionary<string, (string left, string right)> network = [];

        string directions = ParseInput(input, network);

        Cycle<Instruction> instructions = new(directions.Select(c => 
        {
            if (c == 'L') return Instruction.Left;
            if (c == 'R') return Instruction.Right;
            throw new Exception();
        }));

        return StepsToPartOne(directions, network);
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        Dictionary<string, (string left, string right)> network = [];

        string directions = ParseInput(input, network);

        Queue<Instruction> instructions = [];

        foreach(char c in directions)
        {
            if (c == 'L') instructions.Enqueue(Instruction.Left);
            if (c == 'R') instructions.Enqueue(Instruction.Right);
        }

        return StepsToPartTwo(directions, network);
    }

    public static string ParseInput(IEnumerable<string> input, Dictionary<string, (string left, string right)> network)
    {
        string directions = "";

        foreach(string s in input)
        {
            if (s.Contains('='))
            {
                network.Add(s[0..3], new (s[7..10], s[12..15]));
            }
            else if (!string.IsNullOrEmpty(s))
            {
                directions = s;
            }
        }

        return directions;
    }

    private static double StepsToPartOne(string directions, Dictionary<string, (string left, string right)> network) 
    {
        string start = "AAA";
        string end = "ZZZ";

        Cycle<Instruction> instructions = new(directions.Select(c => 
        {
            if (c == 'L') return Instruction.Left;
            if (c == 'R') return Instruction.Right;
            throw new Exception();
        }));

        double stepsTo = 0;

        while (true)
        {
            var (left, right) = network[start];
            
            stepsTo++;

            Instruction next = instructions.Next();

            if (next == Instruction.Left)
            {
                if (left == end)
                
                    break;
                
                else 
                {
                    start = left;
                }
            }
            else if (next == Instruction.Right)
            {
                if (right == end)
                { 
                    break;
                }
                else 
                {
                    start = right;
                }
            }
        }
        
        return stepsTo;
    }

    private static Cycle<double> Collapse(string currentNode, string directions, Dictionary<string, (string left, string right)> network) 
    {
        List<((string origin, Instruction instruction) origin, double distance)> map = [];

        Cycle<Instruction> instructions = new(directions.Select(c => 
        {
            if (c == 'L') return Instruction.Left;
            if (c == 'R') return Instruction.Right;
            throw new Exception();
        }));

        Instruction currentInstruction = instructions.Peek();
        var currentOrigin = (currentNode, currentInstruction);
        double currentStep = 0;

        while (true)
        {
            var (left, right) = network[currentNode];
            currentInstruction = instructions.Next();
            
            if (currentNode.EndsWith('Z'))
            {
                map.Add((currentOrigin, currentStep));

                currentOrigin = (currentNode, currentInstruction);
                currentStep = 0;

                if (map.Any(n => n.origin == currentOrigin)) break;
            }
            if (currentInstruction == Instruction.Left)
            {
                currentNode = left;
            }

            if (currentInstruction == Instruction.Right)
            {
                currentNode = right;
            }

            currentStep++;
        }


        var loopedNode = map.Where(n => n.origin == currentOrigin).First();
        var loopedNodeIndex = map.IndexOf(loopedNode);

        return new Cycle<double>(
            map[..].Select(m => m.distance), 
            map[loopedNodeIndex..].Select(m => m.distance));
    }

    private class Container(double currentStep, Cycle<double> progressor)
    {
        public double CurrentStep { get; set; } = currentStep;
        private Cycle<double> Progressor { get; } = progressor;

        public double Progress() => CurrentStep += Progressor.Next();
    }

    private static double StepsToPartTwo(string directions, Dictionary<string, (string left, string right)> network) 
    {
        string[] origins = network.Keys.Where(k => k.EndsWith('A')).ToArray();
        
        var maps = origins
            .Select(s => Collapse(s, directions, network))
            .Select(m => new Container(0, m))
            .ToList();

        Dictionary<double, double> commonMultiples = [];

        foreach(var map in maps)
        {
            commonMultiples.SumAdd(map.Progress(), 1);
        }

        maps.Sort((a, b) => a.CurrentStep.CompareTo(b.CurrentStep));

        int simultaneousCount = maps.Count;

        double solution = 0;

        while (true)
        {
            maps.Sort((a, b) => a.CurrentStep.CompareTo(b.CurrentStep));
            var currentStep = maps.First().CurrentStep;

            if (commonMultiples[currentStep] == simultaneousCount)
            {
                solution = currentStep;
                break;
            }
            else 
            {
                foreach(var invalidSolution in commonMultiples.Where(kv => kv.Key < currentStep).Select(kv => kv.Key).ToArray())
                {
                    commonMultiples.Remove(invalidSolution);
                }
            }

            commonMultiples.SumAdd(maps.First().Progress(), 1);
        }

        return solution;
    }
}
