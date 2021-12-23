using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/16
    /// </summary>
    internal class Day16 : AdventDay
    {
        /// <summary>
        /// The binary tranmission.
        /// </summary>
        private readonly string binaryTransmission;

        public Day16()
        {
            string input = GetInputData();

            Dictionary<char, string> hexBinaryLookup = new()
            {
                { '0', "0000" },
                { '1', "0001" },
                { '2', "0010" },
                { '3', "0011" },
                { '4', "0100" },
                { '5', "0101" },
                { '6', "0110" },
                { '7', "0111" },
                { '8', "1000" },
                { '9', "1001" },
                { 'A', "1010" },
                { 'B', "1011" },
                { 'C', "1100" },
                { 'D', "1101" },
                { 'E', "1110" },
                { 'F', "1111" }
            };

            StringBuilder transmissionBuilder = new();

            foreach (char c in input)
            {
                transmissionBuilder.Append(hexBinaryLookup[c]);
            }

            binaryTransmission = transmissionBuilder.ToString();
        }

        internal override object? SolvePuzzle1()
        {
            ParsingReport report = Parse(binaryTransmission);
            return VerisonNumberSum(report.ParsedPacket);
        }

        internal override object? SolvePuzzle2()
        {
            ParsingReport report = Parse(binaryTransmission);
            return CalculatePacketValue(report.ParsedPacket);
        }

        /// <summary>
        /// Computes the sum of all version numbers of a packet and its subpackets.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The sum of version numbers.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the packet type is invalid.</exception>
        private static int VerisonNumberSum(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.Literal:
                    return ((LiteralPacket)packet).Version;
                case PacketType.Operator:
                    OperatorPacket operatorPacket = (OperatorPacket)packet;
                    return operatorPacket.Version + operatorPacket.SubPackets.Sum(subPacket => VerisonNumberSum(subPacket));
                default:
                    throw new InvalidOperationException($"The packet type {packet.Type} is invalid.");
            }
        }

        /// <summary>
        /// Computes a packet's value.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The packet value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the packet type is invalid.</exception>
        private static long CalculatePacketValue(Packet packet)
        {
            return packet.Type switch
            {
                PacketType.Literal => ((LiteralPacket)packet).Value,
                PacketType.Operator => CalculatePacketValue((OperatorPacket)packet),
                _ => throw new InvalidOperationException($"The packet type {packet.Type} is invalid."),
            };
        }

        /// <summary>
        /// Computes an operator packet's value.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The packet value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the packet type ID is invalid.</exception>
        private static long CalculatePacketValue(OperatorPacket packet)
        {
            return packet.TypeID switch
            {
                0 => packet.SubPackets.Select(s => CalculatePacketValue(s)).Sum(),
                1 => packet.SubPackets.Select(s => CalculatePacketValue(s)).Aggregate(1L, (a, b) => a * b),
                2 => packet.SubPackets.Select(s => CalculatePacketValue(s)).Min(),
                3 => packet.SubPackets.Select(s => CalculatePacketValue(s)).Max(),
                5 => CalculatePacketValue(packet.SubPackets[0]) > CalculatePacketValue(packet.SubPackets[1]) ? 1 : 0,
                6 => CalculatePacketValue(packet.SubPackets[0]) < CalculatePacketValue(packet.SubPackets[1]) ? 1 : 0,
                7 => CalculatePacketValue(packet.SubPackets[0]) == CalculatePacketValue(packet.SubPackets[1]) ? 1 : 0,
                _ => throw new InvalidOperationException($"The packet type {packet.Type} is invalid."),
            };
        }

        /// <summary>
        /// Parses packet data into a packet.
        /// </summary>
        /// <param name="packetData">The packet data, encoded as binary.</param>
        /// <returns>A parsing report containing the parsed packet.</returns>
        private static ParsingReport Parse(string packetData)
        {
            return ExtractTypeID(packetData) switch
            {
                4 => ParseLiteral(packetData),
                _ => ParseOperator(packetData)
            };
        }

        /// <summary>
        /// Parses packet data that represents a literal packet.
        /// </summary>
        /// <param name="packetData">The packet data, encoded as binary.</param>
        /// <returns>A parsing report containing the parsed packet.</returns>
        private static ParsingReport ParseLiteral(string packetData)
        {
            int version = ExtractVersion(packetData);
            string literalValue = "";
            int bitsParsed = 6;

            for (int i = 6; i < packetData.Length; i += 5)
            {
                char prefix = packetData[i];
                literalValue += packetData[(i + 1)..(i + 5)];

                bitsParsed += 5;

                if (prefix == '0')
                {
                    break;
                }
            }

            return new ParsingReport(
                new LiteralPacket(version, Convert.ToInt64(literalValue, 2)),
                bitsParsed);
        }

        /// <summary>
        /// Parses packet data that represents an operator packet.
        /// </summary>
        /// <param name="packetData">The packet data, encoded as binary.</param>
        /// <returns>A parsing report containing the parsed packet.</returns>
        private static ParsingReport ParseOperator(string packetData)
        {
            int version = ExtractVersion(packetData);
            int typeID = ExtractTypeID(packetData);
            char lengthType = packetData[6];
            List<Packet> subPackets = new();

            int bitsParsed = 7;

            if (lengthType == '0')
            {
                int totalSubpacketLength = Convert.ToInt32(packetData[7..22], 2);
                bitsParsed += 15;

                int subPacketBitsParsed = 0;

                while (subPacketBitsParsed < totalSubpacketLength)
                {
                    ParsingReport subpacketReport = Parse(packetData[(22 + subPacketBitsParsed)..]);

                    subPackets.Add(subpacketReport.ParsedPacket);
                    subPacketBitsParsed += subpacketReport.BitsParsed;
                }

                bitsParsed += subPacketBitsParsed;
            }
            else
            {
                int numberOfSubpackets = Convert.ToInt32(packetData[7..18], 2);
                bitsParsed += 11;

                int subPacketBitsParsed = 0;

                for (int i = 0; i< numberOfSubpackets; i++)
                {
                    ParsingReport subpacketReport = Parse(packetData[(18 + subPacketBitsParsed)..]);

                    subPackets.Add(subpacketReport.ParsedPacket);
                    subPacketBitsParsed += subpacketReport.BitsParsed;
                }

                bitsParsed += subPacketBitsParsed;
            }

            return new ParsingReport(
                new OperatorPacket(version, typeID, subPackets),
                bitsParsed);
        }

        /// <summary>
        /// Extracts the version number from a packet.
        /// </summary>
        /// <param name="packetData">The packet data, encoded as binary.</param>
        /// <returns>The version number.</returns>
        private static int ExtractVersion(string packetData)
        {
            return Convert.ToInt32(packetData[..3], 2);
        }

        /// <summary>
        /// Extracts the type ID from a packet.
        /// </summary>
        /// <param name="packetData">The packet data, encoded as binary.</param>
        /// <returns>The type ID.</returns>
        private static int ExtractTypeID(string packetData)
        {
            return Convert.ToInt32(packetData[3..6], 2);
        }

        /// <summary>
        /// A packet parsing report.
        /// </summary>
        private class ParsingReport
        {
            /// <summary>
            /// The parsed packet.
            /// </summary>
            public readonly Packet ParsedPacket;

            /// <summary>
            /// The number of bits parsed.
            /// </summary>
            public readonly int BitsParsed;

            /// <summary>
            /// Creates a new <see cref="ParsingReport"/>.
            /// </summary>
            /// <param name="parsedPacket">The parsed packet.</param>
            /// <param name="bitsParsed">The number of bits parsed.</param>
            public ParsingReport(Packet parsedPacket, int bitsParsed)
            {
                ParsedPacket = parsedPacket;
                BitsParsed = bitsParsed;
            }
        }

        /// <summary>
        /// A packet.
        /// </summary>
        private abstract class Packet
        {
            /// <summary>
            /// The packet version.
            /// </summary>
            public readonly int Version;

            /// <summary>
            /// The packet type ID.
            /// </summary>
            public readonly int TypeID;

            /// <summary>
            /// Gets the packet type.
            /// </summary>
            public PacketType Type => TypeID switch
            {
                4 => PacketType.Literal,
                _ => PacketType.Operator
            };

            /// <summary>
            /// Creates a new <see cref="Packet"/>.
            /// </summary>
            /// <param name="version">The packet version.</param>
            /// <param name="typeID">The packet type ID.</param>
            public Packet(int version, int typeID)
            {
                Version = version;
                TypeID = typeID;
            }
        }

        /// <summary>
        /// A literal packet.
        /// </summary>
        private class LiteralPacket : Packet
        {
            /// <summary>
            /// The packet's literal value.
            /// </summary>
            public readonly long Value;

            /// <summary>
            /// Creates a new <see cref="LiteralPacket"/>.
            /// </summary>
            /// <param name="version">The packet version.</param>
            /// <param name="value">The literal value.</param>
            public LiteralPacket(int version, long value) : base(version, 4)
            {
                Value = value;
            }
        }

        /// <summary>
        /// An operator packet.
        /// </summary>
        private class OperatorPacket : Packet
        {
            /// <summary>
            /// The operator's subpackets.
            /// </summary>
            public readonly List<Packet> SubPackets;

            /// <summary>
            /// Creates a new <see cref="OperatorPacket"/>.
            /// </summary>
            /// <param name="version">The packet version.</param>
            /// <param name="typeID">The packet's type ID.</param>
            /// <param name="subPackets">The operator's subpackets.</param>
            public OperatorPacket(int version, int typeID, List<Packet> subPackets) : base(version, typeID)
            {
                SubPackets = subPackets;
            }
        }

        /// <summary>
        /// A packet type.
        /// </summary>
        private enum PacketType
        {
            Literal, Operator
        }
    }
}
