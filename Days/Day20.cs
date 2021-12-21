using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_of_Code.Days
{
    /// <summary>
    /// https://adventofcode.com/2021/day/20
    /// </summary>
    internal class Day20 : AdventDay
    {
        /// <summary>
        /// The image enhancement algorithm.
        /// </summary>
        private readonly bool[] algorithm;

        /// <summary>
        /// The original image (unenhanced).
        /// </summary>
        private readonly Image rawImage;

        public Day20()
        {
            string[] input = GetInputData(Environment.NewLine);

            algorithm = input[0].Select(i => ParsePixel(i)).ToArray();

            bool[,] rawImagePixels = new bool[input.Length - 2, input[2].Length];

            for (int row = 0; row < rawImagePixels.GetLength(0); row++)
            {
                for (int column = 0; column < rawImagePixels.GetLength(1); column++)
                {
                    rawImagePixels[row, column] = ParsePixel(input[row + 2][column]);
                }
            }

            rawImage = new Image(rawImagePixels, false);
        }

        /// <summary>
        /// Parses a pixel character (either '#' or '.').
        /// </summary>
        /// <param name="pixelChar">The pixel character.</param>
        /// <returns>True if the input was '#', false if it was '.'.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the input character was neither '#' or '.'.</exception>
        private static bool ParsePixel(char pixelChar)
        {
            return pixelChar switch
            {
                '.' => false,
                '#' => true,
                _ => throw new InvalidOperationException($"Cannot parse the pixel {pixelChar}")
            };
        }

        internal override void SolvePuzzle1()
        {
            Image enhancedImage = Enhance(2);
            Console.WriteLine(enhancedImage.LitPixelCount());
        }

        internal override void SolvePuzzle2()
        {
            Image enhancedImage = Enhance(50);
            Console.WriteLine(enhancedImage.LitPixelCount());
        }

        /// <summary>
        /// Enhances the original image.
        /// </summary>
        /// <param name="steps">The number of times to enhance.</param>
        /// <returns>The enhanced image.</returns>
        private Image Enhance(int steps)
        {
            Image enhancedImage = rawImage;

            for (int i = 0; i < steps; i++)
            {
                enhancedImage = Enhance(enhancedImage);
            }

            return enhancedImage;
        }

        /// <summary>
        /// Enhances an image.
        /// </summary>
        /// <param name="image">The image to enhance.</param>
        /// <returns>The enhanced image.</returns>
        private Image Enhance(Image image)
        {
            bool[,] paddedPixels = Pad(image.Pixels, 2, image.EdgeLit);
            bool[,] enhancedPixels = new bool[paddedPixels.GetLength(0), paddedPixels.GetLength(1)];

            // Modify image in windows of three, pivoting around the centre pixel
            for (int row = 0; row < paddedPixels.GetLength(0); row++)
            {
                for (int column = 0; column < paddedPixels.GetLength(1); column++)
                {
                    // This is the infinite edge
                    if (row == 0 | column == 0 | row == paddedPixels.GetLength(0) - 1 | column == paddedPixels.GetLength(1) - 1)
                    {
                        enhancedPixels[row, column] = image.EdgeLit ? algorithm.Last() : algorithm.First();
                    }
                    else
                    {
                        int window = ToInt(
                            paddedPixels[row - 1, column - 1],
                            paddedPixels[row - 1, column],
                            paddedPixels[row - 1, column + 1],
                            paddedPixels[row, column - 1],
                            paddedPixels[row, column],
                            paddedPixels[row, column + 1],
                            paddedPixels[row + 1, column - 1],
                            paddedPixels[row + 1, column],
                            paddedPixels[row + 1, column + 1]);

                        bool enchancedPixel = algorithm[window];

                        enhancedPixels[row, column] = enchancedPixel;
                    }
                }
            }

            return new Image(enhancedPixels, image.EdgeLit ? algorithm.Last() : algorithm.First());
        }

        /// <summary>
        /// Converts a boolean array (representing bits) to an integer.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <returns>The corresponding integer.</returns>
        private static int ToInt(params bool[] bits)
        {
            int i = 0;

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    i++;
                }

                i <<= 1;
            }

            i >>= 1;

            return i;
        }

        /// <summary>
        /// Pads a 2D array.
        /// </summary>
        /// <param name="array">The array to pad.</param>
        /// <param name="count">The number of extra rows and columns to pad with in each dimension.</param>
        /// <param name="with">The value to pad with.</param>
        /// <returns>The padded array.</returns>
        private static bool[,] Pad(bool[,] array, int count, bool with)
        {
            bool[,] padded = new bool[array.GetLength(0) + count * 2, array.GetLength(1) + count * 2];

            for (int row = 0; row < padded.GetLength(0); row++)
            {
                for (int column = 0; column < padded.GetLength(1); column++)
                {
                    padded[row, column] = with;
                }
            }

            for (int row = 0; row < array.GetLength(0); row++)
            {
                for (int column = 0; column < array.GetLength(1); column++)
                {
                    padded[row + count, column + count] = array[row, column];
                }
            }

            return padded;
        }

        /// <summary>
        /// An infinite image.
        /// </summary>
        private class Image
        {
            /// <summary>
            /// The pixels in the image.
            /// </summary>
            public readonly bool[,] Pixels;

            /// <summary>
            /// Whether the edges of the image are lit.
            /// </summary>
            public bool EdgeLit;

            /// <summary>
            /// Creates a new <see cref="Image"/>.
            /// </summary>
            /// <param name="pixels">The pixels.</param>
            /// <param name="edgeLit">Whether the edge of the image is lit.</param>
            public Image(bool[,] pixels, bool edgeLit)
            {
                Pixels = pixels;
                EdgeLit = edgeLit;
            }

            /// <summary>
            /// Counts the number of lit pixels in the image.
            /// If the edge of the image is lit, then this will return <see cref="int.MaxValue"/>.
            /// </summary>
            /// <returns>The number of lit pixels.</returns>
            public int LitPixelCount()
            {
                if (EdgeLit)
                {
                    return int.MaxValue;
                }

                int litPixelCount = 0;

                for (int row = 0; row < Pixels.GetLength(0); row++)
                {
                    for (int column = 0; column < Pixels.GetLength(1); column++)
                    {
                        if (Pixels[row, column])
                        {
                            litPixelCount++;
                        }
                    }
                }

                return litPixelCount;
            }
        }
    }
}
