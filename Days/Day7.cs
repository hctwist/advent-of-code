using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/7
    /// </summary>
    internal class Day7 : AdventDay
    {
        private readonly int[] crabPositions;

        public Day7()
        {
            crabPositions = GetInputData(",", int.Parse);
        }

        internal override void SolvePuzzle1()
        {
            WriteSolution1(GetMinimumFuelCost(x => x));
        }

        internal override void SolvePuzzle2()
        {
            WriteSolution2(GetMinimumFuelCost(x => x * (x + 1) / 2));
        }

        /// <summary>
        /// Gets the minimum fuel cost that can be achieved to align all crabs.
        /// </summary>
        /// <param name="costFunction">The fuel cost of moving a certain distance.</param>
        /// <returns>The minimum fuel cost.</returns>
        private int GetMinimumFuelCost(Func<int, int> costFunction)
        {
            int startPosition = crabPositions.Min();
            int endPosition = crabPositions.Max();

            int bestPositionFuelCost = GetFuelCost(startPosition, costFunction);

            // Check each position
            for (int position = startPosition + 1; position <= endPosition; position++)
            {
                int positionFuelCost = GetFuelCost(position, costFunction);

                if (positionFuelCost < bestPositionFuelCost)
                {
                    bestPositionFuelCost = positionFuelCost;
                }
            }

            return bestPositionFuelCost;
        }

        /// <summary>
        /// Gets the fuel cost of moving to a certain position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="costFunction">The fuel cost of moving a certain distance.</param>
        /// <returns>The fuel cost.</returns>
        private int GetFuelCost(int position, Func<int, int> costFunction)
        {
            int totalDistance = 0;

            for (int i = 0; i < crabPositions.Length; i++)
            {
                totalDistance += costFunction(Math.Abs(position - crabPositions[i]));
            }

            return totalDistance;
        }
    }
}
