using System.Diagnostics.CodeAnalysis;
using MoreLinq;

namespace AdventOfCode;

public static class Day10TaskSolver
{
    private const char PIPE_E_W = '-';
    private const char PIPE_N_S = '|';
    private const char PIPE_S_E = 'F';
    private const char PIPE_N_E = 'L';
    private const char PIPE_S_W = '7';
    private const char PIPE_N_W = 'J';

    private static char[] PIPES = { PIPE_E_W, PIPE_N_S, PIPE_S_E, PIPE_N_E, PIPE_S_W, PIPE_N_W };

    private const char ANIMAL = 'S';
    private const char GRASS = '.';

    private static bool IsPipe(char c)
    {
        return PIPES.Contains(c);
    }
    
    private static string GetTestInput()
    {
        return @".....
.S-7.
.|.|.
.L-J.
.....";
    }

    public static async Task RunPart1Async()
    {
        // var inputProvider = new InputProvider();
        // var input = await inputProvider.GetStringAsync(9);
        var input = GetTestInput();
        var field = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var animalPosition = Find(ANIMAL, field);

        var iteratorPosition = animalPosition;
        var iteratorObject = ANIMAL;
        var loopLength = 0;

        do
        {
            iteratorPosition = 
        } while (iteratorPosition != animalPosition);
    }
    
    private static IEnumerable<Pos> FindAdjacentPipes(Pos position, string[] field)
    {
        var left = position.Left();
        if (left.X >= 0 && IsPipe(field[left.Y][left.X]))
        {
            yield return left;
        }
        
        var right = position.Right();
        if (right.X < field[0].Length && IsPipe(field[right.Y][right.X]))
        {
            yield return right;
        }

        var up = position.Up();
        if (up.X >= 0 && IsPipe(field[up.Y][up.X]))
        {
            yield return up;
        }

        var down = position.Down();
        if (down.Y < field.Length && IsPipe(field[down.Y][down.X]))
        {
            yield return down;
        }
    }

    private static Pos Find(char c, string[] field)
    {
        for (int y = 0; y < field.Length; y++)
        {
            for (int x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == c)
                {
                    return new(x, y);
                }
            }
        }

        throw new ApplicationException("Not found");
    }

    public static async Task RunPart2Async()
    {
    }

    private readonly record struct Pos(int X, int Y)
    {
        public Pos Left() => this with { X = this.X - 1 };
        public Pos Right() => this with { X = this.X + 1 };
        public Pos Up() => this with { Y = Y - 1 };
        public Pos Down() => this with { Y = Y + 1 };
    }
}