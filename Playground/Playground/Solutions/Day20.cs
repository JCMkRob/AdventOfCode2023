using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day20
{
    private enum Pulse
    {
        High, 
        Low
    }

    private static Pulse Flip(this Pulse pulse) => (pulse == Pulse.High) ? Pulse.Low : Pulse.High;
    
    private enum ModuleType
    {
        Broadcaster,
        FlipFlop,
        Conjunction,
        Button
    }

    private abstract class Module(string Id)
    {
        public Pulse State { get; protected set; } = Pulse.Low;
        public string Id { get; } = Id;

        public abstract Pulse? Receive(string sender, Pulse input);
    }

    private class FlipFlop(string Id) : Module(Id)
    {
        public override Pulse? Receive(string _, Pulse input)
        {
            if (input == Pulse.High)
            {
                return null;
            }
            else 
            {
                return State = State.Flip();
            }
        }
    }

    private class Conjunction(string Id, string[] InputCables) : Module(Id)
    {
        private readonly Dictionary<string, Pulse> Memory = InputCables.ToDictionary(c => c, _ => Pulse.Low);
        public override Pulse? Receive(string sender, Pulse input)
        {
            Memory[sender] = input;

            return Memory.Values.All(p => p == Pulse.High) ? Pulse.Low : Pulse.High;
        }
    }

    private class Broadcast(string Id) : Module(Id)
    {
        public override Pulse? Receive(string _, Pulse input) => input;
    }

    private class Button(string Id) : Module(Id)
    {
        public override Pulse? Receive(string _, Pulse input) => Pulse.Low;
    }

    private static (string id, ModuleType module) CreateModule(string name) => (name, name[0]) switch
    {
        (_, '%') => (name[1..], ModuleType.FlipFlop),
        (_, '&') => (name[1..], ModuleType.Conjunction),
        ("broadcaster", _) => (name, ModuleType.Broadcaster),
        ("button", _) => (name, ModuleType.Button),
        (_, _) => throw new Exception()
    };

    private static (string id, ModuleType module, string[] cables) CreateMachine(string s) 
    {
        var split = s.Split(" -> ");
        var name = split[0];
        var cables = split[1].Split(", ").Where(c => 0 < c.Length).ToArray();
        
        (var id, var module) = CreateModule(name);

        return (id, module, cables);
    }

    private static Dictionary<string, (Module module, string[] cables)> CreateServer(IEnumerable<string> input)
    {
        Dictionary<string, (Module module, string[] cables)> server = [];
        Queue<(string id, ModuleType module, string[] cables)> conjuctions = [];
        
        foreach(string s in input)
        {
            var machine = CreateMachine(s);

            switch (machine.module)
            {
                case ModuleType.Conjunction:
                    conjuctions.Enqueue(machine);
                    break;
                case ModuleType.FlipFlop:
                    server.Add(machine.id, (new FlipFlop(machine.id), machine.cables));
                    break;
                case ModuleType.Broadcaster:
                    server.Add(machine.id, (new Broadcast(machine.id), machine.cables));
                    break;
            }
        }

        var conjuctionHash = conjuctions.Select(c => c.id).ToHashSet();
        var conjuctionInputs = server
            .Select(kv => (machine: kv.Key, conjunctions: kv.Value.cables.Where(c => conjuctionHash.Contains(c))))
            .SelectMany(kv => kv.conjunctions, (parent, child) => (input: parent.machine, conjunction: child))
            .GroupBy(kv => kv.conjunction)
            .ToDictionary(kv => kv.Key, kv => kv.Select(m => m.input).ToArray());

        while(0 < conjuctions.Count)
        {
            var (id, module, cables) = conjuctions.Dequeue();
            var inputs = conjuctionInputs[id];

            server.Add(id, (new Conjunction(id, inputs), cables));
        }

        return server;
    }

    private static (double lowCount , double highCount) PushButton(this Dictionary<string, (Module module, string[] cables)> server)
    {
        Queue<(string id, Pulse pulse, string cable)> queue = [];

        queue.Enqueue(("button", Pulse.Low, "broadcaster"));

        double lowCount = 0;
        double highCount = 0;

        while(0 < queue.Count)
        {
            var (inputId, inputPulse, targetModule) = queue.Dequeue();

            Console.WriteLine($"{inputId} -{((inputPulse == Pulse.High) ? "high" : "low")}-> {targetModule}");

            if (inputPulse == Pulse.High) highCount++;
            if (inputPulse == Pulse.Low) lowCount++;

            if (!server.TryGetValue(targetModule, out var output)) continue;

            if (output.module.Receive(inputId, inputPulse) is Pulse outputPulse)
            {
                foreach(var sendTo in output.cables)
                {
                    queue.Enqueue((output.module.Id, outputPulse, sendTo));
                }
            }
        }

        return (lowCount, highCount);
    }

    private static int Hash(this Dictionary<string, (Module module, string[] cables)> server)
    {
        return string.Join("", server.Select(m => $"{m.Value.module.Id}{m.Value.module.State}")).GetHashCode();
    }

    [Example(solver: nameof(PartOne), solution: -1)]
    public static readonly string PartOneExample = 
        """
        broadcaster -> a
        %a -> inv, con
        &inv -> b
        %b -> con
        &con -> output
        """;

    [Example(solver: nameof(PartTwo), solution: -1)]
    public static readonly string PartTwoExample = 
        """

        """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        var server = CreateServer(input);

        var cycle = new Dictionary<int, (int index, double low, double high)>();

        for(int index = 1; index < 1000; index++)
        {
            var (lowCount, highCount) = server.PushButton();
            var hash = server.Hash();

            if (cycle.TryGetValue(hash, out var c))
            {
                
            }
            else 
            {
                cycle.Add(hash, (index, lowCount, highCount));
            }
        }

        PushButton(server);
        PushButton(server);
        PushButton(server);
        PushButton(server);

        return 0;
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        return 0;
    }
}
