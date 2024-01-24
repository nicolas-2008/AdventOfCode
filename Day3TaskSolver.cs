using System.Text.RegularExpressions;

namespace AdventOfCode;

public static class Day3TaskSolver
{
    private static readonly Regex LineParser = new Regex(@"(?<number>\d+)|(?<symbol>[^0-9.])", RegexOptions.Compiled);

    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(3);

        // input =
        //     "467..114.." + "\n" +
        //     "...*......" + "\n" +
        //     "..35..633." + "\n" +
        //     "......#..." + "\n" +
        //     "617*......" + "\n" +
        //     ".....+.58." + "\n" +
        //     "..592....." + "\n" +
        //     "......755." + "\n" +
        //     "...$.*...." + "\n" +
        //     ".664.598..";

        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var schemaObjects = ParseLines(lines);

        var numberParts = schemaObjects
            .Where(x => x.Type == SchemaObjectType.Number)
            .Where(x => IsAdjacentToSymbol(x, schemaObjects));

        var sum = numberParts.Sum(x => int.Parse(x.Value));
        Console.WriteLine($"Sum of part numbers {sum}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(3);

        // input =
        //     "467..114.." + "\n" +
        //     "...*......" + "\n" +
        //     "..35..633." + "\n" +
        //     "......#..." + "\n" +
        //     "617*......" + "\n" +
        //     ".....+.58." + "\n" +
        //     "..592....." + "\n" +
        //     "......755." + "\n" +
        //     "...$.*...." + "\n" +
        //     ".664.598..";

        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var schemaObjects = ParseLines(lines);

        var gears = schemaObjects
            .Where(x => x is { Type: SchemaObjectType.Symbol, Value: "*" })
            .Select(x => (symbol: x, partNumbers: GetAdjacentNumbers(x, schemaObjects)))
            .Where(x => x.partNumbers.Count == 2)
            .Select(x => (gear: x.symbol, part1: x.partNumbers[0], part2: x.partNumbers[1]));

        var gearRatios = gears.Select(x => int.Parse(x.part1.Value) * int.Parse(x.part2.Value));
        var sum = gearRatios.Sum();

        Console.WriteLine($"Sum of part numbers {sum}");
    }

    private static IReadOnlyList<SchemaObject> GetAdjacentNumbers(SchemaObject symbol, IReadOnlyCollection<SchemaObject> schemaObjects)
    {
        return schemaObjects
            .Where(x => x.Type == SchemaObjectType.Number)
            .Where(x =>
                x.LineIndex == symbol.LineIndex + 1 ||
                x.LineIndex == symbol.LineIndex - 1 ||
                x.LineIndex == symbol.LineIndex)
            .Where(x =>
                x.Index + x.Value.Length - 1 >= symbol.Index - 1 &&
                x.Index <= symbol.Index + 1)
            .ToArray();
    }

    private static bool IsAdjacentToSymbol(SchemaObject number, IReadOnlyCollection<SchemaObject> schemaObjects)
    {
        return schemaObjects
            .Where(x => x.Type == SchemaObjectType.Symbol)
            .Where(x =>
                x.LineIndex == number.LineIndex + 1 ||
                x.LineIndex == number.LineIndex - 1 ||
                x.LineIndex == number.LineIndex)
            .Where(x =>
                x.Index >= number.Index - 1 &&
                x.Index <= number.Index + number.Value.Length)
            .Any();
    }

    private static IReadOnlyCollection<SchemaObject> ParseLines(string[] lines)
    {
        var result = new List<SchemaObject>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var regExResult = LineParser.Matches(line);

            foreach (Match match in regExResult)
            {
                if (match.Groups.TryGetValue("number", out var numberMatch) && numberMatch.Success)
                {
                    var schemaObject = new SchemaObject()
                    {
                        Type = SchemaObjectType.Number,
                        Index = numberMatch.Index,
                        Value = numberMatch.Value,
                        LineIndex = i
                    };
                    result.Add(schemaObject);
                }
                else if (match.Groups.TryGetValue("symbol", out var symbolMatch) && symbolMatch.Success)
                {
                    var schemaObject = new SchemaObject()
                    {
                        Type = SchemaObjectType.Symbol,
                        Index = symbolMatch.Index,
                        Value = symbolMatch.Value,
                        LineIndex = i
                    };
                    result.Add(schemaObject);
                }
                else
                {
                    throw new ApplicationException($"Unrecognized RegEx result {match}");
                }
            }
        }


        return result;
    }
}

public sealed record SchemaObject
{
    public SchemaObjectType Type { get; init; }
    public required string Value { get; init; }
    public int Index { get; init; }
    public int LineIndex { get; init; }
}

public enum SchemaObjectType
{
    Symbol,
    Number
}