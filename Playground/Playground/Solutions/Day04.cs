using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day04
{
    [Example(solver: nameof(PartOne), solution: 13)]
    public static readonly string PartOneExample = 
            """
            Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
            Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
            Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
            Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
            Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
            Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
            """;

    [Example(solver: nameof(PartTwo), solution: 30)]
    public static readonly string PartTwoExample = 
            """
            Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
            Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
            Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
            Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
            Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
            Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        return input.Select(Score).Sum();
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        int currentCard = 0;
        Dictionary<int, int> cards = [];

        foreach(var s in input)
        {   
            cards.SumAdd(currentCard, 1);

            Copies(s, currentCard, cards);

            currentCard++;
        }

        return cards.Values.Sum();
    }


    public static double Score(string s)
    {
        var card = s.Split(':')[1].Split('|');

        var winning = card[0].Trim().Split(' ').Where(c => 0 < c.Length).Select(double.Parse).ToHashSet();
        var numbers = card[1].Trim().Split(' ').Where(c => 0 < c.Length).Select(double.Parse).ToHashSet();

        var winningCount = winning.Intersect(numbers).Count();
        
        return (0 < winningCount) ? Math.Pow(2, winningCount - 1) : 0;
    }

    public static void Copies(string s, int currentCard, Dictionary<int, int> cards)
    {
        var card = s.Split(':')[1].Split('|');
        
        var winning = card[0].Trim().Split(' ').Where(c => 0 < c.Length).Select(double.Parse).ToHashSet();
        var numbers = card[1].Trim().Split(' ').Where(c => 0 < c.Length).Select(double.Parse).ToHashSet();

        var winningCount = winning.Intersect(numbers).Count();

        var copies = 1;
        if (cards.TryGetValue(currentCard, out var c)) copies = c;

        for(int i = currentCard + 1; i < currentCard + 1 + winningCount; i++)
        {
            cards.SumAdd(i, copies);
        }
    }
}