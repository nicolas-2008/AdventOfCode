using System.Diagnostics.CodeAnalysis;
using MoreLinq;

namespace AdventOfCode;

public static class Day9TaskSolver
{
    private static string GetTestInput()
    {
        return @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45";
    }

    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(9);
        // var input = GetTestInput();
        var dataRows = ParseInput(input);
        var result = dataRows.Select(Extrapolate).Sum();
        
        Console.WriteLine($"Sum of extrapolation results is {result}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(9);
        // var input = GetTestInput();
        var dataRows = ParseInput(input);
        var result = dataRows.Select(ExtrapolateBack).Sum();
        
        Console.WriteLine($"Sum of extrapolation results is {result}");
    }

    private static long ExtrapolateBack(long[] valueSequence)
    {
        var firstValues = new List<long>();
        firstValues.Add(valueSequence.First());

        var iteratorSequence = valueSequence;
        while (iteratorSequence.Any(x => x != 0))
        {
            var nextSequence = new long[iteratorSequence.Length - 1];
            for (int i = 0; i < iteratorSequence.Length - 1; i++)
            {
                nextSequence[i] = iteratorSequence[i + 1] - iteratorSequence[i];
            }
            firstValues.Add(nextSequence.First());
            iteratorSequence = nextSequence;
        }

        var result = 0L;
        for (int i = firstValues.Count - 1; i >= 0; --i)
        {
            result = firstValues[i] - result;
        }

        return result;
    }

    private static long Extrapolate(long[] valueSequence)
    {
        var lastValues = new List<long>();
        lastValues.Add(valueSequence.Last());

        var iteratorSequence = valueSequence;
        while (iteratorSequence.Any(x => x != 0))
        {
            var nextSequence = new long[iteratorSequence.Length - 1];
            for (int i = 0; i < iteratorSequence.Length - 1; i++)
            {
                nextSequence[i] = iteratorSequence[i + 1] - iteratorSequence[i];
            }
            lastValues.Add(nextSequence.Last());
            iteratorSequence = nextSequence;
        }

        var result = 0L;
        for (int i = lastValues.Count - 1; i >= 0; --i)
        {
            result = lastValues[i] + result;
        }

        return result;
    }

    private static IEnumerable<long[]> ParseInput(string input)
    {
        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var valueSequence = line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToArray();

            yield return valueSequence;
        }
    }
}