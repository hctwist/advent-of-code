
using Advent_of_Code;
using System.Diagnostics;

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
    Console.Write("Solve day: ");
    string? dayInput = Console.ReadLine();
    Console.WriteLine();

    if (!int.TryParse(dayInput, out int day))
    {
        Console.WriteLine("Invalid advent day");
        return;
    }

    if (TryGetAdventDay(day, out AdventDay? adventDay))
    {
        Stopwatch stopwatch = new();

        stopwatch.Start();
        adventDay!.SolvePuzzle1();
        stopwatch.Stop();

        double solution1TotalSeconds = stopwatch.Elapsed.TotalSeconds;
        Console.WriteLine();

        adventDay!.Reset();

        stopwatch.Restart();
        adventDay!.SolvePuzzle2();
        stopwatch.Stop();
        double solution2TotalSeconds = stopwatch.Elapsed.TotalSeconds;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine();
        Console.WriteLine($"Completed in {solution1TotalSeconds + solution2TotalSeconds:0.0####} seconds ({solution1TotalSeconds:0.0####}s + {solution2TotalSeconds:0.0####}s)");
        Console.ForegroundColor = defaultConsoleColor;
    }
    else
    {
        Console.WriteLine($"Couldn't find a solution for day {day}.");
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
}

static bool TryGetAdventDay(int day, out AdventDay? adventDay)
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