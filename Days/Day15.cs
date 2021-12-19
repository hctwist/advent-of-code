using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/15
    /// </summary>
    internal class Day15 : AdventDay
    {
        /// <summary>
        /// The risk levels.
        /// </summary>
        private readonly int[,] riskLevels;

        public Day15()
        {
            string[] input = GetInputData(Environment.NewLine);

            riskLevels = new int[input.Length, input[0].Length];

            for (int row = 0; row < riskLevels.GetLength(0); row++)
            {
                for (int column = 0; column < riskLevels.GetLength(1); column++)
                {
                    riskLevels[row, column] = CharUnicodeInfo.GetDecimalDigitValue(input[row][column]);
                }
            }
        }

        internal override void SolvePuzzle1()
        {
            Node[,] nodes = new Node[riskLevels.GetLength(0), riskLevels.GetLength(1)];

            for (int row = 0; row < riskLevels.GetLength(0); row++)
            {
                for (int column = 0; column < riskLevels.GetLength(1); column++)
                {
                    nodes[row, column] = new Node(riskLevels[row, column]);
                }
            }

            WriteSolution1(FindShortestPath(nodes));
        }

        internal override void SolvePuzzle2()
        {
            Node[,] nodes = new Node[riskLevels.GetLength(0) * 5, riskLevels.GetLength(1) * 5];

            for (int tileX = 0; tileX < 5; tileX++)
            {
                for (int tileY = 0; tileY < 5; tileY++)
                {
                    for (int row = 0; row < riskLevels.GetLength(0); row++)
                    {
                        for (int column = 0; column < riskLevels.GetLength(1); column++)
                        {
                            int offsetRow = tileX * riskLevels.GetLength(0) + row;
                            int offsetColumn = tileY * riskLevels.GetLength(1) + column;

                            int riskLevel = riskLevels[row, column] + tileX + tileY;
                            if (riskLevel > 9)
                            {
                                riskLevel -= 9;
                            }

                            nodes[offsetRow, offsetColumn] = new Node(riskLevel);
                        }
                    }
                }
            }

            WriteSolution2(FindShortestPath(nodes));
        }

        /// <summary>
        /// Finds the path with the lowest risk.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The risk of the best path.</returns>
        private static int FindShortestPath(Node[,] nodes)
        {
            // Dijkstra's algorithm  https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

            nodes[0, 0].Visited = true;
            nodes[0, 0].RouteRisk = 0;

            HashSet<(int, int)> unvisitedNodeIndicesWithRouteRisk = new(nodes.Length);

            int currentRow = 0;
            int currentColumn = 0;

            while (true)
            {
                Node currentNode = nodes[currentRow, currentColumn];

                IEnumerable<(int, int)> unvisitedAdjacentNodeIndices = new (int, int)[4]
                {
                    (currentRow, currentColumn - 1),
                    (currentRow - 1, currentColumn),
                    (currentRow, currentColumn + 1),
                    (currentRow + 1, currentColumn)
                }.Where(i => !IsOutOfBoundsOrVisited(nodes, i.Item1, i.Item2));

                // Consider all unvisited neighbours
                foreach ((int, int) unvisitedNodeIndex in unvisitedAdjacentNodeIndices)
                {
                    Node node = nodes[unvisitedNodeIndex.Item1, unvisitedNodeIndex.Item2];
                    node.RouteRisk = Math.Min(node.RouteRisk, currentNode.RouteRisk + node.Risk);
                    unvisitedNodeIndicesWithRouteRisk.Add(unvisitedNodeIndex);
                }

                currentNode.Visited = true;
                unvisitedNodeIndicesWithRouteRisk.Remove((currentRow, currentColumn));

                // If we have reached the last node or considered all nodes, we have found the best route
                Node lastNode = nodes[nodes.GetLength(0) - 1, nodes.GetLength(1) - 1];
                if (lastNode.Visited | unvisitedNodeIndicesWithRouteRisk.Count == 0)
                {
                    return lastNode.RouteRisk;
                }

                // Start at the adjacent node with the smallest risk
                (currentRow, currentColumn) = unvisitedNodeIndicesWithRouteRisk.MinBy(i => nodes[i.Item1, i.Item2].RouteRisk);
            }
        }

        /// <summary>
        /// Checks whether a node is either out of bounds or visited.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="row">The row of the node to check.</param>
        /// <param name="column">The column of the node to check.</param>
        /// <returns>True if the row or column is out of bounds, or the node has been visited. False otherwise.</returns>
        private static bool IsOutOfBoundsOrVisited(Node[,] nodes, int row, int column)
        {
            return row < 0 | column < 0 | row > nodes.GetLength(0) - 1 | column > nodes.GetLength(1) - 1 || nodes[row, column].Visited;
        }

        /// <summary>
        /// A node in the cave system, with its risk level.
        /// </summary>
        private class Node
        {
            /// <summary>
            /// The risk level.
            /// </summary>
            public readonly int Risk;

            /// <summary>
            /// The best risk level achieved en-route to this node.
            /// </summary>
            public int RouteRisk;

            /// <summary>
            /// Whether this node has been visited.
            /// </summary>
            public bool Visited;

            /// <summary>
            /// Creates a new <see cref="Node"/>.
            /// On creation, <see cref="RouteRisk"/> will be <see cref="int.MaxValue"/> and <see cref="Visited"/> will be false.
            /// </summary>
            /// <param name="risk">The risk level.</param>
            public Node(int risk)
            {
                Risk = risk;

                RouteRisk = int.MaxValue;
                Visited = false;
            }
        }
    }
}
