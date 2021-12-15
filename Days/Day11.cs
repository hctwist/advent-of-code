using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/11
    /// </summary>
    internal class Day11 : AdventDay
    {
        /// <summary>
        /// The starting octopus energy.
        /// </summary>
        private readonly int[,] startingOctopusEnergy;

        /// <summary>
        /// The octopi.
        /// </summary>
        private readonly Octopus[,] octopi;

        public Day11()
        {
            string[] rows = GetInputData(Environment.NewLine);

            startingOctopusEnergy = new int[rows.Length, rows[0].Length];

            for (int row = 0; row < rows.Length; row++)
            {
                string rowString = rows[row];
                for (int column = 0; column < rowString.Length; column++)
                {
                    startingOctopusEnergy[row, column] = CharUnicodeInfo.GetDecimalDigitValue(rowString[column]);
                }
            }

            octopi = new Octopus[startingOctopusEnergy.GetLength(0), startingOctopusEnergy.GetLength(1)];
            CollectOctopi();
        }

        internal override void SolvePuzzle1()
        {
            int totalFlashes = 0;

            for (int i = 0; i < 100; i++)
            {
                totalFlashes += Step();
            }

            WriteSolution1(totalFlashes);
        }

        internal override void SolvePuzzle2()
        {
            int numberOfOctopi = octopi.Length;

            for (int i = 0; i < int.MaxValue; i++)
            {
                if (Step() == numberOfOctopi)
                {
                    WriteSolution2(i + 1);
                    return;
                }
            }

            WriteSolution2($"Could not find solution in {int.MaxValue} iterations.");
        }

        public override void Reset()
        {
            CollectOctopi();
        }

        /// <summary>
        /// Regenerates the octopi with their starting energy.
        /// </summary>
        private void CollectOctopi()
        {
            for (int row = 0; row < startingOctopusEnergy.GetLength(0); row++)
            {
                for (int column = 0; column < startingOctopusEnergy.GetLength(1); column++)
                {
                    octopi[row, column] = new Octopus(startingOctopusEnergy[row, column]);
                }
            }
        }

        /// <summary>
        /// Take one step in the octopus lifecycle.
        /// </summary>
        /// <returns>The number of octopi that have flashed during this step.</returns>
        private int Step()
        {
            for (int row = 0; row < octopi.GetLength(0); row++)
            {
                for (int column = 0; column < octopi.GetLength(1); column++)
                {
                    TryIncreaseOctopusEnergyLevel(row, column);
                }
            }

            int numberOfFlashes = 0;

            for (int row = 0; row < octopi.GetLength(0); row++)
            {
                for (int column = 0; column < octopi.GetLength(1); column++)
                {
                    Octopus octopus = octopi[row, column];
                    if (octopus.Flashed)
                    {
                        octopus.EnergyLevel = 0;
                        octopus.Flashed = false;
                        numberOfFlashes++;
                    }
                }
            }

            return numberOfFlashes;
        }

        /// <summary>
        /// If the octopus exists, increase its energy level.
        /// If the octopus flashes as a result of the increase, then this effect is spread.
        /// </summary>
        /// <param name="row">The row index of the octopus.</param>
        /// <param name="column">The column index of the octopus.</param>
        private void TryIncreaseOctopusEnergyLevel(int row, int column)
        {
            if (TryGetOctopus(row, column, out Octopus octopus))
            {
                octopus.EnergyLevel++;
                if (octopus.EnergyLevel > 9 & !octopus.Flashed)
                {
                    octopus.Flashed = true;

                    // Increase the adjacent octopi's energy levels
                    TryIncreaseOctopusEnergyLevel(row, column - 1);
                    TryIncreaseOctopusEnergyLevel(row - 1, column - 1);
                    TryIncreaseOctopusEnergyLevel(row - 1, column);
                    TryIncreaseOctopusEnergyLevel(row - 1, column + 1);
                    TryIncreaseOctopusEnergyLevel(row, column + 1);
                    TryIncreaseOctopusEnergyLevel(row + 1, column + 1);
                    TryIncreaseOctopusEnergyLevel(row + 1, column);
                    TryIncreaseOctopusEnergyLevel(row + 1, column - 1);
                }
            }
        }

        /// <summary>
        /// Gets an octopus.
        /// </summary>
        /// <param name="row">The row index of the octopus.</param>
        /// <param name="column">The column index of the octopus.</param>
        /// <param name="octopus">The found octopus.</param>
        /// <returns>True if the octopus was found, false otherwise (ie. if <paramref name="row"/> or <paramref name="column"/> is out of bounds).</returns>
        private bool TryGetOctopus(int row, int column, out Octopus octopus)
        {
            if (row < 0 | row >= octopi.GetLength(0) | column < 0 | column >= octopi.GetLength(1))
            {
                octopus = new Octopus(0);
                return false;
            }
            else
            {
                octopus = octopi[row, column];
                return true;
            }
        }

        /// <summary>
        /// The state of an octopus.
        /// </summary>
        private class Octopus
        {
            /// <summary>
            /// The octopus' energy level.
            /// </summary>
            public int EnergyLevel;

            /// <summary>
            /// Whether the octopus has flashed.
            /// </summary>
            public bool Flashed;

            /// <summary>
            /// Creates a new <see cref="Octopus"/>.
            /// </summary>
            /// <param name="energyLevel">The starting energy level of the octopus.</param>
            public Octopus(int energyLevel)
            {
                EnergyLevel = energyLevel;
                Flashed = false;
            }
        }
    }
}
