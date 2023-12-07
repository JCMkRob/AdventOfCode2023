using System.Reflection;
using AdventOfCode.Solutions;

namespace AdventOfCode.Support;

public static class Wrapper
{
    public static void Test(Type day)
    {
        try 
        {
            var fields = day.GetFields().Where(f => f.IsDefined(typeof(Example), inherit: true));

            foreach(var field in fields)
            {
                if (Attribute.GetCustomAttribute(field, typeof(Example)) is not Example example) continue;
                
                if (field.GetValue(day) is not object exampleInput) return;

                Console.Write($"Testing {field.Name}: ");
                string testResult = "Failed to run test.";

                foreach(var method in day.GetMethods())
                {
                    if (method.Name != example.Solver) continue;
                    if (method.ReturnType != typeof(double)) continue;
                    if (method.GetParameters().Length != 1) continue;

                    object? evaluationResult = example.StrictTypeEvaluation ? StrictTypeEvaluation(exampleInput, method) : DefaultTypeEvaluation(exampleInput, method);
                    
                    if (evaluationResult is double answer)
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
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown running tests for class {day?.Name}:\n{ex}");
        }
    }

    private static object? DefaultTypeEvaluation(object exampleInput, MethodInfo method)
    {
        if (exampleInput is not string input) return null;
        if (!method.GetParameters().All(p => p.ParameterType == typeof(IEnumerable<string>))) return null;

        return method.Invoke(obj: null, [ input.Split("\n").Select(s => s.TrimEnd()) ]);
    }   

    private static object? StrictTypeEvaluation(object exampleInput, MethodInfo method)
    {
        if (!method.GetParameters().All(p => p.ParameterType == exampleInput.GetType())) return null;

        return method.Invoke(obj: null, [ exampleInput ]);
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
    
    public static string SolutionPath() => Path.Combine(PlaygroundPath, "Solutions");
    public static string SolutionPath(int day)
    {
        return Path.Combine(PlaygroundPath, $@"Solutions\Day{day:00}.cs");
    }

    public static string InputPath() => Path.Combine(PlaygroundPath, "Inputs");
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
    public static async Task SetupNewYear()
    {
        CreateEmptyInputs();
        await CreateEmptySolutions();
    }

    public static void CreateEmptyInputs()
    {
        try
        {
            string path = Manager.InputPath();

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            int adventOfCodeDayCount = 25;

            for(int day = 1; day <= adventOfCodeDayCount; day++)
            {
                var part1 = Manager.InputPath(day, part: 1);
                var part2 = Manager.InputPath(day, part: 2);

                if (!File.Exists(part1)) File.Create(part1);
                if (!File.Exists(part2)) File.Create(part2);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown creating empty inputs:\n{ex}");
        }
    }

    public static async Task CreateEmptySolutions()
    {
        try
        {
            string path = Manager.SolutionPath();

            if (!Directory.Exists(path)) 
            {
                // If there is no directory, there is no default implementation of a day, so return.
                Directory.CreateDirectory(path);
                return; 
            }

            var defaultImplementationName = nameof(DayXX);
            var defaultImplementationPath = Path.Combine(path, $"{defaultImplementationName}.cs");

            if (!File.Exists(defaultImplementationPath)) 
            {
                // If there is no default implementation of a day, there is no default implementation of a day, so return.
                return;
            }    

            var defaultImplementationText = File.ReadAllText(defaultImplementationPath);

            int adventOfCodeDayCount = 25;

            for(int day = 1; day <= adventOfCodeDayCount; day++)
            {
                var dayName = $"Day{day:00}";
                var dayPath = Path.Combine(path, $"{dayName}.cs");
                var dayText = defaultImplementationText.Replace(defaultImplementationName, dayName);

                if (!File.Exists(dayPath)) 
                { 
                    using StreamWriter outputFile = new(dayPath);
                    
                    await outputFile.WriteLineAsync(dayText);

                    outputFile.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown creating empty inputs:\n{ex}");
        }
    }
}
