using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code
{
    /// <summary>
    /// A solution to a day's puzzle.
    /// </summary>
    internal abstract class AdventDay
    {
        /// <summary>
        /// A string to return from <see cref="SolvePuzzle1"/> or <see cref="SolvePuzzle2"/> when no solution has been found.
        /// </summary>
        protected const string NoSolutionFound = "No solution found.";

        /// <summary>
        /// Solves the day's first puzzle.
        /// </summary>
        /// <returns>The solution.</returns>
        internal abstract object? SolvePuzzle1();

        /// <summary>
        /// Solves the day's second puzzle.
        /// </summary>
        /// <returns>The solution.</returns>
        internal abstract object? SolvePuzzle2();

        /// <summary>
        /// Called between solving puzzle one and two to reset all state.
        /// </summary>
        public virtual void Reset()
        {

        }

        /// <summary>
        /// Reads input data from a file located in the Days folder. This attempts to read the file with the same name as the inheriting class.
        /// </summary>
        /// <returns>The whole file contents as a string.</returns>
        protected string GetInputData()
        {
            return File.ReadAllText($@"InputData\{GetType().Name}.txt");
        }

        /// <summary>
        /// Reads input data and splits it on a separator.
        /// </summary>
        /// <param name="separator">The separator to split on.</param>
        /// <returns>The file contents, split by the separator.</returns>
        /// <seealso cref="GetInputData"/>
        protected string[] GetInputData(string separator)
        {
            return GetInputData().Split(separator);
        }

        /// <summary>
        /// Reads input data and splits it on a separator.
        /// </summary>
        /// <typeparam name="T">The type of the selected data.</typeparam>
        /// <param name="separator">The separator to split on.</param>
        /// <param name="selector">The selector function.</param>
        /// <returns>The file contents, split by the separator and transformed by the selector.</returns>
        /// <seealso cref="GetInputData(string)"/>
        protected T[] GetInputData<T>(string separator, Func<string, T> selector)
        {
            return GetInputData(separator).Select(selector).ToArray();
        }
    }
}
