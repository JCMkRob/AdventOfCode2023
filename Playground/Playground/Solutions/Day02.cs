using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public record Bag(int Red, int Green, int Blue)
{
    public static bool operator <=(Bag a, Bag b) 
    {
        return (a.Red <= b.Red) && (a.Green <= b.Green) && (a.Blue <= b.Blue);
    }

    public static bool operator >=(Bag a, Bag b) 
    {
        return (a.Red >= b.Red) && (a.Green >= b.Green) && (a.Blue >= b.Blue);
    }
}

public static class Day02
{
    public static double PartOneTest(string s) => PartOne(s.Split("\n"), new(Red: 12, Green: 13, Blue: 14));

    [Example(solver: nameof(PartOneTest), solution: 8)]
    public static readonly string PartOneExample = 
            """
            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
            """;

    public static double PartTwoTest(string s) => PartTwo(s.Split("\n"));
    
    [Example(solver: nameof(PartTwoTest), solution: 2286)]
    public static readonly string PartTwoExample = 
            """
            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
            """;
    

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> s) => PartOne(s, new(Red: 12, Green: 13, Blue: 14));

    private static double PartOne(IEnumerable<string> s, Bag max)
    {
        return s
            .Select(Game)
            .Where(game => game.bags.All(bag => bag <= max))
            .Sum(game => game.gameID);
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> s)
    {
        return s
            .Select(Game)
            .Select(g => Power(g.bags))
            .Sum();
    }

    private static double Power(Bag[] bags)
    {
        double minRed = bags.Where(b => 0 < b.Red).Max(b => b.Red);
        double minGreen = bags.Where(b => 0 < b.Green).Max(b => b.Green);
        double minBlue = bags.Where(b => 0 < b.Blue).Max(b => b.Blue);

        return minRed * minGreen * minBlue;
    }

    private static (Bag[] bags, double gameID) Game(string s)
    {
        var x = s.Trim().Split(':');
        
        var gameID = double.Parse(x[0].Split(' ')[1]);
        var bags = x[1].Split(';').Select(p => p.ToBag()).ToArray();

        return (bags, gameID);
    }

    private static Bag ToBag(this string s)
    {
        var red = 0;
        var green = 0;
        var blue = 0;

        foreach(var segment in s.Trim().Split(',').Select(p => p.Trim().Split(' ')))
        {
            switch (segment[1])
            {
                case "red":
                    red = int.Parse(segment[0]);
                    break;
                case "green":
                    green = int.Parse(segment[0]);
                    break;
                case "blue":
                    blue = int.Parse(segment[0]);
                    break;
            }
        }

        return new Bag(Red: red, Green: green, Blue: blue);
    }

}