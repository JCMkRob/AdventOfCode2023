namespace AdventOfCode.Support;

/// <summary>
/// 
/// </summary>
/// <param name="solver">The method that will be executed with this value as it's input.</param>
/// <param name="solution">The known solution to the example. Output will be compared to this value.</param>
/// <param name="strictTypeEvaluation">By default, assumes a string input that should be split into line. Turning this on will cause the type of the field to be used explicitly.</param>
[AttributeUsage(AttributeTargets.Field)]
public class Example(string solver, double solution, bool strictTypeEvaluation = false) : Attribute
{
    public string Solver { get; } = solver;
    public double Solution { get; } = solution;
    public bool StrictTypeEvaluation { get; } = strictTypeEvaluation;
}

/// <summary>
/// Declares that this method should be run against the input for the defined day and part.
/// Expects a method that takes an IEnumerable<string> and returns a double.
/// </summary>
/// <param name="part">Which part of the solution to execute.</param>
[AttributeUsage(AttributeTargets.Method)]
public class Solution(int part) : Attribute
{
    public int Part { get; } = part;
}