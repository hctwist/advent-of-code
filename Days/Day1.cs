using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/1
    /// </summary>
    internal class Day1 : AdventDay
    {
        /// <summary>
        /// The depths.
        /// </summary>
        private readonly int[] depths;

        public Day1()
        {
            depths = GetInputData("\n", int.Parse);
        }

        internal override void SolvePuzzle1()
        {
            WriteSolution1(CountNumberOfIncreases(depths, 1));
        }

        internal override void SolvePuzzle2()
        {
            WriteSolution2(CountNumberOfIncreases(depths, 3));
        }

        /// <summary>
        /// Counts the number of times the sum of depths (in a window) increases.
        /// </summary>
        /// <param name="depths">The depths.</param>
        /// <param name="windowSize">The window size to consider.</param>
        /// <returns>The number of increases.</returns>
        internal static int CountNumberOfIncreases(int[] depths, int windowSize)
        {
            int increaseCount = 0;

            int lastWindowSum = 0;
            
            // Compute the first window sum
            for (int i = 0; i < windowSize; i++)
            {
                lastWindowSum += depths[i];
            }

            for (int i = 1; i < depths.Length - windowSize + 1; i++)
            {
                int windowSum = lastWindowSum - depths[i - 1] + depths[i + windowSize - 1];

                if (windowSum > lastWindowSum)
                {
                    increaseCount++;
                }

                lastWindowSum = windowSum;
            }

            return increaseCount;
        }
    }
}
