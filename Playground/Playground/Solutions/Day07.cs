using System.CodeDom.Compiler;
using System.Diagnostics;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public enum HandStrength
{
    FiveOfAKind = 6, 
    FourOfAKind = 5,
    FullHouse = 4,
    ThreeOfAKind = 3,
    TwoPair = 2,
    OnePair = 1,
    HighCard = 0
}

public record CamelHand(string Hand, double Bid, HandStrength Strength);

public static class Day07
{
    [Example(solver: nameof(PartOne), solution: 6440)]
    public static readonly string PartOneExample = 
            """
            32T3K 765
            T55J5 684
            KK677 28
            KTJJT 220
            QQQJA 483
            """;

    [Example(solver: nameof(PartTwo), solution: 5905)]
    public static readonly string PartTwoExample = 
            """
            32T3K 765
            T55J5 684
            KK677 28
            KTJJT 220
            QQQJA 483
            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    {
        return Solver(input, CalculateHandStrengthPartOne, CalculateCardStrengthPartOne);
    }

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        return Solver(input, CalculateHandStrengthPartTwo, CalculateCardStrengthPartTwo);
    }

    private static double Solver(IEnumerable<string> input, Func<string, HandStrength> calcHandStrength, Func<char, int> calcCardStrength)
    {
        var hands = input
            .Select(s => 
            {
                var split = s.Split(' ');

                return new CamelHand(
                    Hand: split[0].Trim(), 
                    Bid: double.Parse(split[1].Trim()), 
                    Strength: calcHandStrength(split[0].Trim()));
            })
            .ToList();

        hands.Sort((a, b) => 
        {
            if (a.Strength == b.Strength)
            {
                return CompareHandStrengthByCard(a.Hand, b.Hand, calcCardStrength);
            }
            else
            {
                return a.Strength.CompareTo(b.Strength);
            }
        });

        return hands.Select((value, index) => value.Bid * (index + 1)).Sum();
    }
    
    private static int CompareHandStrengthByCard(string a, string b, Func<char, int> calcCardStrength)
    {
        if (a.Length != b.Length) throw new Exception("Differing lengths.");

        for(int i = 0; i < a.Length; i++)
        {
            int strengthA = calcCardStrength(a[i]);
            int strengthB = calcCardStrength(b[i]);

            if (strengthA != strengthB) 
            {
                return strengthA.CompareTo(strengthB);
            }
        }

        return 0;
    }

    private static int CalculateCardStrengthPartOne(char c) => c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => 11,
        'T' => 10,
        _   => int.Parse($"{c}")
    };

    private static HandStrength CalculateHandStrengthPartOne(string s)
    {
        var counts = s.GroupBy(c => c).Select(c => c.Count()).OrderByDescending(c => c).ToArray();

        if ((counts.Length == 1) &&
            (counts[0] == 5))
        { 
            return HandStrength.FiveOfAKind;
        }
        if ((counts.Length == 2) &&
            (counts[0] == 4) && 
            (counts[1] == 1))
        { 
            return HandStrength.FiveOfAKind;
        }
        if ((counts.Length == 2) &&
            (counts[0] == 3) && 
            (counts[1] == 2)) 
        { 
            return HandStrength.FullHouse;
        }
        if ((counts.Length == 3) &&
            (counts[0] == 3) && 
            (counts[1] == 1) && 
            (counts[2] == 1)) 
        { 
            return HandStrength.ThreeOfAKind;
        }
        if ((counts.Length == 3) &&
            (counts[0] == 2) && 
            (counts[1] == 2) && 
            (counts[2] == 1)) 
        { 
            return HandStrength.TwoPair;
        }
        if ((counts.Length == 4) &&
            (counts[0] == 2) && 
            (counts[1] == 1) && 
            (counts[2] == 1) && 
            (counts[3] == 1)) 
        { 
            return HandStrength.OnePair;
        }
        if ((counts.Length == 5) &&
            (counts[0] == 1) && 
            (counts[1] == 1) && 
            (counts[2] == 1) && 
            (counts[3] == 1) && 
            (counts[4] == 1)) 
        { 
            return HandStrength.HighCard;
        }
            
        throw new Exception("Cards did not match any defined hand.");
    }

    private static int CalculateCardStrengthPartTwo(char c) => c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => 1, // now lowest card.
        'T' => 10,
        _   => int.Parse($"{c}")
    };

    private static HandStrength CalculateHandStrengthPartTwo(string s)
    {
        var counts = s.GroupBy(c => c).Select(c => c.Count()).OrderByDescending(c => c).ToArray();
        var jokers = s.Where(c => c == 'J').Count();

        if ((counts.Length == 1) &&
            (counts[0] == 5)) 
        {
            return HandStrength.FiveOfAKind;
        }
        if ((counts.Length == 2) &&
            (counts[0] == 4) && 
            (counts[1] == 1)) 
        {
            if ((jokers == 1) || (jokers == 4))
            {
                return HandStrength.FiveOfAKind;
            }
            else
            {
                return HandStrength.FourOfAKind;
            }
        }
        if ((counts.Length == 2) &&
            (counts[0] == 3) && 
            (counts[1] == 2)) 
        {
            if ((jokers == 2) || (jokers == 3))
            {
                return HandStrength.FiveOfAKind;
            }
            else
            {
                return HandStrength.FullHouse;
            }
        }
        if ((counts.Length == 3) &&
            (counts[0] == 3) && 
            (counts[1] == 1) && 
            (counts[2] == 1)) 
        {
            if ((jokers == 1) || (jokers == 3))
            {
                return HandStrength.FourOfAKind;
            }
            else
            {
                return HandStrength.ThreeOfAKind;
            }
        }
        if ((counts.Length == 3) &&
            (counts[0] == 2) && 
            (counts[1] == 2) && 
            (counts[2] == 1)) 
        {
            if (jokers == 1)
            {
                return HandStrength.FullHouse;
            }
            else if (jokers == 2)
            {
                return HandStrength.FourOfAKind;
            }
            else
            {
                return HandStrength.TwoPair;
            }
        }
        if ((counts.Length == 4) &&
            (counts[0] == 2) && 
            (counts[1] == 1) && 
            (counts[2] == 1) && 
            (counts[3] == 1)) 
        {
            if ((jokers == 1) || (jokers == 2))
            {
                return HandStrength.ThreeOfAKind;
            }
            else
            {
                return HandStrength.OnePair;
            }
        }
        if ((counts.Length == 5) &&
            (counts[0] == 1) && 
            (counts[1] == 1) && 
            (counts[2] == 1) && 
            (counts[3] == 1) && 
            (counts[4] == 1)) 
        {
            if (jokers == 1)
            {
                return HandStrength.OnePair;
            }
            else
            {
                return HandStrength.HighCard;
            }
        }
            
        throw new Exception("Cards did not match any defined hand.");
    }
}