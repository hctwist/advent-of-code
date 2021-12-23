using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/14
    /// </summary>
    internal class Day14 : AdventDay
    {
        /// <summary>
        /// The initial polymer template.
        /// </summary>
        private readonly string template;

        /// <summary>
        /// The insertion rules.
        /// </summary>
        private readonly List<InsertionRule> insertionRules;

        public Day14()
        {
            string[] input = GetInputData(Environment.NewLine);

            template = input[0];
            insertionRules = new List<InsertionRule>();

            foreach (string inputLine in input.Skip(2))
            {
                Match match = Regex.Match(inputLine, "([A-Z])([A-Z]) -> ([A-Z])");

                InsertionRule rule = new(new TemplatePair(match.Groups[1].Value[0], match.Groups[2].Value[0]), match.Groups[3].Value[0]);
                insertionRules.Add(rule);
            }
        }

        internal override object? SolvePuzzle1()
        {
            return GetCharacterCountRangeAfterInsertions(10);
        }

        internal override object? SolvePuzzle2()
        {
            return GetCharacterCountRangeAfterInsertions(40);
        }

        /// <summary>
        /// Applies the insertion rules to all pairs in the original polymer, then computes the difference between the highest and lowest character count.
        /// </summary>
        /// <param name="steps">The number of times to apply the insertions.</param>
        /// <returns>The difference between the highest and lowest character count.</returns>
        private long GetCharacterCountRangeAfterInsertions(int steps)
        {
            Dictionary<TemplatePair, long> pairCounts = new();

            for (int i = 0; i < template.Length - 1; i++)
            {
                IncrementValue(pairCounts, new TemplatePair(template[i], template[i + 1]), 1);
            }

            for (int i = 0; i < steps; i++)
            {
                ApplyInsertionRules(pairCounts);
            }

            // These character counts are exactly double the true template character counts, except from the start and end characters of the original template, which are one less than double
            Dictionary<char, long> characterCounts = GetCharacterCounts(pairCounts);

            char firstTemplateCharacter = template.First();
            char lastTemplateCharacter = template.Last();

            IEnumerable<long> trueCharacterCounts = characterCounts.Select(count => count.Key == firstTemplateCharacter | count.Key == lastTemplateCharacter ? (count.Value + 1) / 2 : count.Value / 2);

            return trueCharacterCounts.Max() - trueCharacterCounts.Min();
        }

        /// <summary>
        /// Counts characters from character pairs.
        /// </summary>
        /// <param name="pairCounts">The counts of how many times each pair occurs.</param>
        /// <returns>The count for each character.</returns>
        private static Dictionary<char, long> GetCharacterCounts(Dictionary<TemplatePair, long> pairCounts)
        {
            Dictionary<char, long> characterCounts = new();

            foreach (KeyValuePair<TemplatePair, long> pair in pairCounts)
            {
                IncrementValue(characterCounts, pair.Key.FirstCharacter, pair.Value);
                IncrementValue(characterCounts, pair.Key.SecondCharacter, pair.Value);
            }

            return characterCounts;
        }

        /// <summary>
        /// Applies the insertion rules to the polymer.
        /// </summary>
        /// <param name="pairCounts">The current state of the polymer, expressed as counts of the pairs in the polymer.</param>
        private void ApplyInsertionRules(Dictionary<TemplatePair, long> pairCounts)
        {
            Dictionary<TemplatePair, long> pairCountDiffs = new();

            foreach (KeyValuePair<TemplatePair, long> pairCount in pairCounts)
            {
                if (pairCount.Value == 0)
                {
                    continue;
                }

                TemplatePair pair = pairCount.Key;

                foreach (InsertionRule rule in insertionRules)
                {
                    if (rule.From.Equals(pairCount.Key))
                    {
                        // Remove the old pair
                        IncrementValue(pairCountDiffs, pair, -pairCount.Value);

                        // Add in the two new pairs
                        TemplatePair newLeftPair = new(pair.FirstCharacter, rule.To);
                        TemplatePair newRightPair = new(rule.To, pair.SecondCharacter);

                        IncrementValue(pairCountDiffs, newLeftPair, pairCount.Value);
                        IncrementValue(pairCountDiffs, newRightPair, pairCount.Value);

                        break;
                    }
                }
            }

            // Apply the diff
            foreach (KeyValuePair<TemplatePair, long> diff in pairCountDiffs)
            {
                if (diff.Value != 0)
                {
                    IncrementValue(pairCounts, diff.Key, diff.Value);
                }
            }
        }

        /// <summary>
        /// Increments a value in a dictionary. If the key doesn't exist, it's assumed to have a value of 0.
        /// </summary>
        /// <typeparam name="T">The key type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="by">How much to increment by. This can be negative.</param>
        private static void IncrementValue<T>(Dictionary<T, long> dictionary, T key, long by) where T : notnull
        {
            dictionary[key] = dictionary.GetValueOrDefault(key, 0) + by;
        }

        /// <summary>
        /// An insertion rule.
        /// </summary>
        private struct InsertionRule
        {
            /// <summary>
            /// The template pair this rule applies to.
            /// </summary>
            public readonly TemplatePair From;

            /// <summary>
            /// The insertion character.
            /// </summary>
            public readonly char To;

            /// <summary>
            /// Creates a new <see cref="InsertionRule"/>.
            /// </summary>
            /// <param name="pair">The pair.</param>
            /// <param name="insertedElement">The insertion character.</param>
            public InsertionRule(TemplatePair pair, char insertedElement)
            {
                From = pair;
                To = insertedElement;
            }
        }

        /// <summary>
        /// A pair of characters.
        /// </summary>
        private struct TemplatePair
        {
            /// <summary>
            /// The first character.
            /// </summary>
            public readonly char FirstCharacter;

            /// <summary>
            /// The second character.
            /// </summary>
            public readonly char SecondCharacter;

            /// <summary>
            /// Creates a new <see cref="TemplatePair"/>.
            /// </summary>
            /// <param name="firstCharacter">The first character.</param>
            /// <param name="secondCharacter">The second character.</param>
            public TemplatePair(char firstCharacter, char secondCharacter)
            {
                FirstCharacter = firstCharacter;
                SecondCharacter = secondCharacter;
            }

            public override string ToString()
            {
                return $"{FirstCharacter}{SecondCharacter}";
            }
        }
    }
}
