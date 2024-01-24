using MoreLinq;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace AdventOfCode;

public static class Day5TaskSolver
{
    private const string CATEGORY_SEED = "seed";
    private const string CATEGORY_LOCATION = "location";

    private static string GetTestInput()
    {
        string input;
        input = @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4";
        return input;
    }

    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(5);
        // var input = GetTestInput();

        var almanac = ParseInput(input);
        var seedToLocationMapChain = BuildSourceToDestinationMapChain(
            almanac.Maps,
            CATEGORY_SEED,
            CATEGORY_LOCATION);

        var lowestLocation = almanac.Seeds
            .Select(x => MapToDestination(x, seedToLocationMapChain))
            .Min();

        Console.WriteLine($"The lowest location is {lowestLocation}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(5);
        // var input = GetTestInput();

        var almanac = ParseInputV2(input);
        var seedToLocationMapChain = BuildSourceToDestinationMapChain(
            almanac.Maps,
            CATEGORY_SEED,
            CATEGORY_LOCATION);

        // Takes too long to evaluate the result
        // var lowestLocation = almanac.Seeds
        //     .SelectMany(r => EnumerateValues(r.Start, r.Length))
        //     .Select(x => MapValue(x, seedToLocationMapChain))
        //     .Min();

        var lowestLocation = almanac.Seeds
            .Select(x => new Range(x.Start, x.Start + x.Length - 1))
            .SelectMany(x => MapToDestination(x, seedToLocationMapChain))
            .Select(x => x.Start)
            .Min();

        Console.WriteLine($"The lowest location is {lowestLocation}");
    }

    private static long MapToDestination(long source, IReadOnlyList<AlmanacMap> mapChain)
    {
        var result = source;
        foreach (var map in mapChain)
        {
            var range = BinarySearchSourceRange(map.SortedRanges, result);
            if (range is not null)
            {
                var resultOffset = result - range.SourceRangeStart;
                result = range.DestinationRangeStart + resultOffset;
            }
        }

        return result;
    }

    private static IReadOnlyList<Range> MapToDestination(Range source, IReadOnlyList<AlmanacMap> mapChain)
    {
        var result = new[] { source };
        foreach (var map in mapChain)
        {
            result = result
                .SelectMany(r => MapToDestination(r, map))
                .OrderBy(r => r.Start)
                .ToArray();
        }

        return result;
    }

    private static IReadOnlyList<Range> MapToDestination(Range source, AlmanacMap almanacMap)
    {
        var result = new List<Range>();

        var mappedRanges = almanacMap.SortedRanges
            .Select(x => (mapRange: x, sourceIntersection: source.Intersect(x.SourceRangeStart, x.SourceRangeEnd)))
            .Where(x => x.sourceIntersection is not null)
            .ToList();

        // No match
        // Source range :      ssss
        // Almanac map  :  mm      mmmmm
        // Result       :      ssss
        if (mappedRanges.Count == 0)
        {
            result.Add(source);
        }
        // Partial or full match
        // Source range :      sssssss
        // Map ranges   :  mm    mm m  mmmmm  
        // Result       :      ssmmsms
        else
        {
            // Append gap at the beginning
            // Source range :  sssssss
            // Map ranges   :    mmm
            // Result       :  ss.....
            var firstMappedRange = mappedRanges.First();
            if (firstMappedRange.sourceIntersection.Start > source.Start)
            {
                var leftGap = new Range(source.Start, firstMappedRange.sourceIntersection.Start - 1);
                result.Add(leftGap);
            }

            for (int i = 0; i < mappedRanges.Count; i++)
            {
                // Append mapped ranges
                // Source range :  sssssssss
                // Map ranges   :    mm  mm  mmm
                // Result       :  ..mm..mm.
                var mappedRange = mappedRanges[i];
                var destinationRange = mappedRange.sourceIntersection.Shift(mappedRange.mapRange.DestinationOffset);
                result.Add(destinationRange);

                // Append gaps in the middle
                // Source range :  sssssssssssss
                // Map ranges   :    mm  mm mmm
                // Result       :  ....ss..s....
                var nextMappedRange = i < mappedRanges.Count - 1 ? mappedRanges[i + 1] : default;
                if (nextMappedRange != default && nextMappedRange.sourceIntersection.Start < mappedRange.sourceIntersection.End + 1)
                {
                    var middleGap = new Range(mappedRange.sourceIntersection.End + 1, nextMappedRange.sourceIntersection.Start - 1);
                    result.Add(middleGap);
                }
            }

            // Append gap at the end
            // Source range :    ssss
            // Map ranges   :  mmmm
            // Result       :    ..ss
            var lastMappedRange = mappedRanges.Last();
            if (lastMappedRange.sourceIntersection.End < source.End)
            {
                var rightGap = new Range(lastMappedRange.sourceIntersection.End + 1, source.End);
                result.Add(rightGap);
            }
        }

        return result;
    }

    private static IReadOnlyList<AlmanacMap> BuildSourceToDestinationMapChain(IReadOnlyList<AlmanacMap> maps, string sourceCategory, string destinationCategory)
    {
        var mapsDictionary = maps.ToDictionary(x => x.SourceCategory);
        var chain = new List<AlmanacMap>();
        var current = mapsDictionary.GetValueOrDefault(sourceCategory)
                      ?? throw new ArgumentOutOfRangeException($"Unable to find map with source category '{sourceCategory}'");
        chain.Add(current);
        while (current.DestinationCategory != destinationCategory)
        {
            current = mapsDictionary.GetValueOrDefault(current.DestinationCategory)
                      ?? throw new ArgumentOutOfRangeException($"Unable to find map with source category '{current.DestinationCategory}'");
            chain.Add(current);
        }

        return chain;
    }

    private static Almanac ParseInput(string input)
    {
        var (seedsString, mapsString) = input.Split('\n', 2);
        var (_, seedValuesString) = seedsString.Split(':', 2);

        var seeds = seedValuesString
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        var mapsStrings = mapsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        var maps = new List<AlmanacMap>();
        foreach (var mapString in mapsStrings)
        {
            var (mapDescriptorString, rangesString) = mapString.Split(" map:", 2, StringSplitOptions.TrimEntries);
            var (mapSourceCategory, mapDestinationCategory) = mapDescriptorString.Split("-to-");

            var rangesStrings = rangesString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var ranges = new List<AlmanacMapRange>();
            foreach (var rangeString in rangesStrings)
            {
                var (destinationRangeStart, sourceRangeStart, rangeLength) = rangeString.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                var range = new AlmanacMapRange
                {
                    SourceRangeStart = long.Parse(sourceRangeStart),
                    DestinationRangeStart = long.Parse(destinationRangeStart),
                    Length = long.Parse(rangeLength)
                };
                ranges.Add(range);
            }

            var map = new AlmanacMap
            {
                SourceCategory = mapSourceCategory,
                DestinationCategory = mapDestinationCategory,
                SortedRanges = ranges.OrderBy(x => x.SourceRangeStart).ToList()
            };

            maps.Add(map);
        }

        var almanac = new Almanac
        {
            Seeds = seeds,
            Maps = maps
        };

        return almanac;
    }

    private static AlmanacV2 ParseInputV2(string input)
    {
        var (seedsString, mapsString) = input.Split('\n', 2);
        var (_, seedValuesString) = seedsString.Split(':', 2);

        var seeds = seedValuesString
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Batch(2)
            .Select(rangeDefinition => new SeedRange
            {
                Start = long.Parse(rangeDefinition[0]),
                Length = long.Parse(rangeDefinition[1])
            })
            .ToArray();

        var mapsStrings = mapsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        var maps = new List<AlmanacMap>();
        foreach (var mapString in mapsStrings)
        {
            var (mapDescriptorString, rangesString) = mapString.Split(" map:", 2, StringSplitOptions.TrimEntries);
            var (mapSourceCategory, mapDestinationCategory) = mapDescriptorString.Split("-to-");

            var rangesStrings = rangesString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var ranges = new List<AlmanacMapRange>();
            foreach (var rangeString in rangesStrings)
            {
                var (destinationRangeStart, sourceRangeStart, rangeLength) = rangeString.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                var range = new AlmanacMapRange
                {
                    SourceRangeStart = long.Parse(sourceRangeStart),
                    DestinationRangeStart = long.Parse(destinationRangeStart),
                    Length = long.Parse(rangeLength)
                };
                ranges.Add(range);
            }

            var map = new AlmanacMap
            {
                SourceCategory = mapSourceCategory,
                DestinationCategory = mapDestinationCategory,
                SortedRanges = ranges.OrderBy(x => x.SourceRangeStart).ToList()
            };

            maps.Add(map);
        }

        var almanac = new AlmanacV2
        {
            Seeds = seeds,
            Maps = maps
        };

        return almanac;
    }

    private static AlmanacMapRange? BinarySearchSourceRange(IReadOnlyList<AlmanacMapRange> rangesOrderedBySource, long value)
    {
        static bool ValueIsAtRight(long value, AlmanacMapRange range)
        {
            return range.SourceRangeEnd < value;
        }

        static bool ValueIsAtLeft(long value, AlmanacMapRange range)
        {
            return range.SourceRangeStart > value;
        }

        int left = 0;
        int right = rangesOrderedBySource.Count - 1;

        do
        {
            int middle = left + (right - left) / 2;
            var range = rangesOrderedBySource[middle];

            if (ValueIsAtRight(value, range))
            {
                left = middle + 1;
            }
            else if (ValueIsAtLeft(value, range))
            {
                right = middle - 1;
            }
            else // value is in range
            {
                return range;
            }
        } while (left <= right);

        return null;
    }

    private static IEnumerable<long> EnumerateValues(long start, long count)
    {
        for (long i = 0; i < count; i++)
        {
            yield return start + i;
        }
    }
}

internal record Almanac
{
    public required IReadOnlyList<long> Seeds { get; init; }
    public required IReadOnlyList<AlmanacMap> Maps { get; init; }
}

internal record AlmanacV2
{
    public required IReadOnlyList<SeedRange> Seeds { get; init; }
    public required IReadOnlyList<AlmanacMap> Maps { get; init; }
}

internal record AlmanacMap
{
    public required string SourceCategory { get; init; }
    public required string DestinationCategory { get; init; }
    public required List<AlmanacMapRange> SortedRanges { get; init; }
}

internal record AlmanacMapRange
{
    public long SourceRangeStart { get; init; }
    public long SourceRangeEnd => this.SourceRangeStart + this.Length - 1;
    public long DestinationRangeStart { get; init; }
    public long DestinationRangeEnd => DestinationRangeStart + this.Length - 1;
    public long Length { get; init; }
    public long DestinationOffset => this.DestinationRangeStart - this.SourceRangeStart;
}

internal record SeedRange
{
    public long Start { get; init; }
    public long Length { get; init; }
}

internal record Range
{
    public long Start { get; }
    public long End { get; }

    public Range(long start, long end)
    {
        Start = start;
        End = end;
    }

    public Range? Intersect(long start, long end)
    {
        var intersectionStart = Math.Max(this.Start, start);
        var intersectionEnd = Math.Min(this.End, end);

        if (intersectionStart <= intersectionEnd)
        {
            return new Range(intersectionStart, intersectionEnd);
        }

        return null;
    }

    public Range Shift(long offset)
    {
        return new Range(this.Start + offset, this.End + offset);
    }
}