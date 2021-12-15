
using Advent_of_Code;
using System.Diagnostics;

string horizontalBar = "---";

while (true)
{
    Console.WriteLine("~~~ The Advent of Code ~~~");
    Console.WriteLine();

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
        Console.WriteLine($"Attempting to solve day {day}");
        Console.WriteLine(horizontalBar);
        Console.WriteLine();

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

        Console.WriteLine();
        Console.WriteLine(horizontalBar);
        Console.WriteLine($"Attempt completed in {solution1TotalSeconds + solution2TotalSeconds} seconds ({solution1TotalSeconds} for solution 1 and {solution2TotalSeconds} for solution 2)");
    }
    else
    {
        Console.WriteLine($"Couldn't find a solution for day {day}");
    }

    Console.WriteLine();
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