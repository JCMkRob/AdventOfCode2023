
namespace AdventOfCode.Support;

public static class Logging
{
    public static void Report(string? message = null, 
        [System.Runtime.CompilerServices.CallerMemberName] string? caller = null, 
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
        Exception? exception = null,
        Action<string>? writeLine = null)
    {
        writeLine ??= Console.WriteLine;

        if (message is string report)
        {
            writeLine($"report: {report}");
        }

        writeLine($"member name: {caller}");
        writeLine($"source file path: {sourceFilePath}");
        writeLine($"source line number: {sourceLineNumber}");

        if (exception is Exception ex)
        {
            writeLine($"Exception:\n{sourceLineNumber}");
        }
    }
}