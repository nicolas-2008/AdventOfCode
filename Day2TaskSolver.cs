namespace AdventOfCode;

public static class Day2TaskSolver
{
    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(2);
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // lines = new[]
        // {
        //     "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
        //     "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
        //     "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
        //     "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
        //     "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
        // };

        var bag = new CubeSet { { "red", 12 }, { "green", 13 }, { "blue", 14 } };
        var games = ParseLines(lines);
        var possibleGames = games.Where(game => game.CubeSets.All(gameCubeSet => gameCubeSet.IsSubsetOf(bag)));
        var sumOfPossibleGameIds = possibleGames.Sum(game => game.Id);

        Console.WriteLine($"Sum is {sumOfPossibleGameIds}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(2);
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // lines = new[]
        // {
        //     "Game 1: 3 blue, 4 red; 1 red, 2 blue",
        //     "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
        //     "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
        //     "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
        //     "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
        // };
        
        var games = ParseLines(lines);
        var cubesPowerSum = 0;

        foreach (var game in games)
        {
            var requiredCubesBag = new CubeSet();
            foreach (var cubeSet in game.CubeSets)
            {
                foreach (var cubeItem in cubeSet)
                {
                    if (cubeItem.Value > requiredCubesBag.GetValueOrDefault(cubeItem.Key))
                    {
                        requiredCubesBag[cubeItem.Key] = cubeItem.Value;
                    }
                }
            }

            var cubesPower = requiredCubesBag.Values.Aggregate(1, (acc, value) => acc * value);
            cubesPowerSum += cubesPower;
            Console.WriteLine($"Game {game.Id} power is {cubesPower}");
        }
        Console.WriteLine($"Sum of the power is {cubesPowerSum}");
    }
    
    private static IEnumerable<Game> ParseLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var (gameString, cubeSetsString) = line.Split(':', 2);

            var gameId = int.Parse(gameString[5..]);
            var cubeSets = new List<CubeSet>();
            foreach (var cubeSetString in cubeSetsString.Split(';'))
            {
                var cubeSet = new CubeSet();
                foreach (var cubeString in cubeSetString.Split(','))
                {
                    var (valueString, colorString) = cubeString.Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    
                    var value = int.Parse(valueString);
                    var color = colorString;
                    cubeSet.Add(color, value);
                }

                cubeSets.Add(cubeSet);
            }

            yield return new Game { Id = gameId, CubeSets = cubeSets };
        }
    }
}

internal record Game
{
    public required int Id { get; init; }
    public required IReadOnlyCollection<CubeSet> CubeSets { get; init; }
}

internal class CubeSet : Dictionary<string, int>
{
    public bool IsSubsetOf(CubeSet otherCubeSet)
    {
        return this.All(cube => otherCubeSet.TryGetValue(cube.Key, out var value) && value >= cube.Value);
    }
}