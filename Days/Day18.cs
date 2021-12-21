using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/18
    /// </summary>
    internal class Day18 : AdventDay
    {
        /// <summary>
        /// The snailfish numbers.
        /// </summary>
        private readonly List<SnailfishNumber> numbers;

        public Day18()
        {
            string[] input = GetInputData(Environment.NewLine);

            numbers = input.Select(i => Parse(i)).ToList();
        }

        /// <summary>
        /// Parses a string input representing a snailfish number.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>A snailfish number.</returns>
        private static SnailfishNumber Parse(string input)
        {
            // Consider all brackets and numbers, spaces and commas can be ignored
            MatchCollection matches = Regex.Matches(input, @"\d+|\[|\]");
            List<SnailfishElement> numbers = new();

            int depth = -1;

            for (int i = 0; i < matches.Count; i++)
            {
                string match = matches[i].Value;

                if (match == "[")
                {
                    depth++;
                }
                else if (match == "]")
                {
                    depth--;
                }
                else
                {
                    SnailfishElement newNumber = new(int.Parse(match), SnailfishElement.ElementType.Isolated, depth);

                    if (numbers.Count > 0)
                    {
                        SnailfishElement lastNumber = numbers.Last();

                        if (lastNumber.Depth == depth &
                            lastNumber.Type == SnailfishElement.ElementType.Isolated &
                            matches[i - 1].Value != "[")
                        {
                            // This must be a pair
                            lastNumber.Type = SnailfishElement.ElementType.PairLeft;
                            newNumber.Type = SnailfishElement.ElementType.PairRight;
                        }
                    }

                    numbers.Add(newNumber);
                }
            }

            return new SnailfishNumber(numbers);
        }

        internal override void SolvePuzzle1()
        {
            SnailfishNumber startingNumber = numbers[0].Copy();

            foreach (SnailfishNumber number in numbers.Skip(1))
            {
                startingNumber.Add(number);
            }

            WriteSolution1(startingNumber.Magnitude());
        }

        internal override void SolvePuzzle2()
        {
            int bestMagnitude = 0;

            // Consider all pairs of numbers
            foreach (SnailfishNumber firstNumber in numbers)
            {
                foreach (SnailfishNumber secondNumber in numbers)
                {
                    if (firstNumber != secondNumber)
                    {
                        SnailfishNumber sum = firstNumber.Copy();
                        sum.Add(secondNumber);

                        bestMagnitude = Math.Max(bestMagnitude, sum.Magnitude());
                    }
                }
            }

            WriteSolution2(bestMagnitude);
        }

        /// <summary>
        /// A snailfish number.
        /// This class is not immutable, so it should be copied when necessary with <see cref="Copy"/>.
        /// </summary>
        private class SnailfishNumber
        {
            /// <summary>
            /// The elements of the number.
            /// </summary>
            private readonly List<SnailfishElement> elements;

            /// <summary>
            /// Creates a new <see cref="SnailfishNumber"/>.
            /// </summary>
            /// <param name="elements">The elements.</param>
            public SnailfishNumber(List<SnailfishElement> elements)
            {
                this.elements = elements;
            }

            /// <summary>
            /// Adds another snailfish number to this one.
            /// This does not modify <paramref name="number"/>.
            /// </summary>
            /// <param name="number">The number to add.</param>
            public void Add(SnailfishNumber number)
            {
                foreach (SnailfishElement element in elements)
                {
                    element.Depth++;
                }

                SnailfishNumber numberCopy = number.Copy();

                foreach (SnailfishElement element in numberCopy.elements)
                {
                    element.Depth++;
                }

                elements.AddRange(numberCopy.elements);

                Reduce();
            }

            /// <summary>
            /// Computes the magnitude of the snailfish number.
            /// </summary>
            /// <returns>The magnitude.</returns>
            public int Magnitude()
            {
                // Ensure we aren't mutating the original number
                SnailfishNumber copy = Copy();

                // Iteratively compute the magnitude of each real pair until we have reduced down to a single number
                while (copy.elements.Count > 1)
                {
                    for (int i = 0; i < copy.elements.Count - 1; i++)
                    {
                        if (copy.elements[i].Type == SnailfishElement.ElementType.PairLeft &
                            copy.elements[i + 1].Type == SnailfishElement.ElementType.PairRight)
                        {
                            // Compute the magnitude from the pair
                            SnailfishElement newElement = new(
                                3 * copy.elements[i].Value + 2 * copy.elements[i + 1].Value,
                                SnailfishElement.ElementType.Isolated, copy.elements[i].Depth - 1);

                            copy.elements.RemoveRange(i, 2);
                            copy.elements.Insert(i, newElement);

                            // Pair the magnitude
                            copy.TryFormNewPair(i);

                            break;
                        }
                    }
                }

                return copy.elements[0].Value;
            }

            /// <summary>
            /// Copies the snailfish number.
            /// This is a deep copy, so each element in the number is copied too.
            /// </summary>
            /// <returns>A copy of the number.</returns>
            public SnailfishNumber Copy()
            {
                return new SnailfishNumber(elements.Select(element => element.Copy()).ToList());
            }

            /// <summary>
            /// Reduces the number.
            /// </summary>
            private void Reduce()
            {
                while (true)
                {
                    while (TryExplode())
                    {
                        continue;
                    }

                    if (!TrySplit())
                    {
                        break;
                    }
                }
            }

            /// <summary>
            /// Tries to explode a pair in the snailfish number.
            /// </summary>
            /// <returns>True if a pair did explode, false otherwise.</returns>
            private bool TryExplode()
            {
                for (int i = 0; i < elements.Count - 1; i++)
                {
                    SnailfishElement pairLeftCandidate = elements[i];
                    SnailfishElement pairRightCandidate = elements[i + 1];

                    // Check if this is a true pair
                    if (pairLeftCandidate.Type == SnailfishElement.ElementType.PairLeft &
                        pairRightCandidate.Type == SnailfishElement.ElementType.PairRight &
                        pairLeftCandidate.Depth == 4)
                    {
                        if (i != 0)
                        {
                            elements[i - 1].Value += pairLeftCandidate.Value;
                        }

                        if (i + 1 != elements.Count - 1)
                        {
                            elements[i + 2].Value += pairRightCandidate.Value;
                        }


                        // Add a zero in place of the pair
                        elements.RemoveRange(i, 2);
                        SnailfishElement explodedZero = new(0, SnailfishElement.ElementType.Isolated, pairLeftCandidate.Depth - 1);
                        elements.Insert(i, explodedZero);

                        // Pair the new isolated zero if necessary
                        TryFormNewPair(i);

                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Tries to split a regular number in the snailfish number.
            /// </summary>
            /// <returns>True if a split happened, false otherwise.</returns>
            private bool TrySplit()
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    SnailfishElement element = elements[i];

                    if (element.Value >= 10)
                    {
                        // Unlink its previous pairing
                        if (i != 0 && elements[i - 1].Type == SnailfishElement.ElementType.PairLeft)
                        {
                            elements[i - 1].Type = SnailfishElement.ElementType.Isolated;
                        }
                        else if (i != elements.Count - 1 && elements[i + 1].Type == SnailfishElement.ElementType.PairRight)
                        {
                            elements[i + 1].Type = SnailfishElement.ElementType.Isolated;
                        }

                        // Replace it with a new pair
                        SnailfishElement newLeft = new(element.Value / 2, SnailfishElement.ElementType.PairLeft, element.Depth + 1);
                        SnailfishElement newRight = new(element.Value - element.Value / 2, SnailfishElement.ElementType.PairRight, element.Depth + 1);
                        elements.RemoveAt(i);
                        elements.InsertRange(i, new SnailfishElement[] { newLeft, newRight });

                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Tries to form a pair with an isolated element.
            /// If the element cannot form a pair, this is a no-op.
            /// </summary>
            /// <param name="position">The position of the isolated element to form a pair with.</param>
            private void TryFormNewPair(int position)
            {
                if (position != 0 && elements[position - 1].Type == SnailfishElement.ElementType.Isolated & elements[position - 1].Depth == elements[position].Depth)
                {
                    // Form a new pair
                    elements[position - 1].Type = SnailfishElement.ElementType.PairLeft;
                    elements[position].Type = SnailfishElement.ElementType.PairRight;
                }
                else if (position != elements.Count - 1 && elements[position + 1].Type == SnailfishElement.ElementType.Isolated & elements[position + 1].Depth == elements[position].Depth)
                {
                    // Form a new pair
                    elements[position].Type = SnailfishElement.ElementType.PairLeft;
                    elements[position + 1].Type = SnailfishElement.ElementType.PairRight;
                }
            }
        }

        /// <summary>
        /// An element within the snailfish number.
        /// This class is not immutable, so it should be copied when necessary with <see cref="Copy"/>.
        /// </summary>
        private class SnailfishElement
        {
            /// <summary>
            /// The value of the element.
            /// </summary>
            public int Value;

            /// <summary>
            /// The type of element.
            /// </summary>
            public ElementType Type;

            /// <summary>
            /// The depth at which this element resides, ie. the pair 'level'.
            /// For example:
            ///     For the number [1, 2], 1 would be at depth 0.
            ///     For the number [[1, 2], 3], 1 would be at depth 1 and 3 would be at depth 0.
            /// </summary>
            public int Depth;

            /// <summary>
            /// Creates a new <see cref="SnailfishElement"/>.
            /// </summary>
            /// <param name="value">The initial value of the element.</param>
            /// <param name="type">The initial type of the element.</param>
            /// <param name="depth">The initial depth of the element.</param>
            public SnailfishElement(int value, ElementType type, int depth)
            {
                Value = value;
                Type = type;
                Depth = depth;
            }

            /// <summary>
            /// Copies the element.
            /// </summary>
            /// <returns>A copy of the element.</returns>
            public SnailfishElement Copy()
            {
                return new SnailfishElement(Value, Type, Depth);
            }

            public override string ToString()
            {
                return $"#{Value} ({Type}, Depth {Depth})";
            }

            /// <summary>
            /// A type of element, ie. whether it is paired or isolated.
            /// </summary>
            public enum ElementType
            {
                Isolated, PairLeft, PairRight
            }
        }
    }
}
