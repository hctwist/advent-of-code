using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/4
    /// </summary>
    internal class Day4 : AdventDay
    {
        private readonly int[] drawOrder;

        private readonly List<BingoBoard> bingoBoards;

        public Day4()
        {
            bingoBoards = new List<BingoBoard>();

            string data = GetInputData();
            StringReader reader = new(data);

            drawOrder = reader.ReadLine()!.Split(",").Select(int.Parse).ToArray();

            while (reader.ReadLine() != null)
            {
                string[] lines =
                {
                    reader.ReadLine()!,
                    reader.ReadLine()!,
                    reader.ReadLine()!,
                    reader.ReadLine()!,
                    reader.ReadLine()!
                };

                bingoBoards.Add(CreateBoard(lines));
            }
        }

        /// <summary>
        /// Creates a bingo board.
        /// </summary>
        /// <param name="inputLines">Lines containing each row of bingo numbers.</param>
        /// <returns>The created bingo board.</returns>
        private static BingoBoard CreateBoard(string[] inputLines)
        {
            BingoBoard board = new();

            for (int lineIndex = 0; lineIndex < inputLines.Length; lineIndex++)
            {
                int columnIndex = 0;

                foreach (int number in Regex.Split(inputLines[lineIndex].Trim(), " +").Select(int.Parse))
                {
                    board.AddCell(new BingoCell(number, lineIndex, columnIndex++));
                }
            }

            return board;
        }

        internal override object? SolvePuzzle1()
        {
            foreach (int draw in drawOrder)
            {
                foreach (BingoBoard board in bingoBoards)
                {
                    if (board.TryMark(draw) && board.IsComplete)
                    {
                        return GetBoardScore(board, draw);
                    }
                }
            }

            return NoSolutionFound;
        }

        internal override object? SolvePuzzle2()
        {
            HashSet<BingoBoard> remainingBoards = new(bingoBoards);
            List<BingoBoard> newlyCompletedBoards = new();

            for (int drawIndex = 0; drawIndex < drawOrder.Length; drawIndex++)
            {
                int draw = drawOrder[drawIndex];

                foreach (BingoBoard board in remainingBoards)
                {
                    if (board.TryMark(draw) && board.IsComplete)
                    {
                        newlyCompletedBoards.Add(board);
                    }
                }

                foreach (BingoBoard newlyCompletedBoard in newlyCompletedBoards)
                {
                    remainingBoards.Remove(newlyCompletedBoard);
                }

                if (remainingBoards.Count == 1)
                {
                    BingoBoard lastBoard = remainingBoards.First();

                    for (int i = drawIndex + 1; i < drawOrder.Length; i++)
                    {
                        if (lastBoard.TryMark(drawOrder[i]) && lastBoard.IsComplete)
                        {
                            return GetBoardScore(lastBoard, drawOrder[i]);
                        }
                    }
                }

                newlyCompletedBoards.Clear();
            }

            return NoSolutionFound;
        }

        /// <summary>
        /// Calculates the 'score' of a bingo board.
        /// This is the product of the sum of unmarked numbers on the board and the last number called to complete it.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="winningNumber">The last number called which completed the board.</param>
        /// <returns>The score.</returns>
        private static int GetBoardScore(BingoBoard board, int winningNumber)
        {
            return board.GetUnmarkedTotal() * winningNumber;
        }

        public override void Reset()
        {
            foreach (BingoBoard board in bingoBoards)
            {
                board.Reset();
            }
        }

        /// <summary>
        /// The state of a single cell on a bingo board.
        /// </summary>
        private class BingoCell
        {
            /// <summary>
            /// The number.
            /// </summary>
            public int Number;

            /// <summary>
            /// The row the cell belongs to.
            /// </summary>
            public int Row;

            /// <summary>
            /// The column the cell belongs to.
            /// </summary>
            public int Column;

            /// <summary>
            /// Whether the cell has been marked.
            /// </summary>
            public bool Marked;

            /// <summary>
            /// Creates a new <see cref="BingoCell"/>.
            /// </summary>
            /// <param name="number">The number.</param>
            /// <param name="row">The row the cell belongs to.</param>
            /// <param name="column">The column the cell belongs to.</param>
            public BingoCell(int number, int row, int column)
            {
                Number = number;
                Row = row;
                Column = column;

                Marked = false;
            }
        }

        /// <summary>
        /// A bingo board.
        /// </summary>
        private class BingoBoard
        {
            /// <summary>
            /// All the cells on the board, as a map from the number of a bingo cell to the cell itself.
            /// </summary>
            private readonly Dictionary<int, BingoCell> cells;

            /// <summary>
            /// An array representing the number of cells in each row which have been marked.
            /// </summary>
            private readonly int[] markedRowCounts;

            /// <summary>
            /// An array representing the number of cells in each column which have been marked.
            /// </summary>
            private readonly int[] markedColumnCounts;

            /// <summary>
            /// The maximum number of cells that have been marked in any given row or column.
            /// </summary>
            private int maxMarkedCount;

            /// <summary>
            /// Creates a new <see cref="BingoBoard"/>.
            /// </summary>
            public BingoBoard()
            {
                cells = new Dictionary<int, BingoCell>();

                markedRowCounts = new int[5];
                markedColumnCounts = new int[5];
                maxMarkedCount = 0;
            }

            /// <summary>
            /// Checks whether the board is complete or not.
            /// </summary>
            public bool IsComplete => maxMarkedCount == 5;

            /// <summary>
            /// Adds a cell to the board.
            /// </summary>
            /// <param name="cell">The new cell.</param>
            public void AddCell(BingoCell cell)
            {
                cells[cell.Number] = cell;
            }

            /// <summary>
            /// Trys to mark the cell with a specific number.
            /// </summary>
            /// <param name="number">The number.</param>
            /// <returns>True if any cells were marked, ie. whether the number was on the board. False otherwise.</returns>
            public bool TryMark(int number)
            {
                if (cells.TryGetValue(number, out BingoCell? cell))
                {
                    cell.Marked = true;
                    maxMarkedCount = Math.Max(maxMarkedCount, ++markedRowCounts[cell.Row]);
                    maxMarkedCount = Math.Max(maxMarkedCount, ++markedColumnCounts[cell.Column]);

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Computes the sum of all unmarked cells on the board.
            /// </summary>
            /// <returns>The sum.</returns>
            public int GetUnmarkedTotal()
            {
                int unmarkedTotal = 0;

                foreach (BingoCell cell in cells.Values)
                {
                    if (!cell.Marked)
                    {
                        unmarkedTotal += cell.Number;
                    }
                }

                return unmarkedTotal;
            }

            /// <summary>
            /// Resets the board's state.
            /// </summary>
            /// <remarks>This keeps all cells on the board.</remarks>
            public void Reset()
            {
                foreach (BingoCell cell in cells.Values)
                {
                    cell.Marked = false;
                }

                for (int i = 0; i < markedRowCounts.Length; i++)
                {
                    markedRowCounts[i] = 0;
                }

                for (int i = 0; i < markedColumnCounts.Length; i++)
                {
                    markedColumnCounts[i] = 0;
                }

                maxMarkedCount = 0;
            }
        }
    }
}
