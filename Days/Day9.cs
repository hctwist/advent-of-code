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
        /// <summary>
        /// The initial heights from the cave floor.
        /// </summary>
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
            TraversedCell[,] traversalMap = BuildTraversedCellMap();

            List<int> basinSizes = new();

            for (int row = 0; row < heightMap.GetLength(0); row++)
            {
                for (int column = 0; column < heightMap.GetLength(1); column++)
                {
                    if (IsLowPoint(row, column))
                    {
                        TryTraverseBasin(traversalMap, row, column, -1);
                        basinSizes.Add(CountAndReset(traversalMap));
                    }
                }
            }

            WriteSolution2(basinSizes.OrderBy(size => size).Reverse().Take(3).Aggregate((a, b) => a * b));
        }

        /// <summary>
        /// Builds a map which keeps track of traversals of the ocean floor.
        /// </summary>
        /// <returns>The created map.</returns>
        private TraversedCell[,] BuildTraversedCellMap()
        {
            TraversedCell[,] map = new TraversedCell[heightMap.GetLength(0), heightMap.GetLength(1)];

            for (int row = 0; row < heightMap.GetLength(0); row++)
            {
                for (int column = 0; column < heightMap.GetLength(1); column++)
                {
                    map[row, column] = new TraversedCell(heightMap[row, column]);
                }
            }

            return map;
        }

        /// <summary>
        /// Counts the number of cells that have been travered, and resets them.
        /// </summary>
        /// <param name="traversalMap">The map.</param>
        /// <returns>The number of cells traversed.</returns>
        private static int CountAndReset(TraversedCell[,] traversalMap)
        {
            int traversedCount = 0;

            foreach (TraversedCell cell in traversalMap)
            {
                if (cell.Traversed)
                {
                    traversedCount++;
                    cell.Traversed = false;
                }
            }

            return traversedCount;
        }

        /// <summary>
        /// Traverses a basin in a traversal map.
        /// </summary>
        /// <param name="traversalMap">The map.</param>
        /// <param name="row">The row to start on.</param>
        /// <param name="column">The column  to start on.</param>
        /// <param name="previousHeight"></param>
        private void TryTraverseBasin(TraversedCell[,] traversalMap, int row, int column, int previousHeight)
        {
            if (OutOfBounds(row, column))
            {
                return;
            }

            TraversedCell cell = traversalMap[row, column];

            if (!cell.Traversed & cell.Height > previousHeight & cell.Height != 9)
            {
                traversalMap[row, column].Traversed = true;

                TryTraverseBasin(traversalMap, row, column - 1, cell.Height);
                TryTraverseBasin(traversalMap, row - 1, column, cell.Height);
                TryTraverseBasin(traversalMap, row, column + 1, cell.Height);
                TryTraverseBasin(traversalMap, row + 1, column, cell.Height);
            }
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
            if (OutOfBounds(row, column))
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

        /// <summary>
        /// Determines whether a cell is out of bounds.
        /// </summary>
        /// <param name="row">The row to check.</param>
        /// <param name="column">The column to check.</param>
        /// <returns>True if the cell was out of bounds, false otherwise.</returns>
        private bool OutOfBounds(int row, int column)
        {
            return row < 0 | row >= heightMap.GetLength(0) | column < 0 | column >= heightMap.GetLength(1);
        }
        
        /// <summary>
        /// A cell with a height, that holds information of whether the cell has been traversed or not.
        /// </summary>
        private class TraversedCell
        {
            /// <summary>
            /// The cell height off the ocean floor.
            /// </summary>
            public readonly int Height;

            /// <summary>
            /// Whether the cell has been traversed.
            /// </summary>
            public bool Traversed;

            /// <summary>
            /// Creates a new <see cref="TraversedCell"/>.
            /// </summary>
            /// <param name="height">The height of the cell off the ocean floor.</param>
            public TraversedCell(int height)
            {
                Height = height;
                Traversed = false;
            }
        }
    }
}
