using AdventOfCode.Solutions;

namespace AdventOfCode.Support;

public static class YearBuilder
{
    public static async Task SetupNewYear()
    {
        CreateEmptyInputs();
        await CreateEmptySolutions();
    }

    public static void CreateEmptyInputs()
    {
        try
        {
            string path = YearFileManager.InputPath();

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            for(int day = 1; day <= Constants.AdventOfCodeDayCount; day++)
            {
                var part1 = YearFileManager.InputPath(day, part: 1);
                var part2 = YearFileManager.InputPath(day, part: 2);

                if (!File.Exists(part1)) File.Create(part1);
                if (!File.Exists(part2)) File.Create(part2);
            }
        }
        catch (Exception ex)
        {
            Logging.Report(exception: ex);
        }
    }

    public static async Task CreateEmptySolutions()
    {
        try
        {
            string path = YearFileManager.SolutionPath();

            if (!Directory.Exists(path)) 
            {
                // If there is no directory, there is no default implementation of a day, so return.
                Directory.CreateDirectory(path);
                return; 
            }

            var defaultImplementationName = nameof(DayXX);
            var defaultImplementationPath = Path.Combine(path, $"{defaultImplementationName}.cs");

            if (!File.Exists(defaultImplementationPath)) 
            {
                // If there is no default implementation of a day, there is no default implementation of a day, so return.
                return;
            }    

            var defaultImplementationText = File.ReadAllText(defaultImplementationPath);

            for(int day = 1; day <= Constants.AdventOfCodeDayCount; day++)
            {
                var dayName = $"Day{day:00}";
                var dayPath = Path.Combine(path, $"{dayName}.cs");
                var dayText = defaultImplementationText.Replace(defaultImplementationName, dayName);

                if (!File.Exists(dayPath)) 
                { 
                    using StreamWriter outputFile = new(dayPath);
                    
                    await outputFile.WriteLineAsync(dayText);

                    outputFile.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Report(exception: ex);
        }
    }
}