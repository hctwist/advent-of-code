using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/2
    /// </summary>
    internal class Day2 : AdventDay
    {
        /// <summary>
        /// The move instructions.
        /// </summary>
        private readonly MoveInstruction[] instructions;

        public Day2()
        {
            instructions = GetInputData("\n", MoveInstruction.Parse);
        }

        internal override void SolvePuzzle1()
        {
            int horizontalPosition = 0;
            int depth = 0;

            foreach (MoveInstruction instruction in instructions)
            {
                switch (instruction.Direction)
                {
                    case MoveDirection.Up:
                        depth -= instruction.Amount;
                        break;
                    case MoveDirection.Down:
                        depth += instruction.Amount;
                        break;
                    case MoveDirection.Forward:
                        horizontalPosition += instruction.Amount;
                        break;
                }
            }

            WriteSolution1(horizontalPosition * depth);
        }

        internal override void SolvePuzzle2()
        {
            int aim = 0;
            int horizontalPosition = 0;
            int depth = 0;

            foreach (MoveInstruction instruction in instructions)
            {
                switch (instruction.Direction)
                {
                    case MoveDirection.Up:
                        aim -= instruction.Amount;
                        break;
                    case MoveDirection.Down:
                        aim += instruction.Amount;
                        break;
                    case MoveDirection.Forward:
                        horizontalPosition += instruction.Amount;
                        depth += aim * instruction.Amount;
                        break;
                }
            }

            WriteSolution2(horizontalPosition * depth);
        }

        /// <summary>
        /// A move instruction.
        /// </summary>
        private struct MoveInstruction
        {
            /// <summary>
            /// The direction to move in.
            /// </summary>
            public readonly MoveDirection Direction;

            /// <summary>
            /// The amount to move.
            /// </summary>
            public readonly int Amount;

            /// <summary>
            /// Creates a new <see cref="MoveInstruction"/>.
            /// </summary>
            /// <param name="direction"></param>
            /// <param name="amount"></param>
            public MoveInstruction(MoveDirection direction, int amount)
            {
                Direction = direction;
                Amount = amount;
            }

            /// <summary>
            /// Parses a string into a <see cref="MoveInstruction"/>.
            /// </summary>
            /// <param name="str">A string of the format "{direction} {amount}" where {direction} is either "forward", "down" or "up" and {amount} is an integer.</param>
            /// <returns>The <see cref="MoveInstruction"/> parsed from <paramref name="str"/>.</returns>
            /// <exception cref="ArgumentException">Thrown if the direction isn't valid.</exception>
            public static MoveInstruction Parse(string str)
            {
                string[] splitString = str.Split(" ");
                (string directionString, string moveAmountString) = (splitString[0], splitString[1]);

                MoveDirection direction = directionString switch
                {
                    "forward" => MoveDirection.Forward,
                    "up" => MoveDirection.Up,
                    "down" => MoveDirection.Down,
                    _ => throw new ArgumentException($"{directionString} cannot be parsed into a {nameof(MoveDirection)}")
                };

                return new MoveInstruction(direction, int.Parse(moveAmountString));
            }
        }

        /// <summary>
        /// A direction to move in.
        /// </summary>
        private enum MoveDirection
        {
            Forward, Up, Down
        }
    }
}
