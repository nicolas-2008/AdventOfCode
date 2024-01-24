namespace AdventOfCode;

public static class Day1TaskSolver
{
    public static async Task RunPart1Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(1);
        var lines = input.Split('\n');
        
        var sum = 0;
        foreach (var line in lines)
        {
            var leftHandDigit = 0;
            var rightHandDigit = 0;

            for (int i = 0; i < line.Length; ++i)
            {
                if (Char.IsDigit(line[i]))
                {
                    leftHandDigit = byte.Parse(line[i].ToString());
                    break;
                }
            }

            for (int i = line.Length - 1; i >= 0; --i)
            {
                if (Char.IsDigit(line[i]))
                {
                    rightHandDigit = byte.Parse(line[i].ToString());
                    break;
                }
            }

            var twoDigitNumber = leftHandDigit * 10 + rightHandDigit;
            sum += twoDigitNumber;
        }

        Console.WriteLine($"Sum is {sum}");
    }

    public static async Task RunPart2Async()
    {
        var inputProvider = new InputProvider();
        var input = await inputProvider.GetStringAsync(1);
        var lines = input.Split('\n');
        //lines = new[]
        //{
        //    "two1nine",
        //    "eightwothree",
        //    "abcone2threexyz",
        //    "xtwone3four",
        //    "4nineeightseven2",
        //    "zoneight234",
        //    "7pqrstsixteen"
        //};
        var digitsDict = new Dictionary<String, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 }
        };

        var sum = 0;
        foreach (var line in lines)
        {
            var leftHandDigit = 0;
            var rightHandDigit = 0;

            for (int i = 0; i < line.Length; ++i)
            {
                if (Char.IsDigit(line[i]))
                {
                    leftHandDigit = byte.Parse(line[i].ToString());
                    break;
                }
                else
                {
                    foreach (var item in digitsDict)
                    {
                        var digitStrLength = i + item.Key.Length;
                        if (digitStrLength <= line.Length && line[i..digitStrLength] == item.Key)
                        {
                            leftHandDigit = item.Value;
                            break;
                        }
                    }

                    if (leftHandDigit != 0)
                    {
                        break;
                    }
                }
            }

            for (int i = line.Length - 1; i >= 0; --i)
            {
                if (Char.IsDigit(line[i]))
                {
                    rightHandDigit = byte.Parse(line[i].ToString());
                    break;
                }
                else
                {
                    foreach (var item in digitsDict)
                    {
                        var index = line.LastIndexOf(item.Key, StringComparison.Ordinal);
                        if (index != -1 && index + item.Key.Length - 1 == i)
                        {
                            rightHandDigit = item.Value;
                            break;
                        }
                    }

                    if (rightHandDigit > 0)
                    {
                        break;
                    }
                }
            }

            var twoDigitNumber = leftHandDigit * 10 + rightHandDigit;
            sum += twoDigitNumber;
        }

        Console.WriteLine($"Sum is {sum}");
    }
}