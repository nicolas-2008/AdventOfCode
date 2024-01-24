using System.Diagnostics.CodeAnalysis;
using MoreLinq;

namespace AdventOfCode;

public static class Day6TaskSolver
{
    private static string GetTestInput()
    {
        return @"Time:      7  15   30
Distance:  9  40  200";
    }

    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider(); 
        var input = await inputProvider.GetStringAsync(6);
        // var input = GetTestInput();
        var races = ParseInput(input);
        var timeHoldEvalResults = races
            .Select(EvaluateTimeHoldToBeatTheRecord)
            .Where(x => x is not null);
        var timeHoldWaysToWinCount = timeHoldEvalResults
            .Select(x => x.Max - x.Min + 1);
        var multiplied = timeHoldWaysToWinCount.Aggregate(1L, (acc, value) => acc * value);
        Console.WriteLine($"Multiplication of ways to win count is {multiplied}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider(); 
        var input = await inputProvider.GetStringAsync(6);
        // var input = GetTestInput();
        var race = ParseInputV2(input);
        var timeHoldEvalResult = EvaluateTimeHoldToBeatTheRecord(race);
        var timeHoldWaysToWinCount = timeHoldEvalResult is not null 
            ? timeHoldEvalResult.Max - timeHoldEvalResult.Min + 1 
            : 0;
        Console.WriteLine($"Ways to win count is {timeHoldWaysToWinCount}");
    }

    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static TimeHoldEvalResult? EvaluateTimeHoldToBeatTheRecord(Race race)
    {
        /*
           Math:
         
           timeHold + timeTravel = timeTotal
           velocity * timeTravel > distance
           velocity = timeHold
           
           =>
           timeTravel = timeTotal - timeHold
           timeHold * timeTravel > distance
           
           =>
           timeHold * (timeTotal - timeHold) > distance
           
           =>
           -1 * timeHold^2 + timeTotal * timeHold - distance > 0  // quadratic inequity
           
           D = timeTotal^2 - 4*(-1)*(-distance)
           
           if (D > 0)
             timeHoldMin = (-timeTotal+sqrt(D))/2*(-1)
             timeHoldMax = (-timeTotal-sqrt(D))/2*(-1)
             resultRange = (timeHoldMin, timeHoldMax)  // exclusive range
           else 
             solution doesn't exist
         */
        
        var D = race.Time * race.Time - 4 * race.Distance;

        if (D > 0)
        {
            var timeHoldMin = (-race.Time + Math.Sqrt(D)) / -2;
            var timeHoldMax = (-race.Time - Math.Sqrt(D)) / -2;

            if (timeHoldMin < 0 || timeHoldMax < 0)
            {
                return null;
            }

            var timeHoldMinIntToBeat = double.IsInteger(timeHoldMin) ? (long) timeHoldMin + 1 : (long) Math.Ceiling(timeHoldMin);
            var timeHoldMaxIntToBeat = double.IsInteger(timeHoldMax) ? (long) timeHoldMax - 1 : (long) Math.Floor(timeHoldMax);

            if (timeHoldMinIntToBeat > timeHoldMaxIntToBeat)
            {
                return null;
            }

            return new TimeHoldEvalResult
            {
                Min = timeHoldMinIntToBeat,
                Max = timeHoldMaxIntToBeat
            };
        }
        else
        {
            return null;
        }
    }

    private static IEnumerable<Race> ParseInput(string input)
    {
        var (timeString, distanceString) = input.Split('\n', 2);
        
        var (_, timeValuesString) = timeString.Split(':', 2);
        var times = timeValuesString
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse);

        var (_, distanceValuesString) = distanceString.Split(':', 2);
        var distances = distanceValuesString
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse);

        var races = times.Zip(distances).Select(x => new Race { Time = x.First, Distance = x.Second });

        return races;
    }
    
    private static Race ParseInputV2(string input)
    {
        var (timeString, distanceString) = input.Split('\n', 2);
        
        var (_, timeValuesString) = timeString.Split(':', 2);
        var timeValueWithoutSpaces = timeValuesString.Replace(" ", "");
        var time = long.Parse(timeValueWithoutSpaces);

        var (_, distanceValuesString) = distanceString.Split(':', 2);
        var distanceValueWithoutSpaces = distanceValuesString.Replace(" ", "");
        var distance = long.Parse(distanceValueWithoutSpaces);

        return new Race
        {
            Time = time,
            Distance = distance
        };
    }

}

internal record Race
{
    public long Time { get; init; }
    public long Distance { get; init; }
}

internal record TimeHoldEvalResult 
{
    public long Min { get; init; }
    public long Max { get; init; }
}






