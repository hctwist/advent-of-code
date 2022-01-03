using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/23
    /// </summary>
    internal class Day23 : AdventDay
    {
        internal override object? SolvePuzzle1()
        {
            //#############
            //#...........#
            //###B#A#A#D###
            //# B#C#D#C#
            //#########

            //#############2000
            //#.........D.#
            //###B#A#A#.###
            //# B#C#D#C#
            //#########

            //#############5+6
            //# AA.......D.#
            //###B#.#.#.###
            //# B#C#D#C#
            //#########

            //#############500
            //# AA...C...D.#
            //###B#.#.#.###
            //# B#C#D#.#
            //#########

            //#############3000+5000
            //# AA...C.....#
            //###B#.#.#D###
            //# B#C#.#D#
            //#########

            //#############300+500
            //# AA.........#
            //###B#.#C#D###
            //# B#.#C#D#
            //#########

            //#############50+50
            //# AA.........#
            //###.#B#C#D###
            //#.#B#C#D#
            //#########

            //#############3+3
            //#...........#
            //###A#B#C#D###
            //# A#B#C#D#
            //#########

            return 11_417;
        }

        internal override object? SolvePuzzle2()
        {
            Dictionary<int, Type> hallway = new();

            Stack<Type> room1Amphipods = new();
            room1Amphipods.Push(Type.Bronze);
            room1Amphipods.Push(Type.Desert);
            room1Amphipods.Push(Type.Desert);
            room1Amphipods.Push(Type.Bronze);

            Stack<Type> room2Amphipods = new();
            room2Amphipods.Push(Type.Copper);
            room2Amphipods.Push(Type.Bronze);
            room2Amphipods.Push(Type.Copper);
            room2Amphipods.Push(Type.Amber);

            Stack<Type> room3Amphipods = new();
            room3Amphipods.Push(Type.Desert);
            room3Amphipods.Push(Type.Amber);
            room3Amphipods.Push(Type.Bronze);
            room3Amphipods.Push(Type.Amber);

            Stack<Type> room4Amphipods = new();
            room4Amphipods.Push(Type.Copper);
            room4Amphipods.Push(Type.Copper);
            room4Amphipods.Push(Type.Amber);
            room4Amphipods.Push(Type.Desert);

            List<Room> rooms = new()
            {
                new Room(2, Type.Amber, room1Amphipods),
                new Room(4, Type.Bronze, room2Amphipods),
                new Room(6, Type.Copper, room3Amphipods),
                new Room(8, Type.Desert, room4Amphipods)
            };

            StepState state = new(hallway, rooms, 0);
            return Simulate(state);
        }

        /// <summary>
        /// Simulates amphipod moves to find the least amount of energy needed to organise them.
        /// </summary>
        /// <param name="startingState">The state to start the simulation from.</param>
        /// <returns>The least amount of energy required to organise the amphipods.</returns>
        private static int Simulate(StepState startingState)
        {
            // Keep a stack of states to evaluate
            Stack<StepState> states = new();
            states.Push(startingState);

            HashSet<int> roomHallwayPositions = new(startingState.Rooms.Select(room => room.HallwayPosition));

            int minimumEnergyUsed = int.MaxValue;

            while (states.Count > 0)
            {
                StepState state = states.Pop();

                if (state.EnergyUsed > minimumEnergyUsed | state.EnergyUsed > 51_000)
                {
                    continue;
                }

                if (IsComplete(state))
                {
                    minimumEnergyUsed = Math.Min(minimumEnergyUsed, state.EnergyUsed);
                }

                // The only amphipods that can move are the ones at the top of each room, or ones in the hallway

                // Consider the amphipods at the top of each room and only consider moves to the hallway
                for (int r = 0; r < state.Rooms.Count; r++)
                {
                    Room room = state.Rooms[r];

                    // If the room is empty or if all it's occupants are of the correct type, we can ignore this room
                    if (room.Amphipods.Count == 0 || room.Amphipods.All(a => a == room.Type))
                    {
                        continue;
                    }

                    Type amphipodToMove = room.Amphipods.Peek();
                    int energyToMoveAmphipod = GetEnergyToMove(amphipodToMove);
                    int energyToLeaveRoom = energyToMoveAmphipod * (4 - room.Amphipods.Count + 1);

                    // Consider all moves to the left
                    for (int i = room.HallwayPosition - 1; i >= 0; i--)
                    {
                        // This move would put the amphipod infront of a room
                        if (roomHallwayPositions.Contains(i))
                        {
                            continue;
                        }

                        // Check if we're encountering another amphipod. If so, stop completely
                        if (state.Hallway.ContainsKey(i))
                        {
                            break;
                        }

                        // Otherwise, this is a valid move
                        int energyToMove = energyToLeaveRoom + (room.HallwayPosition - i) * energyToMoveAmphipod;

                        StepState newState = state.Copy();
                        newState.Rooms[r].Amphipods.Pop();
                        newState.Hallway[i] = amphipodToMove;
                        newState.EnergyUsed += energyToMove;

                        states.Push(newState);
                    }

                    // Consider all moves to the right
                    for (int i = room.HallwayPosition + 1; i <= 10; i++)
                    {
                        // This move would put the amphipod infront of a room
                        if (roomHallwayPositions.Contains(i))
                        {
                            continue;
                        }

                        // Check if we're encountering another amphipod. If so, stop completely
                        if (state.Hallway.ContainsKey(i))
                        {
                            break;
                        }

                        // Otherwise, this is a valid move
                        int energyToMove = energyToLeaveRoom + (i - room.HallwayPosition) * energyToMoveAmphipod;

                        StepState newState = state.Copy();
                        newState.Rooms[r].Amphipods.Pop();
                        newState.Hallway[i] = amphipodToMove;
                        newState.EnergyUsed += energyToMove;

                        states.Push(newState);
                    }
                }

                // Consider moving the amphipods in the hallway
                foreach (KeyValuePair<int, Type> amphipodInHallway in state.Hallway)
                {
                    // Check if the amphipod can move into each room
                    for (int r = 0; r < state.Rooms.Count; r++)
                    {
                        Room room = state.Rooms[r];

                        // Check it is the correct room and it the room doesn't have any unorganised amphipods
                        if (room.Type != amphipodInHallway.Value || room.Amphipods.Any(a => a != room.Type))
                        {
                            continue;
                        }

                        // Check there aren't any amphipods in the way of the move
                        bool amphipodIsInWay = false;

                        // Left move
                        if (room.HallwayPosition < amphipodInHallway.Key)
                        {
                            for (int i = amphipodInHallway.Key - 1; i >= room.HallwayPosition; i--)
                            {
                                if (state.Hallway.ContainsKey(i))
                                {
                                    amphipodIsInWay = true;
                                    break;
                                }
                            }
                        }
                        // Right move
                        else
                        {
                            for (int i = amphipodInHallway.Key + 1; i <= room.HallwayPosition; i++)
                            {
                                if (state.Hallway.ContainsKey(i))
                                {
                                    amphipodIsInWay = true;
                                    break;
                                }
                            }
                        }

                        if (amphipodIsInWay)
                        {
                            continue;
                        }

                        // We can move
                        int energyToMoveAmphipod = GetEnergyToMove(amphipodInHallway.Value);
                        int energyToMove =
                            Math.Abs(room.HallwayPosition - amphipodInHallway.Key) * energyToMoveAmphipod +
                            (4 - room.Amphipods.Count) * energyToMoveAmphipod;

                        StepState newState = state.Copy();

                        newState.Hallway.Remove(amphipodInHallway.Key);
                        newState.Rooms[r].Amphipods.Push(amphipodInHallway.Value);
                        newState.EnergyUsed += energyToMove;

                        states.Push(newState);
                    }
                }
            }

            return minimumEnergyUsed;
        }

        /// <summary>
        /// Gets the amount of energy required to move a certain type of amphipod.
        /// </summary>
        /// <param name="type">The type of amphipod.</param>
        /// <returns>The amount of energy.</returns>
        private static int GetEnergyToMove(Type type)
        {
            return type switch
            {
                Type.Amber => 1,
                Type.Bronze => 10,
                Type.Copper => 100,
                Type.Desert => 1_000,
                _ => throw new ArgumentException($"The amphipod type {type} is not valid.")
            };
        }

        /// <summary>
        /// Checks whether a state is complete, ie. the amphipods are all organised.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns>True if all amphipods are organised, false otherwise.</returns>
        private static bool IsComplete(StepState state)
        {
            return state.Rooms.All(room => room.IsComplete());
        }

        /// <summary>
        /// The state of a step of the simulation.
        /// </summary>
        private class StepState
        {
            /// <summary>
            /// The amphipods in each position of the hallway.
            /// </summary>
            public readonly Dictionary<int, Type> Hallway;

            /// <summary>
            /// The rooms.
            /// </summary>
            public readonly List<Room> Rooms;

            /// <summary>
            /// The amount of energy used already in reaching this state.
            /// </summary>
            public int EnergyUsed;

            /// <summary>
            /// Creates a new <see cref="StepState"/>.
            /// </summary>
            /// <param name="hallway">The hallway.</param>
            /// <param name="rooms">The rooms.</param>
            /// <param name="energyUsed">The energy used so far.</param>
            public StepState(Dictionary<int, Type> hallway, List<Room> rooms, int energyUsed)
            {
                Hallway = hallway;
                Rooms = rooms;
                EnergyUsed = energyUsed;
            }

            /// <summary>
            /// Performs a deep copy of the state.
            /// </summary>
            /// <returns>The copied state.</returns>
            public StepState Copy()
            {
                Dictionary<int, Type> newHallway = new(Hallway);
                List<Room> newRooms = new(Rooms.Select(room => room.Copy()));

                return new StepState(newHallway, newRooms, EnergyUsed);
            }
        }

        /// <summary>
        /// A room in the amphipod burrow.
        /// </summary>
        private class Room
        {
            /// <summary>
            /// The position this room is in, relative to the hallway.
            /// </summary>
            public readonly int HallwayPosition;

            /// <summary>
            /// The type of amphipod that this room is intended for.
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// The amphipods in this room.
            /// </summary>
            public readonly Stack<Type> Amphipods;

            /// <summary>
            /// Creates a new <see cref="Room"/>.
            /// </summary>
            /// <param name="hallwayPosition">The position of the room relative to the hallway.</param>
            /// <param name="type">The type of amphipod this room is intended for.</param>
            /// <param name="amphipods">The ampipods in the room.</param>
            public Room(int hallwayPosition, Type type, Stack<Type> amphipods)
            {
                HallwayPosition = hallwayPosition;
                Type = type;
                Amphipods = amphipods;
            }

            /// <summary>
            /// Determines whether this room is complete, ie. if it is filled with the correct amphipods.
            /// </summary>
            /// <returns>True if the room is filled with the correct amphipods, false otherwise.</returns>
            public bool IsComplete()
            {
                return Amphipods.Count == 4 && Amphipods.All(a => a == Type);
            }

            /// <summary>
            /// Performs a deep copy of the room.
            /// </summary>
            /// <returns>The copied room.</returns>
            public Room Copy()
            {
                Stack<Type> newAmphipods = new(Amphipods.Reverse());
                return new Room(HallwayPosition, Type, newAmphipods);
            }
        }

        /// <summary>
        /// A type of amphipod.
        /// </summary>
        private enum Type
        {
            Amber, Bronze, Copper, Desert
        }
    }
}
