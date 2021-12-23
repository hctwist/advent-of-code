using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/3
    /// </summary>
    internal class Day3 : AdventDay
    {
        /// <summary>
        /// The entries in the diagnostic report.
        /// </summary>
        private readonly List<string> entries;

        /// <summary>
        /// The number of bits that each entry in the diagnostic report contains.
        /// </summary>
        private readonly int bitCount;

        public Day3()
        {
            entries = GetInputData(Environment.NewLine).ToList();
            bitCount = entries[0].Length;
        }

        internal override object? SolvePuzzle1()
        {
            int gamma = 0;
            for (int i = 0; i < bitCount; i++)
            {
                if (GetGammaBit(entries, i) == 1)
                {
                    gamma++;
                }
                gamma <<= 1;
            }
            gamma >>= 1;

            int flipMask = (1 << bitCount) - 1;
            int epsilon = gamma ^ flipMask;

            return epsilon * gamma;
        }

        internal override object? SolvePuzzle2()
        {
            int bitCursor = 0;
            int bitLength = entries[0].Length;

            List<string> oxygenCandidates = new(entries);
            List<string> c02Candiates= new(entries);

            while (oxygenCandidates.Count > 1)
            {
                int gammaBit = GetGammaBit(oxygenCandidates, bitCursor);
                char targetBit = gammaBit == 0 ? '0' : '1';

                for (int i = oxygenCandidates.Count - 1; i >= 0; i--)
                {
                    if (targetBit != oxygenCandidates[i][bitCursor])
                    {
                        oxygenCandidates.RemoveAt(i);
                    }
                }

                bitCursor++;
                bitCursor %= bitLength;
            }

            bitCursor = 0;

            while (c02Candiates.Count > 1)
            {
                int gammaBit = GetGammaBit(c02Candiates, bitCursor);
                char targetBit = gammaBit == 0 ? '1' : '0';

                for (int i = c02Candiates.Count - 1; i >= 0; i--)
                {
                    if (targetBit != c02Candiates[i][bitCursor])
                    {
                        c02Candiates.RemoveAt(i);
                    }
                }

                bitCursor++;
                bitCursor %= bitLength;
            }

            int lifeSupportRating = Convert.ToInt32(oxygenCandidates[0], 2) * Convert.ToInt32(c02Candiates[0], 2);
            return lifeSupportRating;
        }

        /// <summary>
        /// Computes the 'gamma bit' which is:
        /// 1 - if 1 is the most common bit in the specified position
        /// 0 - if 0 is the most common bit
        /// -1 - if there are an equal number of zeroes and ones.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="position">The position to check.</param>
        /// <returns>The computed 'gamma bit'</returns>
        private static int GetGammaBit(IEnumerable<string> report, int position)
        {
            int setBitCount = 0;
            int reportCount = 0;

            foreach (string entry in report)
            {
                if (entry[position] == '1')
                {
                    setBitCount++;
                }

                reportCount++;
            }

            if (setBitCount * 2 == reportCount)
            {
                return -1;
            }
            else if (setBitCount * 2 > reportCount)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
