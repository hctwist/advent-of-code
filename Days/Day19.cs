using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/19
    /// </summary>
    internal class Day19 : AdventDay
    {
        /// <summary>
        /// The scanners.
        /// </summary>
        private readonly List<Scanner> scanners;

        /// <summary>
        /// The minimum number of beacons that need to overlap to constitute a match.
        /// </summary>
        private const int MinimumNumberOfBeaconsOverlapping = 12;

        public Day19()
        {
            string[] input = GetInputData(Environment.NewLine);

            scanners = new List<Scanner>();

            int i = 0;
            while (i < input.Length)
            {
                string scannerName = Regex.Match(input[i], @"scanner \d+").Value;
                i++;

                List<Point> beacons = new();

                while (i < input.Length && input[i] != "")
                {
                    string[] pointStrings = input[i].Split(",");

                    Point beacon = new(
                        int.Parse(pointStrings[0]),
                        int.Parse(pointStrings[1]),
                        int.Parse(pointStrings[2]));

                    beacons.Add(beacon);

                    i++;
                }

                i++;

                Scanner scanner = new(scannerName, beacons);
                scanners.Add(scanner);
            }
        }

        internal override object? SolvePuzzle1()
        {
            // Align pairs of scanners
            List<AlignedScanner> alignedScanners = AlignScanners();

            // Count distinct beacon locations
            HashSet<Point> allBeaconLocations = new();

            foreach (AlignedScanner scanner in alignedScanners)
            {
                allBeaconLocations.UnionWith(scanner.Scanner.BeaconLocations);
            }

            return allBeaconLocations.Count;
        }

        internal override object? SolvePuzzle2()
        {
            List<AlignedScanner> alignedScanners = AlignScanners();
            List<Point> scannerPositions = alignedScanners.Select(scanner => scanner.OriginalTransformation.Translation).ToList();

            int largestManhattanDistance = 0;

            foreach (Point position in scannerPositions)
            {
                foreach (Point otherPoint in scannerPositions)
                {
                    int manhattanDistance = Math.Abs(position.X - otherPoint.X) + Math.Abs(position.Y - otherPoint.Y) + Math.Abs(position.Z -  otherPoint.Z);
                    largestManhattanDistance = Math.Max(largestManhattanDistance, manhattanDistance);
                }
            }

            return largestManhattanDistance;
        }

        /// <summary>
        /// Finds an alignment of all the scanners.
        /// </summary>
        /// <returns>The aligned scanners.</returns>
        private List<AlignedScanner> AlignScanners()
        {
            List<AlignedScanner> alignedScanners = new(scanners.Count);
            alignedScanners.Add(new AlignedScanner(scanners[0], new Transformation()));

            List<Scanner> scannersToAlign = new(scanners.Skip(1));

            while (scannersToAlign.Count > 0)
            {
                AlignSinglePairOfScanners(scannersToAlign, alignedScanners);
            }

            return alignedScanners;
        }

        /// <summary>
        /// Trys to align a scanner with already aligned scanners.
        /// This will align one scanner, remove it from <paramref name="scannersToAlign"/> and add it to <paramref name="alignedScanners"/>.
        /// </summary>
        /// <param name="scannersToAlign">The scanners left to align.</param>
        /// <param name="alignedScanners">The already aligned scanners.</param>
        /// <exception cref="InvalidOperationException">Thrown if none of the scanners could be aligned.</exception>
        private static void AlignSinglePairOfScanners(List<Scanner> scannersToAlign, List<AlignedScanner> alignedScanners)
        {
            foreach (Scanner scanner in scannersToAlign)
            {
                foreach (AlignedScanner alignedScanner in alignedScanners)
                {
                    if (TryAlignScanners(scanner, alignedScanner.Scanner, out AlignedScanner newlyAlignedScanner))
                    {
                        scannersToAlign.Remove(scanner);
                        alignedScanners.Add(newlyAlignedScanner);
                        return;
                    }
                }
            }

            throw new InvalidOperationException("Couldn't align any pairs of scanners.");
        }

        /// <summary>
        /// Try and align two scanners.
        /// </summary>
        /// <param name="scanner">The scanner to align.</param>
        /// <param name="relativeTo">The scanner that <paramref name="scanner"/> should be aligned to.</param>
        /// <param name="alignedScanner">The aligned scanner.</param>
        /// <returns>True if the scanner could be aligned, false otherwise.</returns>
        private static bool TryAlignScanners(Scanner scanner, Scanner relativeTo, out AlignedScanner alignedScanner)
        {
            // Try and align each pair of beacons until we get the desired overlap
            foreach (Point relativeBeacon in relativeTo.BeaconLocations)
            {
                foreach (Point beacon in scanner.BeaconLocations)
                {
                    foreach (Direction direction in Enum.GetValues<Direction>())
                    {
                        foreach (Orientation orientation in Enum.GetValues<Orientation>())
                        {
                            Transformation transformation = new();

                            transformation.Direction = direction;
                            transformation.Orientation = orientation;

                            Point rotatedAndReflectedPoint = transformation.Transform(beacon);

                            // Find the translation necessary to align the two beacons
                            Point lockingTranslation = relativeBeacon - rotatedAndReflectedPoint;

                            transformation.Translation = lockingTranslation;

                            List<Point> transformedPoints = transformation.Transformed(scanner.BeaconLocations);
                            int commonPoints = CountCommonPoints(relativeTo.BeaconLocations, transformedPoints);

                            if (commonPoints >= MinimumNumberOfBeaconsOverlapping)
                            {
                                // We have found a valid alignment
                                alignedScanner = new AlignedScanner(new Scanner(scanner.Name, transformedPoints), transformation);
                                return true;
                            }
                        }
                    }
                }
            }

            alignedScanner = new AlignedScanner(new Scanner("", new List<Point>()), new Transformation());
            return false;
        }

        /// <summary>
        /// Counts the number of common points.
        /// </summary>
        /// <param name="points1">A list of points.</param>
        /// <param name="points2">Another list of points.</param>
        /// <returns>The number of common points between <paramref name="points1"/> and <paramref name="points2"/>.</returns>
        private static int CountCommonPoints(List<Point> points1, List<Point> points2)
        {
            HashSet<Point> set = new(points1.Concat(points2));
            return points1.Count + points2.Count - set.Count;
        }

        /// <summary>
        /// A transformation.
        /// </summary>
        private class Transformation
        {
            /// <summary>
            /// The direction.
            /// </summary>
            public Direction Direction;

            /// <summary>
            /// The orientation.
            /// </summary>
            public Orientation Orientation;

            /// <summary>
            /// The translation.
            /// </summary>
            public Point Translation;

            /// <summary>
            /// Creates a new <see cref="Transformation"/> with no translation, a positive Z direction and an up orientation.
            /// </summary>
            public Transformation()
            {
                Translation = new Point(0, 0, 0);
                Direction = Direction.PositiveZ;
                Orientation = Orientation.Up;
            }

            /// <summary>
            /// Transforms multiple points.
            /// </summary>
            /// <param name="points">The points.</param>
            /// <returns>A new list of transformed points.</returns>
            public List<Point> Transformed(List<Point> points)
            {
                return points.Select(Transform).ToList();
            }

            /// <summary>
            /// Transforms a single point.
            /// </summary>
            /// <param name="point">The point.</param>
            /// <returns>The transformed point.</returns>
            public Point Transform(Point point)
            {
                return TranslatePoint(OrientPoint(DirectPoint(point)));
            }

            /// <summary>
            /// Transforms a point to face a different direction.
            /// </summary>
            /// <param name="point">The point.</param>
            /// <returns>The transformed point.</returns>
            private Point DirectPoint(Point point)
            {
                return Direction switch
                {
                    Direction.PositiveZ => point,
                    Direction.NegativeZ => new Point(-point.X, point.Y, -point.Z),
                    Direction.PositiveY => new Point(point.X, -point.Z, point.Y),
                    Direction.NegativeY => new Point(point.X, point.Z, -point.Y),
                    Direction.PositiveX => new Point(-point.Z, point.Y, point.X),
                    Direction.NegativeX => new Point(point.Z, point.Y, -point.X),
                    _ => throw new InvalidOperationException($"Cannot transform a point with direction {Direction}")
                };
            }

            /// <summary>
            /// Transforms a point to have a different orientation.
            /// </summary>
            /// <param name="point">The point.</param>
            /// <returns>The transformed point.</returns>
            private Point OrientPoint(Point point)
            {
                return Orientation switch
                {
                    Orientation.Up => point,
                    Orientation.Down => new Point(-point.X, -point.Y, point.Z),
                    Orientation.Left => new Point(-point.Y, point.X, point.Z),
                    Orientation.Right => new Point(point.Y, -point.X, point.Z),
                    _ => throw new InvalidOperationException($"Cannot transform a point with orientation {Orientation}")
                };
            }

            /// <summary>
            /// Translates a point.
            /// </summary>
            /// <param name="point">The point.</param>
            /// <returns>The transformed point.</returns>
            private Point TranslatePoint(Point point)
            {
                return point + Translation;
            }
        }

        /// <summary>
        /// A direction.
        /// </summary>
        public enum Direction
        {
            NegativeX, PositiveX, NegativeY, PositiveY, NegativeZ, PositiveZ
        }

        /// <summary>
        /// An orientation.
        /// </summary>
        public enum Orientation
        {
            Up, Down, Left, Right
        }

        /// <summary>
        /// A scanner.
        /// </summary>
        private class Scanner
        {
            /// <summary>
            /// The name of the scanner.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// Locations of detected beacons, relative to this scanner.
            /// </summary>
            public readonly List<Point> BeaconLocations;

            /// <summary>
            /// Creates a new <see cref="Scanner"/>.
            /// </summary>
            /// <param name="name">The name of the scanner.</param>
            /// <param name="beaconLocations">The relative locations of detected beacons.</param>
            public Scanner(string name, List<Point> beaconLocations)
            {
                Name = name;
                BeaconLocations = beaconLocations;
            }
        }

        /// <summary>
        /// An aligned scanner.
        /// </summary>
        private class AlignedScanner
        {
            /// <summary>
            /// The scanner.
            /// </summary>
            public readonly Scanner Scanner;

            /// <summary>
            /// The transformation that aligned the scanner.
            /// </summary>
            public readonly Transformation OriginalTransformation;

            /// <summary>
            /// Creates a new <see cref="AlignedScanner"/>.
            /// </summary>
            /// <param name="scanner">The scanner.</param>
            /// <param name="originalTransformation">The transformation that aligned the scanner.</param>
            public AlignedScanner(Scanner scanner, Transformation originalTransformation)
            {
                Scanner = scanner;
                OriginalTransformation = originalTransformation;
            }
        }

        /// <summary>
        /// A point in 3D space.
        /// </summary>
        private struct Point
        {
            /// <summary>
            /// The X coordinate.
            /// </summary>
            public readonly int X;

            /// <summary>
            /// The Y coordinate.
            /// </summary>
            public readonly int Y;

            /// <summary>
            /// The Z coordinate.
            /// </summary>
            public readonly int Z;

            /// <summary>
            /// Creates a new <see cref="Point"/>.
            /// </summary>
            /// <param name="x">The X coordinate.</param>
            /// <param name="y">The Y coordinate.</param>
            /// <param name="z">The Z coordinate.</param>
            public Point(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static Point operator -(Point point)
            {
                return new Point(-point.X, -point.Y, -point.Z);
            }

            public static Point operator -(Point p1, Point p2)
            {
                return new Point(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
            }

            public static Point operator +(Point p1, Point p2)
            {
                return new Point(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
            }

            public override string ToString()
            {
                return $"({X}, {Y}, {Z})";
            }
        }
    }
}
