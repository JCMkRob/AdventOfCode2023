// using AdventOfCode.Support;

// namespace AdventOfCode.Solutions;

// public static class Day12b
// {
//     private class DoubleContainer(double Value)
//     {
//         public double Value { get; } = Value;
//     }

//     private enum SpringState
//     {
//         Operational,
//         Damaged,
//         Unknown 
//     }

//     private class SpringRow(string s)
//     {
//         private Queue<SpringState> Springs { get; init; } = new Queue<SpringState>(s.Select(c => FromChar(c)));

//         private static SpringState FromChar(char c) => c switch
//         {
//             '.' => SpringState.Operational,
//             '#' => SpringState.Damaged,
//             '?' => SpringState.Unknown,
//             _ => throw new Exception(),
//         };

//         public bool Empty() => Springs.Count != 0;
//         public SpringState Next() => Springs.Dequeue();
//         public SpringState Peek() => Springs.Peek();
//     }

//     [Example(solver: nameof(PartOne), solution: 1)]
//     public static readonly string PartOneExampleOne = 
//         """
//         ???.### 1,1,3
//         """;

//     [Example(solver: nameof(PartOne), solution: 4)]

//     public static readonly string PartOneExampleTwo = 
//         """
//         .??..??...?##. 1,1,3
//         """;
//     [Example(solver: nameof(PartOne), solution: 1)]

//     public static readonly string PartOneExampleThree = 
//         """
//         ?#?#?#?#?#?#?#? 1,3,1,6
//         """;

//     [Example(solver: nameof(PartOne), solution: 1)]
//     public static readonly string PartOneExampleFour = 
//         """
//         ????.#...#... 4,1,1
//         """;

//     [Example(solver: nameof(PartOne), solution: 4)]
//     public static readonly string PartOneExampleFive = 
//         """
//         ????.######..#####. 1,6,5
//         """;

//     [Example(solver: nameof(PartOne), solution: 10)]
//     public static readonly string PartOneExampleSix = 
//         """
//         ?###???????? 3,2,1
//         """;

//     [Example(solver: nameof(PartOne), solution: 21)]
//     public static readonly string PartOneExampleSeven = 
//         """
//         ???.### 1,1,3
//         .??..??...?##. 1,1,3
//         ?#?#?#?#?#?#?#? 1,3,1,6
//         ????.#...#... 4,1,1
//         ????.######..#####. 1,6,5
//         ?###???????? 3,2,1
//         """;
            
//     [Example(solver: nameof(PartTwo), solution: 1)]
//     public static readonly string PartTwoExampleOne = 
//         """
//         ???.### 1,1,3
//         """;

//     [Example(solver: nameof(PartTwo), solution: 16384)]
//     public static readonly string PartTwoExampleTwo = 
//         """
//         .??..??...?##. 1,1,3
//         """;

//     [Example(solver: nameof(PartTwo), solution: 1)]
//     public static readonly string PartTwoExampleThree = 
//         """
//         ?#?#?#?#?#?#?#? 1,3,1,6
//         """;

//     [Example(solver: nameof(PartTwo), solution: 16)]
//     public static readonly string PartTwoExampleFour = 
//         """
//         ????.#...#... 4,1,1
//         """;

//     [Example(solver: nameof(PartTwo), solution: 2500)]
//     public static readonly string PartTwoExampleFive = 
//         """
//         ????.######..#####. 1,6,5
//         """;

//     [Example(solver: nameof(PartTwo), solution: 506250)]
//     public static readonly string PartTwoExampleSix = 
//         """
//         ?###???????? 3,2,1
//         """;

//     [Solution(part: 1)]
//     public static double PartOne(IEnumerable<string> input)
//     {


//         return 0;
//     }

//     [Solution(part: 2)]
//     public static double PartTwo(IEnumerable<string> input)
//     {
//         return 0;
//     }

//     public static void SSS(string s, int[] groups)
//     {
//         int group = groups[0];
        
//         s.Take(4);
//     }
//     //.?###????????. 3,2,1
//     public static void Match(string s, int size)
//     {
//         // 14
//         // 5 + 4 + 3 = 12
//         // 
//         var length = s.Length;
//         string substring = "";

//         byte question = 0;
//         byte octothorpe = 0;

//         foreach(char c in s)
//         {
//             substring += c;

//             if (c == '?') question++;
//             if (c == '#') octothorpe++;
//             if (c == '.') 
//             {
//                 question = 0;
//                 octothorpe = 0;
//             }
//             if (size < substring.Length)
//             {
//                 if (size <= (question + octothorpe))
//                 {

//                 }
//             }
//         }

//         if (length < size)
//     }

//     public static void Greed(string s, int size)
//     {
//         var length = s.Length;
//         string substring = "";

//         byte firstOctothorpe = 0;

//         foreach(char c in s)
//         {
//             substring += c;

//             if (c == '#' && (firstOctothorpe == 0)) octothorpe++;
//             if (c == '.') 
//             {
//                 question = 0;
//                 octothorpe = 0;
//             }
//             if (size < substring.Length)
//             {
//                 if (size <= (question + octothorpe))
//                 {

//                 }
//             }
//         }

//         if (length < size)
//     }

//     private static double GreedySwitch(string s, int size) => s switch
//     {
        
//     }

//     public static double Permutations(byte knownSize, byte requestedWidth)
//     {   

//     }

//     // just add periods to both ends dumby
//     public static double MinimumWidth(byte knownSize) => knownSize + 2;
// }
 
