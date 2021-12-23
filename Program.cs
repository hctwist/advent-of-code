
using Advent_of_Code;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

ConsoleColor defaultConsoleColor = Console.ForegroundColor;

Console.WriteLine();
Console.WriteLine("Advent of Code 2021");
Console.WriteLine(@"
   .-.                                                   \ /
  ( (                                |                  - * -
   '-`                              -+-                  / \
            \            o          _|_          \
            ))          }^{        /___\         ))
          .-#-----.     /|\     .---'-'---.    .-#-----.
     ___ /_________\   //|\\   /___________\  /_________\  
    /___\ |[] _ []|    //|\\    | A /^\ A |    |[] _ []| _.O,_
....|'#'|.|  |*|  |...///|\\\...|   |'|   |....|  |*|  |..(^)....");
Console.WriteLine();
Console.WriteLine();

while (true)
{
    Console.Write("Day ");
    string? dayInput = Console.ReadLine();

    if (!int.TryParse(dayInput, out int day))
    {
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
        Console.WriteLine($"Day {dayInput} <- Invalid day.");
        Console.WriteLine();

        continue;
    }

    Console.WriteLine();

    if (TryGetAdventDay(day, out AdventDay? adventDay))
    {
        Stopwatch stopwatch = new();

        stopwatch.Start();
        string? solution1 = adventDay.SolvePuzzle1()?.ToString();
        stopwatch.Stop();

        if (solution1 != null)
        {
            PrintSolution(1, solution1);
            Console.WriteLine();
        }

        double solution1TotalSeconds = stopwatch.Elapsed.TotalSeconds;

        adventDay.Reset();

        stopwatch.Restart();
        string? solution2 = adventDay.SolvePuzzle2()?.ToString();
        stopwatch.Stop();

        if (solution2 != null)
        {
            PrintSolution(2, solution2);
            Console.WriteLine();
        }

        double solution2TotalSeconds = stopwatch.Elapsed.TotalSeconds;

        if (solution1 == null & solution2 == null)
        {
            Console.WriteLine($"No solutions found for Day {day}.");
            Console.WriteLine();
            continue;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();

            double totalSeconds = (solution1 == null ? 0 : solution1TotalSeconds) + (solution2 == null ? 0 : solution2TotalSeconds);

            Console.Write($"Completed in {totalSeconds:0.0####} seconds");

            if (solution1 != null & solution2 != null)
            {
                Console.WriteLine($" ({solution1TotalSeconds:0.0####}s + {solution2TotalSeconds:0.0####}s)");
            }
            else
            {
                Console.WriteLine();
            }

            Console.ForegroundColor = defaultConsoleColor;
        }
    }
    else
    {
        Console.WriteLine($"Day {day} hasn't been started yet.");
        Console.WriteLine();
        continue;
    }

    Console.WriteLine();
    Console.WriteLine();
}

static void PrintSolution(int number, string solution)
{
    Console.Write($"Solution {number}: ");

    if (solution.Contains(Environment.NewLine))
    {
        Console.WriteLine();
    }

    Console.WriteLine(solution);
}

static bool TryGetAdventDay(int day, [NotNullWhen(returnValue: true)] out AdventDay? adventDay)
{
    Type? type = Type.GetType($"Advent_of_Code.Days.Day{day}");

    if (type == null)
    {
        adventDay = default;
        return false;
    }

    adventDay = (AdventDay?)Activator.CreateInstance(type);
    return adventDay != null;
}