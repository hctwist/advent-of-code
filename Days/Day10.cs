using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/10
    /// </summary>
    internal class Day10 : AdventDay
    {
        /// <summary>
        /// Lines of chunk characters.
        /// </summary>
        private readonly List<List<ChunkCharacter>> lines;

        public Day10()
        {
            string[] input = GetInputData(Environment.NewLine);

            lines = new List<List<ChunkCharacter>>(input.Length);

            foreach (string inputLine in input)
            {
                List<ChunkCharacter> characters = new(inputLine.Length);

                foreach (char inputCharacter in inputLine)
                {
                    characters.Add(Parse(inputCharacter));
                }

                lines.Add(characters);
            }
        }

        /// <summary>
        /// Parses a character into a <see cref="ChunkCharacter"/>.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The matching <see cref="ChunkCharacter"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="c"/> isn't a valid chunk character.</exception>
        private static ChunkCharacter Parse(char c)
        {
            return c switch
            {
                '(' => new ChunkCharacter(ChunkCharacterClass.Parentheses, false),
                ')' => new ChunkCharacter(ChunkCharacterClass.Parentheses, true),
                '[' => new ChunkCharacter(ChunkCharacterClass.Brackets, false),
                ']' => new ChunkCharacter(ChunkCharacterClass.Brackets, true),
                '{' => new ChunkCharacter(ChunkCharacterClass.Braces, false),
                '}' => new ChunkCharacter(ChunkCharacterClass.Braces, true),
                '<' => new ChunkCharacter(ChunkCharacterClass.Tags, false),
                '>' => new ChunkCharacter(ChunkCharacterClass.Tags, true),
                _ => throw new ArgumentException($"Cannot parse {c}")
            };
        }

        internal override void SolvePuzzle1()
        {
            int score = 0;

            foreach (List<ChunkCharacter> line in lines)
            {
                if (TryFindFirstIllegalCharacter(line, out ChunkCharacterClass illegalCharacter, out _))
                {
                    score += illegalCharacter.SyntaxCheckerScore;
                }
            }

            WriteSolution1(score);
        }

        internal override void SolvePuzzle2()
        {
            List<long> scores = new(lines.Count);

            foreach (List<ChunkCharacter> line in lines)
            {
                if (!TryFindFirstIllegalCharacter(line, out _, out Stack<ChunkCharacter> unmatchedCharacters))
                {
                    long score = 0;

                    while (unmatchedCharacters.Count > 0)
                    {
                        score *= 5;
                        score += unmatchedCharacters.Pop().Character.AutocompleteToolScore;
                    }

                    scores.Add(score);
                }
            }

            scores.Sort();

            WriteSolution2(scores[scores.Count / 2]);
        }

        /// <summary>
        /// Trys to find the first illegal character that appears on a single line.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <param name="illegalCharacter">The illegal character found.</param>
        /// <param name="unmatchedCharacters">The stack of unmatched characters up until the illegal character was found.</param>
        /// <returns>True if an illegal character was found, false otherwise.</returns>
        private static bool TryFindFirstIllegalCharacter(List<ChunkCharacter> line, out ChunkCharacterClass illegalCharacter, out Stack<ChunkCharacter> unmatchedCharacters)
        {
            unmatchedCharacters = new Stack<ChunkCharacter>();

            foreach (ChunkCharacter character in line)
            {
                if (character.IsClosing)
                {
                    // We need a matching opening character on the stack
                    if (!character.Matches(unmatchedCharacters.Pop()))
                    {
                        illegalCharacter = character.Character;
                        return true;
                    }
                }
                else
                {
                    unmatchedCharacters.Push(character);
                }
            }

            illegalCharacter = ChunkCharacterClass.Brackets;
            return false;
        }

        /// <summary>
        /// An opening or closing chunk character.
        /// </summary>
        private class ChunkCharacter
        {
            /// <summary>
            /// The character class.
            /// </summary>
            public readonly ChunkCharacterClass Character;

            /// <summary>
            /// Whether the character is closing.
            /// </summary>
            public readonly bool IsClosing;

            /// <summary>
            /// Creates a new <see cref="ChunkCharacter"/>.
            /// </summary>
            /// <param name="character">The character class.</param>
            /// <param name="isClosing">Whether the character is closing.</param>
            public ChunkCharacter(ChunkCharacterClass character, bool isClosing)
            {
                Character = character;
                IsClosing = isClosing;
            }

            /// <summary>
            /// Checks whether a character matches another.
            /// Two characters match if they form an opening and closing pair of the same character class.
            /// </summary>
            /// <param name="matchingCandidate">The other character to check.</param>
            /// <returns>True if <paramref name="matchingCandidate"/> matches, false otherwise.</returns>
            public bool Matches(ChunkCharacter matchingCandidate)
            {
                return IsClosing != matchingCandidate.IsClosing & Character == matchingCandidate.Character;
            }
        }

        /// <summary>
        /// Represents a class of chunk character, ie. parenteses. This is invariant to whether the character is a closing character.
        /// </summary>
        private class ChunkCharacterClass
        {
            /// <summary>
            /// A character class for parentheses - ()
            /// </summary>
            public static readonly ChunkCharacterClass Parentheses = new('(', ')', 3, 1);

            /// <summary>
            /// A character class for brackets - []
            /// </summary>
            public static readonly ChunkCharacterClass Brackets = new('[', ']', 57, 2);

            /// <summary>
            /// A character class for braces - {}
            /// </summary>
            public static readonly ChunkCharacterClass Braces = new('{', '}', 1197, 3);

            /// <summary>
            /// A character class for tags - <>
            /// </summary>
            public static readonly ChunkCharacterClass Tags = new('<', '>', 25137, 4);

            /// <summary>
            /// The opening character of this class.
            /// </summary>
            public readonly char OpeningCharacter;

            /// <summary>
            /// The closing character of this class.
            /// </summary>
            public readonly char ClosingCharacter;

            /// <summary>
            /// The score of this character class in a syntax checker contest.
            /// </summary>
            public readonly int SyntaxCheckerScore;

            /// <summary>
            /// The score of this character class in an autocomplete tool contest.
            /// </summary>
            public readonly int AutocompleteToolScore;

            /// <summary>
            /// Creates a new <see cref="ChunkCharacterClass"/>.
            /// </summary>
            /// <param name="openingCharacter">The opening character.</param>
            /// <param name="closingCharacter">The closing character.</param>
            /// <param name="syntaxCheckerScore">The score of this class.</param>
            private ChunkCharacterClass(char openingCharacter, char closingCharacter, int syntaxCheckerScore, int autocompleteToolScore)
            {
                OpeningCharacter = openingCharacter;
                ClosingCharacter = closingCharacter;
                SyntaxCheckerScore = syntaxCheckerScore;
                AutocompleteToolScore = autocompleteToolScore;
            }
        }
    }
}
