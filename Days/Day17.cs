using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/17
    /// </summary>
    internal class Day17 : AdventDay
    {
        /// <summary>
        /// The maximum Y value to use as velocity for simulations.
        /// </summary>
        private const int SimulationVelocityYMax = 2_000;

        /// <summary>
        /// The target area.
        /// </summary>
        private readonly Rect targetArea;

        public Day17()
        {
            Match inputMatch = Regex.Match(GetInputData(), @"target area: x=(-?\d+)..(-?\d+), y=(-?\d+)..(-?\d+)");

            targetArea = new Rect(
                int.Parse(inputMatch.Groups[1].Value),
                int.Parse(inputMatch.Groups[4].Value),
                int.Parse(inputMatch.Groups[2].Value),
                int.Parse(inputMatch.Groups[3].Value));
        }
        internal override void SolvePuzzle1()
        {
            int minVelocityX = 1;
            int maxVelocityX = targetArea.Right + 1;

            int minVelocityY = 1;
            int maxVelocityY = SimulationVelocityYMax;

            int maxY = 0;

            for (int vx = minVelocityX; vx <= maxVelocityX; vx++)
            {
                for (int vy = minVelocityY; vy <= maxVelocityY; vy++)
                {
                    var r = Simulate(vx, vy);
                    if (r.ReachedTargetArea & r.MaxY > maxY)
                    {
                        maxY = r.MaxY;
                    }
                }
            }

            WriteSolution1(maxY);
        }

        internal override void SolvePuzzle2()
        {
            int minVelocityX = 1;
            int maxVelocityX = targetArea.Right + 1;

            int minVelocityY = -SimulationVelocityYMax;
            int maxVelocityY = SimulationVelocityYMax;

            int count = 0;

            for (int vx = minVelocityX; vx <= maxVelocityX; vx++)
            {
                for (int vy = minVelocityY; vy <= maxVelocityY; vy++)
                {
                    var r = Simulate(vx, vy);
                    if (r.ReachedTargetArea)
                    {
                        count++;
                    }
                }
            }

            WriteSolution2(count);
        }

        /// <summary>
        /// Simulates the probe launcher.
        /// </summary>
        /// <param name="velocityX">The initial X velocity.</param>
        /// <param name="velocityY">The initial Y velocity.</param>
        /// <returns>The results of the simulation.</returns>
        private SimulationReport Simulate(int velocityX, int velocityY)
        {
            int x = 0;
            int y = 0;

            int maxY = y;

            while (y > targetArea.Bottom)
            {
                x += velocityX;
                y += velocityY;

                if (y > maxY)
                {
                    maxY = y;
                }

                if (targetArea.Contains(x, y))
                {
                    return new SimulationReport(true, maxY);
                }

                velocityX = velocityX == 0 ? 0 : velocityX - 1;
                velocityY--;
            }

            return new SimulationReport(false, maxY);
        }

        /// <summary>
        /// A rectangle.
        /// </summary>
        private struct Rect
        {
            /// <summary>
            /// The left coordinate of the rectange.
            /// </summary>
            public readonly int Left;

            /// <summary>
            /// The top coordinate of the rectange.
            /// </summary>
            public readonly int Top;

            /// <summary>
            /// The right coordinate of the rectange.
            /// </summary>
            public readonly int Right;

            /// <summary>
            /// The bottom coordinate of the rectange.
            /// </summary>
            public readonly int Bottom;

            /// <summary>
            /// Creates a new <see cref="Rect"/>.
            /// </summary>
            /// <param name="left">The left coordinate of the rectangle.</param>
            /// <param name="top">The top coordinate of the rectangle.</param>
            /// <param name="right">The right coordinate of the rectangle.</param>
            /// <param name="bottom">The bottom coordinate of the rectangle.</param>
            public Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            /// <summary>
            /// Determines whether a point is contained within the rectangle.
            /// </summary>
            /// <param name="x">The x coordinate of the point.</param>
            /// <param name="y">The y coordinate of the point.</param>
            /// <returns>True if the point is contained, false otherwise.</returns>
            public bool Contains(int x, int y)
            {
                return Left <= x & x <= Right & Bottom <= y & y <= Top;
            }
        }

        /// <summary>
        /// Results of the simulation.
        /// </summary>
        private class SimulationReport
        {
            /// <summary>
            /// Whether the probe reached the target area.
            /// </summary>
            public readonly bool ReachedTargetArea;

            /// <summary>
            /// The maximum Y height achieved during the simulation.
            /// </summary>
            public readonly int MaxY;

            /// <summary>
            /// Creates a new <see cref="SimulationReport"/>.
            /// </summary>
            /// <param name="reachedTargetArea">Whether the probe reached the target area.</param>
            /// <param name="maxY">The maximuim Y height achieved.</param>
            public SimulationReport(bool reachedTargetArea, int maxY)
            {
                ReachedTargetArea = reachedTargetArea;
                MaxY = maxY;
            }
        }
    }
}
