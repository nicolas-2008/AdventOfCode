namespace AdventOfCode;

public static class Day4TaskSolver
{
    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(4);
//         input = @"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
// Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
// Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
// Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
// Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
// Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";

        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var cards = ParseCards(lines);
        var cardsMatchNumbers = cards
            .Select(c => (card: c, matchNumbers: c.OwnedNumbers.Intersect(c.WinningNumbers)));
        var cardsPoints = cardsMatchNumbers
            .Select(c => CalculateCardPoints(c.matchNumbers.Count()));
        var sumOfCardPoints = cardsPoints.Sum();

        Console.WriteLine($"Sum of card points is {sumOfCardPoints}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(4);
//         input = @"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
// Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
// Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
// Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
// Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
// Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";

        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var cards = ParseCards(lines).ToArray();
        var cardEvaluations = new CardEvaluation[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            cardEvaluations[i] = new CardEvaluation
            {
                Card = card,
                MatchedNumbersCount = card.OwnedNumbers.Intersect(card.WinningNumbers).Count(),
                CopiesCount = 1
            };
        }
        for (int i = 0; i < cards.Length; i++)
        {
            for (int j = 1; j <= cardEvaluations[i].MatchedNumbersCount && j < cards.Length; j++)
            {
                cardEvaluations[i + j].CopiesCount += cardEvaluations[i].CopiesCount;
            }
        }

        var totalCardCount = cardEvaluations.Sum(x => x.CopiesCount);
        Console.WriteLine($"Total card count is {totalCardCount}");
    }

    private static int CalculateCardPoints(int matchNumbersCount)
    {
        if (matchNumbersCount == 0)
        {
            return 0;
        }

        return GeometricSequence(1, 2, matchNumbersCount);
    }

    private static int GeometricSequence(int a1, int commonRatio, int n)
    {
        return a1 * (int) Math.Pow(commonRatio, n - 1);
    }

    private static IEnumerable<Card> ParseCards(string[] lines)
    {
        foreach (var line in lines)
        {
            var (cardString, numbersString) = line.Split(':', 2);
            var (winningNumbersString, ownedNumbersString) = numbersString.Split('|');

            var cardId = int.Parse(cardString[5..]);

            var winningNumbers = winningNumbersString
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            var ownedNumbers = ownedNumbersString
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            yield return new Card
            {
                CardId = cardId,
                OwnedNumbers = ownedNumbers,
                WinningNumbers = winningNumbers
            };
        }
    }

    internal record Card
    {
        public required int CardId { get; set; }
        public required IReadOnlyList<int> WinningNumbers { get; init; }
        public required IReadOnlyList<int> OwnedNumbers { get; init; }
    }

    internal record CardEvaluation
    {
        public required Card Card { get; init; }
        public required int MatchedNumbersCount { get; init; }
        public required int CopiesCount { get; set; }
    }
}