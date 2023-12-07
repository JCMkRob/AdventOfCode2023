
namespace AdventOfCode.Support;

public static class YearFileManager
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