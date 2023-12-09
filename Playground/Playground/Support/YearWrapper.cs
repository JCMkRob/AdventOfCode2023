using System.Reflection;
using AdventOfCode.Solutions;

namespace AdventOfCode.Support;

public static class YearWrapper
{
    public static void CurrentDayRunAndTest()
    {
        try 
        {
            if (DateTime.Now.Month != 12) 
            {
                Console.WriteLine($"It's not December! You're going to have to declare what you want, bub.");
                return;
            }

            var dayOfMonth = DateTime.Now.Day;
            
            if (25 < DateTime.Now.Month) 
            {
                Console.WriteLine($"It's past Christmas! You're going to have to declare what you want, pal.");
                return;
            }

            // TODO: Time shift
            // TODO: Oh, that's an idea. Could have year as an optional parameter and break solutions and Inputs down by year. 
            // I'd want to move previous years to their own folders within a root folder called PreviousYears, To keep the structure clean for the current year.
            

            if (Type.GetType($"AdventOfCode.Solutions.Day{dayOfMonth:00}") is Type currentDaySolution)
            {
                Console.WriteLine($"Running Advent of Code {DateTime.Now.Year}, Day {dayOfMonth:00}...");

                Test(currentDaySolution);

                Run(currentDaySolution, part: 1);
                Run(currentDaySolution, part: 2);
            }
            else 
            {
                Console.WriteLine($"No type was found for Day{dayOfMonth:00}.");
            }
        }
        catch (Exception ex)
        {    
            Logging.Report(exception: ex);
        }
    }

    public static void TestAll()
    {
        for(int day = 1; day <= Constants.AdventOfCodeDayCount; day++)
        {
            if (Type.GetType($"AdventOfCode.Solutions.Day{day:00}") is Type daySolution)
            {
                Test(daySolution);
            }
            else 
            {
                Console.WriteLine($"{nameof(Test)}: No type found for Day{day:00}.");
            }
        }
    }

    private record MethodSignature(string Name, Type ReturnType, Type PropertyType);
    public static void Test(Type day)
    {
        try 
        {
            var fields = day.GetFields().Where(f => f.IsDefined(typeof(Example), inherit: true));

            var methodDictionary = day
                .GetMethods()
                .Where(m => m.GetParameters().Length == 1)
                .ToDictionary(m => new MethodSignature(m.Name, m.ReturnType, m.GetParameters().Select(s => s.ParameterType).First() ));

            foreach(var field in fields)
            {
                if (field.GetValue(day) is not object inputValue) continue;
                if (Attribute.GetCustomAttribute(field, typeof(Example)) is not Example example) continue;
                
                var inputType = example.StrictParameterType ? inputValue.GetType() : typeof(IEnumerable<string>);
                var returnType = example.StrictParameterType ? example.Solution.GetType() : typeof(double);

                if (!example.StrictParameterType && inputValue is string s)
                {
                    inputValue = s.Split("\n").Select(s => s.TrimEnd());
                }

                var methodSignature = new MethodSignature(example.Solver, returnType, inputType);

                Console.Write($"Testing {day.Name} {field.Name}: ");

                string testResult = "Failed to run test.";
                
                if (methodDictionary.TryGetValue(methodSignature, out var method))
                {
                    if (method.Invoke(obj: null, [ inputValue ]) is not object solverSolution) continue;
                    
                    var solved = example.StrictParameterType ? 
                        example.Solution.Equals(solverSolution) : 
                        Convert.ToDouble(example.Solution) == Convert.ToDouble(solverSolution);

                    if (solved)
                    {
                        testResult = "Passed.";
                    }
                    else 
                    {
                        testResult = $"Failed. Expected {example.Solution}, Received {solverSolution}.";
                    }
                }
                else 
                {
                    testResult = $"Could not find method for described signature.";
                }

                Console.WriteLine(testResult);
            }
        }
        catch (Exception ex)
        {
            Logging.Report(message: $"Exception thrown running tests for class {day?.Name}.", exception: ex);
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

    public static void RunAll()
    {
        for(int day = 1; day <= Constants.AdventOfCodeDayCount; day++)
        {
            if (Type.GetType($"AdventOfCode.Solutions.Day{day:00}") is Type daySolution)
            {
                Run(daySolution, part: 1);
                Run(daySolution, part: 2);
            }
            else 
            {
                Console.WriteLine($"{nameof(Test)}: No type found for Day{day:00}.");
            }
        }
    }

    public static void Run(Type day, int part)
    {
        try 
        {
            int dayNumber = int.Parse(day.Name[^2..]);
        
            Console.Write($"Running Day{dayNumber:00} Part{part:0}: ");

            if (GetSolutionFor(day, part) is not MethodInfo method) 
            {
                Console.WriteLine("Could not find method.");
                return;
            }
            
            if (method.Invoke(obj: null, parameters: [ YearFileManager.ReadLines(day: dayNumber, part: part) ]) is double answer)
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
            Logging.Report(exception: ex);
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

