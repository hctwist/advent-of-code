using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/21
    /// </summary>
    internal class Day21 : AdventDay
    {
        /// <summary>
        /// The starting state of the game.
        /// </summary>
        private readonly GameState startingState;

        public Day21()
        {
            string[] input = GetInputData(Environment.NewLine);

            string pattern = @"Player \d+ starting position: (\d+)";

            int player1StartingSpace = int.Parse(Regex.Match(input[0], pattern).Groups[1].Value);
            int player2StartingSpace = int.Parse(Regex.Match(input[1], pattern).Groups[1].Value);

            startingState = new GameState(
                new PlayerState(0, player1StartingSpace),
                new PlayerState(0, player2StartingSpace),
                1);
        }

        internal override void SolvePuzzle1()
        {
            DeterministicDie die = new();

            GameState state = startingState;
            int totalDiceRolls = 0;

            while (true)
            {
                int roll = die.Roll() + die.Roll() + die.Roll();
                totalDiceRolls += 3;

                state = state.TakeTurn(roll);

                if (state.Player1State.Score >= 1000)
                {
                    WriteSolution1(totalDiceRolls * state.Player2State.Score);
                    break;
                }
                else if (state.Player2State.Score >= 1000)
                {
                    WriteSolution1(totalDiceRolls * state.Player1State.Score);
                    break;
                }
            }
        }

        internal override void SolvePuzzle2()
        {
            QuantumGame game = new(startingState);
            QuantumGame.Wins wins = game.CountPlayerWins();
            WriteSolution2(Math.Max(wins.Player1Wins, wins.Player2Wins));
        }

        /// <summary>
        /// A quantum game of Dirac Dice.
        /// </summary>
        private class QuantumGame
        {
            /// <summary>
            /// The starting state of the game.
            /// </summary>
            private readonly GameState startingState;

            /// <summary>
            /// The already observed outcomes.
            /// </summary>
            private readonly Dictionary<GameState, Wins> observedOutcomes;

            /// <summary>
            /// Create a new <see cref="QuantumGame"/>.
            /// </summary>
            /// <param name="startingState">The starting state.</param>
            public QuantumGame(GameState startingState)
            {
                this.startingState = startingState;
                observedOutcomes = new Dictionary<GameState, Wins>();
            }

            /// <summary>
            /// Counts the number of wins by each player.
            /// </summary>
            /// <returns>The wins.</returns>
            public Wins CountPlayerWins()
            {
                return CountPlayerWins(startingState);
            }

            /// <summary>
            /// Counts the number of wins by each player.
            /// </summary>
            /// <param name="state">The state of the game to start from.</param>
            /// <returns>The wins.</returns>
            private Wins CountPlayerWins(GameState state)
            {
                if (observedOutcomes.TryGetValue(state, out Wins wins))
                {
                    return wins;
                }

                if (state.Player1State.Score >= 21)
                {
                    return new Wins(1, 0);
                }

                if (state.Player2State.Score >= 21)
                {
                    return new Wins(0, 1);
                }

                Wins winCount = new(0, 0);

                for (int i = 1; i <= 3; i++)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        for (int k = 1; k <= 3; k++)
                        {
                            GameState newState = state.TakeTurn(i + j + k);
                            winCount += CountPlayerWins(newState);
                        }
                    }
                }

                observedOutcomes[state] = winCount;

                return winCount;
            }

            /// <summary>
            /// Each player's number of wins.
            /// </summary>
            public struct Wins
            {
                /// <summary>
                /// The number of times player 1 has won.
                /// </summary>
                public readonly long Player1Wins;

                /// <summary>
                /// The number of times player 2 has won.
                /// </summary>
                public readonly long Player2Wins;

                /// <summary>
                /// Creates a new <see cref="Wins"/>.
                /// </summary>
                /// <param name="player1Wins">Player 1's wins.</param>
                /// <param name="player2Wins">Player 2's wins.</param>
                public Wins(long player1Wins, long player2Wins)
                {
                    Player1Wins = player1Wins;
                    Player2Wins = player2Wins;
                }

                /// <summary>
                /// Adds two <see cref="Wins"/> together.
                /// </summary>
                /// <param name="win1">The first win.</param>
                /// <param name="win2">The second win.</param>
                /// <returns>The sum of <paramref name="win1"/> and <paramref name="win2"/>.</returns>
                public static Wins operator +(Wins win1, Wins win2)
                {
                    return new Wins(win1.Player1Wins + win2.Player1Wins, win1.Player2Wins + win2.Player2Wins);
                }
            }
        }

        /// <summary>
        /// The state of a game of Dirac Dice.
        /// </summary>
        private struct GameState
        {
            /// <summary>
            /// Player 1's state.
            /// </summary>
            public readonly PlayerState Player1State;

            /// <summary>
            /// Player 2's state.
            /// </summary>
            public readonly PlayerState Player2State;

            /// <summary>
            /// The next player to take a turn. This should be either 1 or 2.
            /// </summary>
            public readonly int NextPlayer;

            /// <summary>
            /// Creates a new <see cref="GameState"/>.
            /// </summary>
            /// <param name="player1State">Player 1's state.</param>
            /// <param name="player2State">Player 2's state.</param>
            /// <param name="nextPlayer">The next player.</param>
            public GameState(PlayerState player1State, PlayerState player2State, int nextPlayer)
            {
                Player1State = player1State;
                Player2State = player2State;
                NextPlayer = nextPlayer;
            }

            /// <summary>
            /// Computes the game state after a single turn is taken in the current state.
            /// This does not mutate the current state.
            /// </summary>
            /// <param name="diceRoll">The total dice roll.</param>
            /// <returns>The new state after a turn.</returns>
            /// <exception cref="InvalidOperationException">Thrown if <see cref="NextPlayer"/> is not valid, ie. if it isn't 1 or 2.</exception>
            public GameState TakeTurn(int diceRoll)
            {
                PlayerState newPlayer1State;
                PlayerState newPlayer2State;
                int newNextPlayer;

                if (NextPlayer == 1)
                {
                    int newSpace = GetSpace(Player1State, diceRoll);
                    newPlayer1State = new PlayerState(Player1State.Score + newSpace, newSpace);
                    newPlayer2State = Player2State;
                    newNextPlayer = 2;
                }
                else if (NextPlayer == 2)
                {
                    int newSpace = GetSpace(Player2State, diceRoll);
                    newPlayer1State = Player1State;
                    newPlayer2State = new PlayerState(Player2State.Score + newSpace, newSpace);
                    newNextPlayer = 1;
                }
                else
                {
                    throw new InvalidOperationException($"Tried to take a turn with an invalid next player {NextPlayer}.");
                }

                return new GameState(newPlayer1State, newPlayer2State, newNextPlayer);
            }

            /// <summary>
            /// Gets the new space after a dice roll.
            /// </summary>
            /// <param name="playerState">The player's current state.</param>
            /// <param name="diceRoll">The total dice roll.</param>
            /// <returns>The new space.</returns>
            private static int GetSpace(PlayerState playerState, int diceRoll)
            {
                int newSpace = playerState.Space + diceRoll;
                newSpace = ((newSpace - 1) % 10) + 1;

                return newSpace;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Player1State.Score, Player1State.Space, Player2State.Score, Player2State.Space, NextPlayer);
            }
        }

        /// <summary>
        /// A player's state during a game of Dirac Dice.
        /// </summary>
        private struct PlayerState
        {
            /// <summary>
            /// The player's score.
            /// </summary>
            public readonly int Score;

            /// <summary>
            /// The space that the player's pawn is currently on.
            /// </summary>
            public readonly int Space;

            /// <summary>
            /// Creates a new <see cref="PlayerState"/>.
            /// </summary>
            /// <param name="score">The player's score.</param>
            /// <param name="space">The player's pawn's space.</param>
            public PlayerState(int score, int space)
            {
                Score = score;
                Space = space;
            }
        }

        /// <summary>
        /// A deterministic dice.
        /// This rolls incrementing numbers from 1 to 100, looping back round to 1 after exceeding 100.
        /// </summary>
        private class DeterministicDie
        {
            /// <summary>
            /// The last dice roll.
            /// </summary>
            private int lastRoll;

            /// <summary>
            /// Creates a new <see cref="DeterministicDie"/>.
            /// </summary>
            public DeterministicDie()
            {
                lastRoll = 0;
            }

            /// <summary>
            /// Rolls the dice.
            /// </summary>
            /// <returns>The roll.</returns>
            public int Roll()
            {
                lastRoll = ++lastRoll == 101 ? 1 : lastRoll;
                return lastRoll;
            }
        }
    }
}
