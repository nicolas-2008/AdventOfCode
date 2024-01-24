namespace AdventOfCode;

public static class Day7TaskSolver
{
    private const char JOKER = 'J';

    private static readonly Dictionary<char, int> CardStrength = new()
    {
        { '2', 1 },
        { '3', 2 },
        { '4', 3 },
        { '5', 4 },
        { '6', 5 },
        { '7', 6 },
        { '8', 7 },
        { '9', 8 },
        { 'T', 9 },
        { 'J', 10 },
        { 'Q', 11 },
        { 'K', 12 },
        { 'A', 13 }
    };

    private static readonly Dictionary<char, int> CardStrengthWithJokerRule = new()
    {
        { 'J', 0 },
        { '2', 1 },
        { '3', 2 },
        { '4', 3 },
        { '5', 4 },
        { '6', 5 },
        { '7', 6 },
        { '8', 7 },
        { '9', 8 },
        { 'T', 9 },
        { 'Q', 11 },
        { 'K', 12 },
        { 'A', 13 }
    };

    private static string GetTestInput()
    {
        return @"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483";
    }

    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(7);
        // var input = GetTestInput();
        var hands = ParseInput(input);
        var result = hands
            .Select(h => (hand: h, type: GetHandType(h)))
            .OrderBy(x => x, Comparer<(Hand, HandType)>.Create((x, y) => CompareHandStrength(x, y)))
            .Select((t, i) => (hand: t.hand, rank: i + 1))
            .Sum(t => t.hand.Bid * t.rank);
        
        Console.WriteLine($"Total winnings is {result}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(7);
        // var input = GetTestInput();
        var hands = ParseInput(input);
        var result = hands
            .Select(h => (hand: h, type: GetHandType(h, useJokerRule: true)))
            .OrderBy(x => x, Comparer<(Hand, HandType)>.Create((x, y) => CompareHandStrength(x, y, useJokerRule: true)))
            .Select((t, i) => (hand: t.hand, rank: i + 1))
            .Sum(t => t.hand.Bid * t.rank);
        
        Console.WriteLine($"Total winnings is {result}");
    }


    private static IEnumerable<Hand> ParseInput(string input)
    {
        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var (cardsString, bidString) = line.Split(' ', 2);
            yield return new Hand
            {
                Cards = cardsString,
                Bid = long.Parse(bidString)
            };
        }
    }

    private static HandType GetHandType(Hand hand, bool useJokerRule = false)
    {
        var cards = hand.Cards;

        if (useJokerRule && cards.Contains(JOKER) && cards.Any(c => c != JOKER))
        {
            var mostFrequentCard = cards
                .Where(c => c != JOKER)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .First();

            cards = cards.Replace(JOKER, mostFrequentCard.Key);
        }
        
        var cardCounts = cards
            .GroupBy(card => card)
            .ToLookup(
                cardGroup => cardGroup.Count(), 
                cardGroup => cardGroup.Key);

        var type = cardCounts switch
        {
            _ when cardCounts.Contains(5) => HandType.FiveOfKind,
            _ when cardCounts.Contains(4) => HandType.FourOfKind,
            _ when cardCounts.Contains(3) =>
                cardCounts switch
                {
                    _ when cardCounts.Contains(2) => HandType.FullHouse,
                    _ => HandType.ThreeOfKind
                },
            _ when cardCounts.Contains(2) =>
                cardCounts switch
                {
                    _ when cardCounts[2].Count() == 2 => HandType.TwoPair,
                    _ => HandType.OnePair
                },
            _ => HandType.HighCard
        };

        return type;
    }

    private static int CompareHandStrength((Hand hand, HandType type) x, (Hand hand, HandType type) y, bool useJokerRule = false)
    {
        var result = x.type.CompareTo(y.type);

        if (result == 0)
        {
            var cardStrength = useJokerRule ? CardStrengthWithJokerRule : CardStrength;
            result = x.hand.Cards.Zip(y.hand.Cards)
                .Where(t => t.First != t.Second)
                .Select(t => cardStrength[t.First].CompareTo(cardStrength[t.Second]))
                .FirstOrDefault();
        }

        return result;
    }
}

internal enum HandType
{
    HighCard = 1,
    OnePair = 2,
    TwoPair = 3,
    ThreeOfKind = 4,
    FullHouse = 5,
    FourOfKind = 6,
    FiveOfKind = 7
}

internal record Hand
{
    public string Cards { get; init; }
    public long Bid { get; init; }
}