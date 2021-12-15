using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/5
    /// </summary>
    internal class Day5 : AdventDay
    {
        /// <summary>
        /// The vent lines.
        /// </summary>
        private readonly List<VentLine> ventLines;

        public Day5()
        {
            string[] inputLines = GetInputData(Environment.NewLine);

            ventLines = new List<VentLine>(inputLines.Length);

            foreach (string inputLine in inputLines)
            {
                string[] pointStrings = inputLine.Split(" -> ");
                Point startPoint = Point.Parse(pointStrings[0]);
                Point endPoint = Point.Parse(pointStrings[1]);

                ventLines.Add(new VentLine(startPoint, endPoint));
            }
        }

        internal override void SolvePuzzle1()
        {
            int[,] ventMap = BuildVentMap(ventLines.Where(line => line.Start.X == line.End.X | line.Start.Y == line.End.Y));
            WriteSolution1(CountOverlappingVents(ventMap));
        }

        internal override void SolvePuzzle2()
        {
            int[,] ventMap = BuildVentMap(ventLines);
            WriteSolution1(CountOverlappingVents(ventMap));
        }

        /// <summary>
        /// Counts the number of points on a vent map where there is more than one vent.
        /// </summary>
        /// <param name="ventMap">The vent map.</param>
        /// <returns>The number of overlapping points.</returns>
        private static int CountOverlappingVents(int[,] ventMap)
        {
            int numberOfOverlappingVents = 0;

            foreach (int ventCount in ventMap)
            {
                if (ventCount > 1)
                {
                    numberOfOverlappingVents++;
                }
            }

            return numberOfOverlappingVents;
        }

        /// <summary>
        /// Constructs a 'vent map', a grid showing how many vents are at each point.
        /// </summary>
        /// <param name="ventLines">A given set of vent lines.</param>
        /// <returns>The constructed map.</returns>
        private static int[,] BuildVentMap(IEnumerable<VentLine> ventLines)
        {
            Point maxPoint = GetMaxPoint(ventLines);

            int[,] ventGrid = new int[maxPoint.X + 1, maxPoint.Y + 1];

            foreach (VentLine ventLine in ventLines)
            {
                Point lineDirection = ventLine.End - ventLine.Start;
                Point normalisedLineDirection = lineDirection / GCD(Math.Abs(lineDirection.X), Math.Abs(lineDirection.Y));

                Point cursor = ventLine.Start;

                int xThresholdStart = Math.Min(ventLine.Start.X, ventLine.End.X);
                int xThresholeEnd = Math.Max(ventLine.Start.X, ventLine.End.X);

                int yThresholdStart = Math.Min(ventLine.Start.Y, ventLine.End.Y);
                int yThresholeEnd = Math.Max(ventLine.Start.Y, ventLine.End.Y);

                while (xThresholdStart <= cursor.X & cursor.X <= xThresholeEnd & yThresholdStart <= cursor.Y & cursor.Y <= yThresholeEnd)
                {
                    ventGrid[cursor.X, cursor.Y]++;
                    cursor += normalisedLineDirection;
                }
            }

            return ventGrid;
        }

        /// <summary>
        /// Gets the maximum point from a set of vent lines.
        /// </summary>
        /// <param name="ventLines">The vent lines.</param>
        /// <returns>The maximum point.</returns>
        private static Point GetMaxPoint(IEnumerable<VentLine> ventLines)
        {
            int maxX = 0;
            int maxY = 0;

            foreach (VentLine line in ventLines)
            {
                maxX = Math.Max(maxX, line.Start.X);
                maxX = Math.Max(maxX, line.End.X);

                maxY = Math.Max(maxY, line.Start.Y);
                maxY = Math.Max(maxY, line.End.Y);
            }

            return new Point(maxX, maxY);
        }

        /// <summary>
        /// Computes the GCD of two integers.
        /// </summary>
        /// <param name="a">The first integer.</param>
        /// <param name="b">The second integer.</param>
        /// <returns>The GCD of <paramref name="a"/> and <paramref name="b"/>.</returns>
        private static int GCD(int a, int b)
        {
            while (a != 0 & b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }

            return a | b;
        }

        /// <summary>
        /// A vent line.
        /// </summary>
        public struct VentLine
        {
            /// <summary>
            /// The start of the vent line.
            /// </summary>
            public readonly Point Start;

            /// <summary>
            /// The end of the vent line.
            /// </summary>
            public readonly Point End;

            /// <summary>
            /// Creates a new <see cref="VentLine"/>.
            /// </summary>
            /// <param name="start">The start point.</param>
            /// <param name="end">The end point.</param>
            public VentLine(Point start, Point end)
            {
                Start = start;
                End = end;
            }
        }

        /// <summary>
        /// A 2D point.
        /// </summary>
        public struct Point
        {
            /// <summary>
            /// The x coordinate.
            /// </summary>
            public readonly int X;
            
            /// <summary>
            /// The y coordinate.
            /// </summary>
            public readonly int Y;

            /// <summary>
            /// Creates a new <see cref="Point"/>.
            /// </summary>
            /// <param name="x">X.</param>
            /// <param name="y">Y.</param>
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            /// <summary>
            /// Parses a string of the form {X},{Y} to create a point.
            /// </summary>
            /// <param name="point">The string to parse.</param>
            /// <returns>A point.</returns>
            public static Point Parse(string point)
            {
                string[] coordinates = point.Split(",");
                return new Point(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
            }

            /// <summary>
            /// Adds two points.
            /// </summary>
            /// <param name="point1">The first point.</param>
            /// <param name="point2">The second point.</param>
            /// <returns><paramref name="point1"/> + <paramref name="point2"/>.</returns>
            public static Point operator +(Point point1, Point point2)
            {
                return new Point(point1.X + point2.X, point1.Y + point2.Y);
            }

            /// <summary>
            /// Subtracts two points.
            /// </summary>
            /// <param name="point1">The first point.</param>
            /// <param name="point2">The second point.</param>
            /// <returns><paramref name="point1"/> - <paramref name="point2"/>.</returns>
            public static Point operator -(Point point1, Point point2)
            {
                return new Point(point1.X - point2.X, point1.Y - point2.Y);
            }

            /// <summary>
            /// Divides a point by a scalar.
            /// </summary>
            /// <param name="point">The point.</param>
            /// <param name="divisor">The divisor.</param>
            /// <returns>The scaled point.</returns>
            public static Point operator /(Point point, int divisor)
            {
                return new Point(point.X / divisor, point.Y / divisor);
            }
        }
    }
}
