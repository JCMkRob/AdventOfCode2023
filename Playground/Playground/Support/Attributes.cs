namespace AdventOfCode.Support;

[AttributeUsage(AttributeTargets.Field)]
public class Example(string solver, double solution) : Attribute
{
    public string Solver { get; } = solver;
    public double Solution { get; } = solution;
}
