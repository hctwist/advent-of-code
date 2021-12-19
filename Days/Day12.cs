using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/12
    /// </summary>
    internal class Day12 : AdventDay
    {
        /// <summary>
        /// The cave map.
        /// </summary>
        private readonly CaveMap caveMap;

        public Day12()
        {
            string[] connectionStrings = GetInputData(Environment.NewLine);

            Dictionary<string, Cave> existingNodes = new();

            Cave start = new("start", false);
            Cave end = new("end", false);

            existingNodes[start.Name] = start;
            existingNodes[end.Name] = end;

            foreach (string connectionString in connectionStrings)
            {
                string[] split = connectionString.Split("-");
                (string connectionStart, string connectionEnd) = (split[0], split[1]);

                Cave connectionStartNode = existingNodes.GetValueOrDefault(connectionStart, new Cave(connectionStart, connectionStart.All(c => char.IsLower(c))));
                Cave connectionEndNode = existingNodes.GetValueOrDefault(connectionEnd, new Cave(connectionEnd, connectionEnd.All(c => char.IsLower(c))));

                connectionStartNode.Connections.Add(connectionEndNode);
                connectionEndNode.Connections.Add(connectionStartNode);

                existingNodes[connectionStartNode.Name] = connectionStartNode;
                existingNodes[connectionEndNode.Name] = connectionEndNode;
            }

            caveMap = new(start, end);
        }

        internal override void SolvePuzzle1()
        {
            int totalPaths = 0;

            foreach (Cave node in caveMap.Start.Connections)
            {
                totalPaths += FindNumberOfDistinctPathsWithOneSmallCaveVisit(node, new HashSet<Cave>());
            }

            WriteSolution1(totalPaths);
        }

        internal override void SolvePuzzle2()
        {
            int totalPaths = 0;

            foreach (Cave node in caveMap.Start.Connections)
            {
                totalPaths += FindNumberOfDistinctPathsWithAnExtraSmallCaveVisit(node, new HashSet<Cave>(), false);
            }

            WriteSolution2(totalPaths);
        }

        /// <summary>
        /// Finds the number of distinct paths through the cave system, without visiting any small caves more than once.
        /// The search starts from <paramref name="startingCave"/> which can be considered unvisited.
        /// </summary>
        /// <param name="startingCave">The node to start from. This can be considere unvisited at this point.</param>
        /// <param name="smallCavesVisited">The small caves that have already been visitied. This collection should never be modified.</param>
        /// <returns>The number of distinct paths.</returns>
        private int FindNumberOfDistinctPathsWithOneSmallCaveVisit(Cave startingCave, HashSet<Cave> smallCavesVisited)
        {
            if (startingCave == caveMap.End)
            {
                return 1;
            }

            if (startingCave == caveMap.Start)
            {
                return 0;
            }

            if (startingCave.IsSmall & smallCavesVisited.Contains(startingCave))
            {
                return 0;
            }

            int totalPaths = 0;

            foreach (Cave node in startingCave.Connections)
            {
                HashSet<Cave> updatedSmallCavesVisited = startingCave.IsSmall ? new(smallCavesVisited) { startingCave } : smallCavesVisited;
                totalPaths += FindNumberOfDistinctPathsWithOneSmallCaveVisit(node, updatedSmallCavesVisited);
            }

            return totalPaths;
        }

        /// <summary>
        /// Finds the number of distinct paths through the cave system, with only visiting a single small cave (at most) twice and no other small cave more than once.
        /// The search starts from <paramref name="startingCave"/> which can be considered unvisited.
        /// </summary>
        /// <param name="startingCave">The node to start from. This can be considere unvisited at this point.</param>
        /// <param name="smallCavesVisited">The small caves that have already been visitied. This collection should never be modified.</param>
        /// <param name="hasVisitedASmallCaveTwice">Whether a small cave has already been visited twice.</param>
        /// <returns>The number of distinct paths.</returns>
        private int FindNumberOfDistinctPathsWithAnExtraSmallCaveVisit(Cave startingCave, HashSet<Cave> smallCavesVisited, bool hasVisitedASmallCaveTwice)
        {
            if (startingCave == caveMap.End)
            {
                return 1;
            }

            if (startingCave == caveMap.Start)
            {
                return 0;
            }

            if (startingCave.IsSmall & smallCavesVisited.Contains(startingCave))
            {
                if (hasVisitedASmallCaveTwice)
                {
                    return 0;
                }
                else
                {
                    hasVisitedASmallCaveTwice = true;
                }
            }

            int totalPaths = 0;

            foreach (Cave node in startingCave.Connections)
            {
                HashSet<Cave> updatedSmallCavesVisited = startingCave.IsSmall ? new(smallCavesVisited) { startingCave } : smallCavesVisited;
                totalPaths += FindNumberOfDistinctPathsWithAnExtraSmallCaveVisit(node, updatedSmallCavesVisited, hasVisitedASmallCaveTwice);
            }

            return totalPaths;
        }

        /// <summary>
        /// A map of the cave.
        /// </summary>
        public class CaveMap
        {
            /// <summary>
            /// The starting cave.
            /// </summary>
            public readonly Cave Start;

            /// <summary>
            /// The end cave.
            /// </summary>
            public readonly Cave End;

            /// <summary>
            /// Creates a new <see cref="CaveMap"/>.
            /// </summary>
            /// <param name="start">The start cave.</param>
            /// <param name="end">The end cave.</param>
            public CaveMap(Cave start, Cave end)
            {
                Start = start;
                End = end;
            }
        }

        /// <summary>
        /// A cave in the cave system.
        /// </summary>
        public class Cave
        {
            /// <summary>
            /// The name of the cave.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// Whether the cave is small.
            /// </summary>
            public readonly bool IsSmall;

            /// <summary>
            /// The connections this cave has to others.
            /// </summary>
            public readonly HashSet<Cave> Connections;

            /// <summary>
            /// Creates a new <see cref="Cave"/>.
            /// </summary>
            /// <param name="name">The name of the cave.</param>
            /// <param name="isSmall">Whether the cave is small.</param>
            public Cave(string name, bool isSmall)
            {
                Name = name;
                IsSmall = isSmall;
                Connections= new HashSet<Cave>();
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
