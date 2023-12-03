using System.Reflection;

namespace AdventOfCode.Support;

public static class Wrapper
{
    public static void Test(Type day)
    {
        var fields = day.GetFields().Where(f => f.IsDefined(typeof(Example), inherit: true));

        foreach(var field in fields)
        {
            if (Attribute.GetCustomAttribute(field, typeof(Example)) is not Example example) continue;
            if (field.GetValue(day) is not object input) continue;

            Console.Write($"Testing {field.Name}: ");
            string testResult = "Failed to run test.";

            foreach(var method in day.GetMethods())
            {
                if (method.Name != example.Solver) continue;
                if (method.ReturnType != typeof(double)) continue;
                if (method.GetParameters().Length != 1) continue;
                if (!method.GetParameters().All(p => p.ParameterType == input.GetType())) continue;

                if (method.Invoke(obj: null, [ input ]) is double answer)
                {
                    if (example.Solution == answer)
                    {
                        testResult = $"Passed.";
                    }
                    else 
                    {
                        testResult = $"Failed. Expected {example.Solution}, Received {answer}";
                    }
                }
            }

            Console.WriteLine(testResult);
        }
    }

    public static void Run(Type day, int part)
    {
        int dayNumber = int.Parse(day.Name[^2..]);
        
        Console.Write($"Running day {dayNumber:00} Part {part:0}: ");
        try 
        {
            if (GetSolutionFor(day, part) is not MethodInfo method) 
            {
                Console.WriteLine("Could not find method.");
                return;
            }
            
            if (method.Invoke(obj: null, parameters: [ Manager.ReadLines(day: dayNumber, part: part) ]) is double answer)
            {
                Console.WriteLine(answer);
            }
            else
            {
                Console.WriteLine("Failed to invoke method.");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Exception invoking method.");
            Console.WriteLine(ex);
        }
    }

    private static MethodInfo? GetSolutionFor(Type day, int part)
    {
        int dayNumber = int.Parse(day.Name[^2..]);
        
        var methods = day.GetMethods().Where(f => f.IsDefined(typeof(Solution), inherit: true));
        
        foreach(var method in methods)
        {
            if (Attribute.GetCustomAttribute(method, typeof(Solution)) is not Solution solution) continue;
            if (solution.Part != part) continue;

            return method;
        }

        return null;
    }
}

public static class Manager
{
    public static string PlaygroundPath => $@"{Directory.GetCurrentDirectory()}\..\..\..";
    public static string SolutionPath(int day)
    {
        return Path.Combine(PlaygroundPath, $@"Solutions\Day{day:00}.cs");
    }

    public static string InputPath(int day, int part) 
    {
        return Path.Combine(PlaygroundPath, $@"Inputs\day{day:00}part{part:0}.txt");
    }
    
    public static IEnumerable<string> ReadLines(int day, int part)
    {
        string path = InputPath(day, part);

        return File.ReadLines(path);
    }
}

public static class YearBuilder
{
    public static void CreateEmptyInputs()
    {
        string path = Path.Combine(Manager.PlaygroundPath, "Inputs");
        
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        for(int i = 1; i <= 25; i++)
        {
            var part1 = Path.Combine(path, $"day{i:00}part1.txt");
            var part2 = Path.Combine(path, $"day{i:00}part2.txt");

            if (!File.Exists(part1)) File.Create(part1);
            if (!File.Exists(part2)) File.Create(part2);
        }
    }

    public static void CreateEmptySolutions()
    {
        
        // string path = Path.Combine(Manager.PlaygroundPath, "Solutions");

        // if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // for(int i = 1; i <= 25; i++)
        // {
        //     var day = Path.Combine(path, $"day{i:00}.cs");

        //     if (!File.Exists(day)) File.Create(day);
        // }

        // TODO Create with boiler plate
    }
}
