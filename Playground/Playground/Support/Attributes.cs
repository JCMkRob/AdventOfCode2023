namespace AdventOfCode.Support;

[AttributeUsage(AttributeTargets.Field)]
public class Example(string solver, double solution, bool strictTypeEvaluation = false) : Attribute
{
    public string Solver { get; } = solver;
    public double Solution { get; } = solution;
    public bool StrictTypeEvaluation { get; } = strictTypeEvaluation;
}


[AttributeUsage(AttributeTargets.Method)]
public class Solution(int part) : Attribute
{
    public int Part { get; } = part;
}