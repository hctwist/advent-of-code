using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/13
    /// </summary>
    internal class Day13 : AdventDay
    {
        /// <summary>
        /// The initial input.
        /// </summary>
        private readonly string[] input;

        /// <summary>
        /// The paper.
        /// </summary>
        private Paper paper;

        /// <summary>
        /// The fold instructions.
        /// </summary>
        private List<FoldInstruction> instructions;

        public Day13()
        {
            input = GetInputData(Environment.NewLine);
            (paper, instructions) = Parse(input);
        }

        /// <summary>
        /// Parse the input data.
        /// </summary>
        /// <param name="input">The input lines.</param>
        /// <returns>The parsed paper and instructions.</returns>
        private static (Paper, List<FoldInstruction>) Parse(string[] input)
        {
            List<FoldInstruction> instructions = new();
            HashSet<Point> points = new();

            bool parsingInstructions = false;

            foreach (string line in input)
            {
                if (line == "")
                {
                    parsingInstructions = true;
                }
                else if (parsingInstructions)
                {
                    GroupCollection groups = Regex.Match(line, @"fold along ([xy])=(\d+)").Groups;
                    instructions.Add(new FoldInstruction(int.Parse(groups[2].Value), groups[1].Value == "y"));
                }
                else
                {
                    string[] xy = line.Split(",");
                    points.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
                }
            }

            return (new Paper(points), instructions);
        }

        internal override void SolvePuzzle1()
        {
            paper.Fold(instructions[0]);
            WriteSolution1(paper.Points.Count);
        }

        internal override void SolvePuzzle2()
        {
            foreach (FoldInstruction instruction in instructions)
            {
                paper.Fold(instruction);
            }

            WriteSolution2(paper);
        }

        public override void Reset()
        {
            (paper, instructions) = Parse(input);
        }

        /// <summary>
        /// A collection of points drawn on paper.
        /// </summary>
        private class Paper
        {
            /// <summary>
            /// The points on the paper.
            /// </summary>
            public readonly HashSet<Point> Points;

            /// <summary>
            /// Creates a new <see cref="Paper"/>.
            /// </summary>
            /// <param name="points">The points on the paper.</param>
            public Paper(HashSet<Point> points)
            {
                Points = points;
            }

            /// <summary>
            /// Folds the paper.
            /// </summary>
            /// <param name="instruction">The instruction with details on how the paper is folded.</param>
            public void Fold(FoldInstruction instruction)
            {
                HashSet<Point> oldPoints = new(Points);
                Points.Clear();

                if (instruction.Y)
                {
                    foreach (Point point in oldPoints)
                    {
                        Point newPoint = point.Y > instruction.Pivot ?
                            new Point(point.X, instruction.Pivot- (point.Y - instruction.Pivot)) :
                            point;

                        Points.Add(newPoint);
                    }
                }
                else
                {
                    foreach (Point point in oldPoints)
                    {
                        Point newPoint = point.X > instruction.Pivot ?
                            new Point(instruction.Pivot - (point.X - instruction.Pivot), point.Y) :
                            point;

                        Points.Add(newPoint);
                    }
                }
            }

            public override string ToString()
            {
                int minX = int.MaxValue;
                int maxX = int.MinValue;

                int minY = int.MaxValue;
                int maxY = int.MinValue;

                foreach (Point point in Points)
                {
                    minX = Math.Min(minX, point.X);
                    maxX = Math.Max(maxX, point.X);

                    minY = Math.Min(minY, point.Y);
                    maxY = Math.Max(maxY, point.Y);
                }

                bool[,] pointGrid = new bool[maxX - minX + 1, maxY - minY + 1];

                foreach (Point point in Points)
                {
                    pointGrid[point.X - minX, point.Y - minY] = true;
                }

                StringBuilder builder = new();

                for (int row = 0; row < pointGrid.GetLength(1); row++)
                {
                    for (int column = 0; column < pointGrid.GetLength(0); column++)
                    {
                        if (pointGrid[column, row])
                        {
                            builder.Append('■');
                        }
                        else
                        {
                            builder.Append(' ');
                        }
                    }

                    builder.AppendLine();
                }

                builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);

                return builder.ToString();
            }
        }

        /// <summary>
        /// An instruction on how to fold paper.
        /// </summary>
        private class FoldInstruction
        {
            /// <summary>
            /// The fold pivot point.
            /// </summary>
            public readonly int Pivot;

            /// <summary>
            /// Whether the fold is along the Y direction.
            /// </summary>
            public readonly bool Y;

            /// <summary>
            /// Creates a new <see cref="FoldInstruction"/>.
            /// </summary>
            /// <param name="pivot">The pivot.</param>
            /// <param name="y">Whether the fold is along the Y direction.</param>
            public FoldInstruction(int pivot, bool y)
            {
                Pivot = pivot;
                Y = y;
            }
        }

        /// <summary>
        /// An integer point.
        /// </summary>
        private struct Point
        {
            /// <summary>
            /// The X coordinate.
            /// </summary>
            public int X;

            /// <summary>
            /// The Y coordinate.
            /// </summary>
            public int Y;

            /// <summary>
            /// Creates a new point.
            /// </summary>
            /// <param name="x">The X coordinate.</param>
            /// <param name="y">The Y coordinate.</param>
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return $"({X}, {Y})";
            }
        }
    }
}
