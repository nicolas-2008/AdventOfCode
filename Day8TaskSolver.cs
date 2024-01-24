namespace AdventOfCode;

public static class Day8TaskSolver
{
    private const char LEFT = 'L';
    private const char RIGHT = 'R';

    private static string GetTestInput()
    {
        return @"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)";
    }

    private static string GetTestInputV2()
    {
        return @"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)";
    }

    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(8);
        // var input = GetTestInput();

        var navigator = ParseInput(input);
        var result = Navigate(navigator, "AAA", "ZZZ").Count();
        Console.WriteLine($"Count of steps AAA => ZZZ: {result}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(8);
        // var input = GetTestInputV2();

        var navigator = ParseInput(input);
        var result = NavigateV2EvaluateStepsToEnd(navigator, 'A', 'Z');
        Console.WriteLine($"Count of steps **A => **Z: {result}");
    }

    private static IEnumerable<Node> Navigate(Navigator navigator, string startNodeId, string endNodeId)
    {
        IEnumerable<char> EnumerateInstructionsLoop(string instructions)
        {
            while (true)
            {
                foreach (var instruction in instructions)
                {
                    yield return instruction;
                }
            }
        }

        var currentNode = navigator.Nodes[startNodeId];
        foreach (var instruction in EnumerateInstructionsLoop(navigator.Instructions))
        {
            var nextNodeId = GetNextNodeId(instruction, currentNode);
            currentNode = navigator.Nodes[nextNodeId];
            yield return currentNode;

            if (currentNode.Id == endNodeId)
            {
                break;
            }
        }
    }

    private static long NavigateV2EvaluateStepsToEnd(Navigator navigator, char startNodeIdSuffix, char endNodeIdSuffix)
    {
        IEnumerable<char> EnumerateInstructionsLoop(string instructions)
        {
            while (true)
            {
                foreach (var instruction in instructions)
                {
                    yield return instruction;
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }

        var nodes = navigator.Nodes.Values.Where(x => x.Id.EndsWith(startNodeIdSuffix));
        var nodesStepsToEnd = new List<long>();
        foreach (var node in nodes)
        {
            var count = 0;
            var nodeIterator = node;
            foreach (var instruction in EnumerateInstructionsLoop(navigator.Instructions))
            {
                var nextNodeId = GetNextNodeId(instruction, nodeIterator);
                nodeIterator = navigator.Nodes[nextNodeId];
                count++;
                if (nodeIterator.Id.EndsWith(endNodeIdSuffix))
                {
                    nodesStepsToEnd.Add(count);
                    break;
                }
            }
        }

        var result = LeastCommonMultiple(nodesStepsToEnd);
        return result;
    }

    private static string GetNextNodeId(char instruction, Node node)
    {
        var nextNodeId = instruction switch
        {
            LEFT => node.LeftNodeId,
            RIGHT => node.RightNodeId,
            _ => throw new ArgumentOutOfRangeException($"Unknown instruction {instruction}")
        };
        return nextNodeId;
    }

    private static Navigator ParseInput(string input)
    {
        var (instructionsString, nodesString) = input.Split('\n', 2);

        var nodes = new Dictionary<string, Node>();
        foreach (var nodeString in nodesString.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var (nodeIdString, nodeInfoString) = nodeString.Split('=', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var (leftNodeIdString, rightNodeIdString) = nodeInfoString[1..^1].Split(',', StringSplitOptions.TrimEntries);

            var node = new Node
            {
                Id = nodeIdString,
                LeftNodeId = leftNodeIdString,
                RightNodeId = rightNodeIdString
            };

            nodes.Add(node.Id, node);
        }

        return new Navigator
        {
            Instructions = instructionsString,
            Nodes = nodes
        };
    }

    private static long LeastCommonMultiple(IReadOnlyList<long> values)
    {
        if (values.Count == 0)
        {
            throw new ArgumentException("No values provided");
        }

        var result = values[0];
        for (int i = 1; i < values.Count; i++)
        {
            result = LeastCommonMultiple(result, values[i]);
        }

        return result;
    }

    private static long LeastCommonMultiple(long a, long b)
    {
        return a * b / BiggestCommonDivisor(a, b);
    }

    private static long BiggestCommonDivisor(long a, long b)
    {
        var dividend = 0L;
        var divisor = 0L;

        if (a > b)
        {
            dividend = a;
            divisor = b;
        }
        else
        {
            dividend = b;
            divisor = a;
        }

        var result = 0L;
        
        while (true)
        {
            var remainder = dividend % divisor;
            if (remainder != 0)
            {
                dividend = divisor;
                divisor = remainder;
            }
            else
            {
                result = divisor;
                break;
            }
        }

        return result;
    }
}

internal record Navigator
{
    public string Instructions { get; set; }
    public Dictionary<string, Node> Nodes { get; set; }
}

internal record Node
{
    public string Id { get; set; }
    public string LeftNodeId { get; set; }
    public string RightNodeId { get; set; }
}