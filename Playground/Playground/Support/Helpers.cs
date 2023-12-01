using System.Diagnostics.Contracts;
using System.Reflection;

namespace AdventOfCode.Support;


public static class Wrapper
{
    public static void Test(Type day)
    {
        var fields = day.GetFields().Where(f => f.IsDefined(typeof(Example), inherit: true));

        foreach(var field in fields)
        {
            if (Attribute.GetCustomAttribute(field, typeof(Example)) is not Example example) return;
            if (field.GetValue(day) is not object input) return;

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

    public static void Run(int day, int part)
    {

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
