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
        Dictionary<string, List<string>> conjuctionInputs = [];
        
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

            foreach(var cable in machine.cables)
            {
                conjuctionInputs.ListAdd(cable, machine.id);
            }
        }

        while(0 < conjuctions.Count)
        {
            var (id, module, cables) = conjuctions.Dequeue();

            var inputs = conjuctionInputs[id];
            server.Add(id, (new Conjunction(id, [.. inputs]), cables));
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

    private static bool PushButton(this Dictionary<string, (Module module, string[] cables)> server, string module)
    {
        Queue<(string id, Pulse pulse, string cable)> queue = [];

        queue.Enqueue(("button", Pulse.Low, "broadcaster"));

        bool success = false;
        
        while(0 < queue.Count)
        {
            var (inputId, inputPulse, targetModule) = queue.Dequeue();

            if (!server.TryGetValue(targetModule, out var output)) continue;

            if (output.module.Receive(inputId, inputPulse) is Pulse outputPulse)
            {
                foreach(var sendTo in output.cables)
                {
                    queue.Enqueue((output.module.Id, outputPulse, sendTo));

                    if ((sendTo == module) && (outputPulse == Pulse.Low))
                    {
                        success = true;
                    }
                }
            }
        }

        return success;
    }

    private static int Hash(this Dictionary<string, (Module module, string[] cables)> server)
    {
        return string.Join("", server.Select(m => $"{m.Value.module.Id}{m.Value.module.State}")).GetHashCode();
    }

    [Example(solver: nameof(PartOne), solution: 32000000)]
    public static readonly string PartOneExampleOne = 
        """
        broadcaster -> a, b, c
        %a -> b
        %b -> c
        %c -> inv
        &inv -> a
        """;

    [Example(solver: nameof(PartOne), solution: 11687500)]
    public static readonly string PartOneExampleTwo = 
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

        //var history = new Dictionary<int, (double index, double low, double high)>();
        var history = new Dictionary<int, (double index, double low, double high)>();
        var requestedCycles = 1000;
        var index = 0;

        while (true)
        {
            var (lowCount, highCount) = server.PushButton();
            var hash = server.Hash();

            if (history.TryGetValue(hash, out var machineState))
            {
                var initialGroup = history
                    .Where(s => s.Value.index < machineState.index)
                    .Select(s => (low: s.Value.low, high: s.Value.high));

                var initialGroupCount = initialGroup.Count();

                var repeatingGroup = history
                    .Where(s => s.Value.index >= machineState.index)
                    .Select(s => (low: s.Value.low, high: s.Value.high));

                var repeatingGroupCount = repeatingGroup.Count();
                
                var repeatingCount = (requestedCycles - initialGroupCount) / repeatingGroupCount;
                var remainderOffset = (requestedCycles - initialGroupCount) % repeatingGroupCount;

                var remainderGroup = history
                    .Where(s => s.Value.index < remainderOffset)
                    .Select(s => (low: s.Value.low, high: s.Value.high));

                var runningLow = 
                    initialGroup.Sum(s => s.low) + 
                    repeatingGroup.Sum(s => s.low) * repeatingCount +
                    remainderGroup.Sum(s => s.low);

                var runningHigh = 
                    initialGroup.Sum(s => s.high) + 
                    repeatingGroup.Sum(s => s.high) * repeatingCount +
                    remainderGroup.Sum(s => s.high);

                return runningLow * runningHigh;
            }
            else if (index == requestedCycles)
            {
                var totalGroup = history.Select(s => (low: s.Value.low, high: s.Value.high));

                return totalGroup.Sum(s => s.low) * totalGroup.Sum(s => s.high);
            }
            else 
            {
                history.Add(hash, (index++, lowCount, highCount));
            }
        }

        throw new Exception();
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        var server = CreateServer(input);
        double count = 0;

        while (true)
        {
            count++;

            if (server.PushButton(module: "rx"))
            {
                return count;
            }
        }
    }
}
