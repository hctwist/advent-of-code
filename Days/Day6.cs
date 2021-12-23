using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/6
    /// </summary>
    internal class Day6 : AdventDay
    {
        /// <summary>
        /// The timers of the original fish.
        /// </summary>
        private readonly int[] startingFishTimers;

        public Day6()
        {
            startingFishTimers = GetInputData(",", int.Parse);
        }

        internal override object? SolvePuzzle1()
        {
            return SimulateLanternFish(80).Sum();
        }

        internal override object? SolvePuzzle2()
        {
            return SimulateLanternFish(256).Sum();
        }

        /// <summary>
        /// Simulates lantern fish spawning over a number of days.
        /// </summary>
        /// <param name="days">The number of days to simulate.</param>
        /// <returns>An array which contains the count of fish which have each of the 9 timer states.</returns>
        private long[] SimulateLanternFish(int days)
        {
            // A count of fish in each 9 timer states
            long[] fishCounts = new long[9];

            foreach (int timer in startingFishTimers)
            {
                fishCounts[timer]++;
            }

            for (int day = 0; day < days; day++)
            {
                long numberOfFishReadyToSpawn = fishCounts[0];

                for (int i = 1; i < fishCounts.Length; i++)
                {
                    fishCounts[i - 1] = fishCounts[i];
                }

                fishCounts[8] = numberOfFishReadyToSpawn;
                fishCounts[6] += numberOfFishReadyToSpawn;
            }

            return fishCounts;
        }
    }
}
