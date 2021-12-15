using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/9
    /// </summary>
    internal class Day9 : AdventDay
    {
        private readonly int[,] heightMap;

        public Day9()
        {
            string[] inputLines = GetInputData(Environment.NewLine);

            int rows = inputLines.Length;
            int columns = inputLines[0].Length;

            heightMap = new int[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    heightMap[row, column] = CharUnicodeInfo.GetDecimalDigitValue(inputLines[row][column]);
                }
            }
        }

        internal override void SolvePuzzle1()
        {
            int risk = 0;

            for (int row = 0; row < heightMap.GetLength(0); row++)
            {
                for (int column = 0; column < heightMap.GetLength(1); column++)
                {
                    if (IsLowPoint(row, column))
                    {
                        risk += 1 + heightMap[row, column];
                    }
                }
            }

            WriteSolution1(risk);
        }

        internal override void SolvePuzzle2()
        {
        }

        /// <summary>
        /// Checks whether a cell is a low point.
        /// A cell is a low point if all it's adjacent (non-diagonal) cells are strictly higher.
        /// </summary>
        /// <param name="row">The row of the cell to check.</param>
        /// <param name="column">The column of the cell to check.</param>
        /// <returns>True if the cell is a low point, false otherwise.</returns>
        private bool IsLowPoint(int row, int column)
        {
            int cell = heightMap[row, column];

            if (TryGetCell(row - 1, column, out int leftCell) & leftCell <= cell)
            {
                return false;
            }

            if (TryGetCell(row, column - 1, out int topCell) & topCell <= cell)
            {
                return false;
            }

            if (TryGetCell(row + 1, column, out int rightCell) & rightCell <= cell)
            {
                return false;
            }

            if (TryGetCell(row, column + 1, out int bottomCell) & bottomCell <= cell)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Trys to get a cell for the specified row and column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="cell">The cell. This is only set if the cell isn't out of bounds.</param>
        /// <returns>True if the cell was found (ie. wasn't out of bounds) or false otherwise.</returns>
        private bool TryGetCell(int row, int column, out int cell)
        {
            if (row < 0 | row >= heightMap.GetLength(0) | column < 0 | column >= heightMap.GetLength(1))
            {
                cell = default;
                return false;
            }
            else
            {
                cell = heightMap[row, column];
                return true;
            }
        }
    }
}
