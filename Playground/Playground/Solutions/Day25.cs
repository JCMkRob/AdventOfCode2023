using System.Data;
using AdventOfCode.Support;

namespace AdventOfCode.Solutions;

public static class Day25
{
    [Example(solver: nameof(PartOne), solution: -1)]
    public static readonly string PartOneExample = 
            """
            
            """;

    [Example(solver: nameof(PartTwo), solution: -1)]
    public static readonly string PartTwoExample = 
            """

            """;

    [Solution(part: 1)]
    public static double PartOne(IEnumerable<string> input)
    { 
        string incidentTypePath = @"c:\Users\Joshua\Downloads\IncidentTypeAssociatedItems.csv";
        string personsInvolvedPath = @"c:\Users\Joshua\Downloads\ReportsPersonsInvolvedAssociatedItems.csv";
        string casualAnalysisPath = @"c:\Users\Joshua\Downloads\ReportsCasualAnalysisAssociatedItems.csv";
        string actionsPath = @"c:\Users\Joshua\Downloads\Actions.csv";
        
        var incidentType = Map(incidentTypePath, "ReportIDLookup", "Type of Incident");
        var personsInvolved = Map(personsInvolvedPath, "ReportIDLookup", "Full Name");
        var casualAnalysis = Map(casualAnalysisPath, "ReportIDLookup", "DescriptionReadOnly");
        var actions = Map(actionsPath, "Parent Item", "Action");

        string reportsPath = @"c:\Users\Joshua\Downloads\Reports.csv";

        var allLines = File.ReadAllLines(reportsPath);

        var mapping = allLines[0].Split(',').Select((key, index) => (key, index)).ToDictionary();

        var answers = new Dictionary<string, string>();

        List<string> result = [allLines[0]];

        for(int i = 1; i < allLines.Length; i++)
        {
            var line = allLines[i].Split(',').Select(s => GetLiteral(s)).ToList();
            Console.WriteLine(line.Count());

            var id = GetValue(line, mapping, "Report ID");
            
            if (incidentType.TryGetValue(id, out var incident))
            {
                line[mapping["Incident Type Display"]] = WrapLiteral(incident);
            }
            if (personsInvolved.TryGetValue(id, out var involved))
            {
                line[mapping["Involved Persons Display"]] = WrapLiteral(involved);
            }
            if (casualAnalysis.TryGetValue(id, out var causal))
            {
                line[mapping["Causal Analysis Display"]] = WrapLiteral(causal);
            }
            if (actions.TryGetValue(id, out var act))
            {
                line[mapping["Corrective Actions Display"]] = WrapLiteral(act);
            }

            result.Add(string.Join(",", line));
        }



        string output = @"c:\Users\Joshua\Downloads\output.csv";

        if (File.Exists(output)) File.Delete(output);

        File.WriteAllText(output, string.Join("\n", result));

        return 0;
    }

    private static Dictionary<string, string> Map(string path, string id, string value)
    {
        var allLines = File.ReadAllLines(path);

        var mapping = allLines[0].Split(',').Select((key, index) => (key, index)).ToDictionary();

        var answers = new Dictionary<string, string>();

        for(int i = 1; i < allLines.Length; i++)
        {
            var line = allLines[i].Split(',');

            CommaAdd(answers, GetValue(line, mapping, id), GetValue(line, mapping, value));
        }

        return answers;
    }

    

    private static void CommaAdd(this Dictionary<string, string> dict, string source, string add)
    {
        if (dict.ContainsKey(source))
        {
            dict[source] += "\n" + add;
        }
        else 
        {
            dict.Add(source, add);
        }
    }
    private static string GetValue(string[] sArray, Dictionary<string, int> dict, string key)
    {
        return GetLiteral(sArray[dict[key]]);
    }

    private static string GetValue(List<string> sArray, Dictionary<string, int> dict, string key)
    {
        return GetLiteral(sArray[dict[key]]);
    }

    private static string GetLiteral(string s) => s.Replace("\\", "").Replace("\"", "");
    private static string WrapLiteral(string s) => "\"" + s + "\"";

    [Solution(part: 2)]
    public static double PartTwo(IEnumerable<string> input)
    {
        return 0;
    }
}
