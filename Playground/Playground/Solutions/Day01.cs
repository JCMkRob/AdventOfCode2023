using System.Security.Cryptography;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day01
{
    [Example(solver: nameof(PartOne), solution: 142)]
    public static readonly string PartOneExample = 
            """
            1abc2
            pqr3stu8vwx
            a1b2c3d4e5f
            treb7uchet
            """;

    [Example(solver: nameof(PartTwo), solution: 281)]
    public static readonly string PartTwoExample = 
            """
            two1nine
            eightwothree
            abcone2threexyz
            xtwone3four
            4nineeightseven2
            zoneight234
            7pqrstsixteen
            """; 

    public static double Example()
    {
        return Manager.ReadLines(day: 1, part: 2)
            .Select(line => line.ToDoubleIncludeWords())
            .Sum();
    }

    public static double PartOne()
    {
        return Manager.ReadLines(day: 1, part: 1)
            .Select(line => line.ToDouble())
            .Sum();
    }
    public static double PartOne(string s)
    {
        return s.Split("\n").Select(line => line.ToDouble()).Sum();
    }

    public static double PartTwo()
    {
        return Manager.ReadLines(day: 1, part: 2)
            .Select(line => line.ToDoubleIncludeWords())
            .Sum();
    }

    public static double PartTwo(string s)
    {
        return s.Split("\n").Select(line => line.ToDoubleIncludeWords()).Sum();
    }

    public static double PartTwo(IEnumerable<string> s)
    {
        return s.Select(line => line.ToDoubleIncludeWords()).Sum();
    }

    private static double ToDouble(this string line)
    {
        var numbers = line.Where(c => char.IsNumber(c));
        var firtsAndLast = $"{numbers.First()}{numbers.Last()}";
        var twoDigitNumber = double.Parse(firtsAndLast);

        return twoDigitNumber;
    }

    private static double ToDoubleIncludeWords(this string line)
    {
        List<double> numbers = [];

        for (int i = 0; i < line.Length; i++)
        {
            if (Check(line[i..]) is double d)
            {
                numbers.Add(d);
            }
        }
        
        var firtsAndLast = $"{numbers.First()}{numbers.Last()}";
        var twoDigitNumber = double.Parse(firtsAndLast);

        return twoDigitNumber;
    }

    private static double? Check(string s) =>
        s.Length switch
        {
            > 4 => CheckFiveCharacters(s),
            4 => CheckFourCharacters(s),
            3 => CheckThreeCharacters(s),
            2 => CheckOneCharacter(s),
            1 => CheckOneCharacter(s),
            < 1 => null
        };

    private static double? CheckFiveCharacters(string s) =>
        s[0..5] switch
        {
            "three" => 3,
            "seven" => 7,
            "eight" => 8,
            _ => CheckFourCharacters(s),
        };
    
    private static double? CheckFourCharacters(string s) =>
        s[0..4] switch
        {
            "four" => 4,
            "five" => 5,
            "nine" => 9,
            _ => CheckThreeCharacters(s),
        };

    private static double? CheckThreeCharacters(string s) =>
        s[0..3] switch
        {
            "one" => 1,
            "two" => 2,
            "six" => 6,
            _ => CheckOneCharacter(s),
        };

    private static double? CheckOneCharacter(string s) => char.IsNumber(s[0]) ? double.Parse($"{s[0]}") : null;
}