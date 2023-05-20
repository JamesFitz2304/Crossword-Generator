using System.Collections.Generic;
using System.Drawing;
using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.Generator
{
    public class PlacementFinder : IPlacementFinder
    {
        public IEnumerable<Placement> FindWordPlacements(LetterBlock[,] blocks, Word word)
        {
            var placements = new List<Placement>();

            for (var i = 0; i < word.WordLength; i++)
            {
                for (var y = 0; y < blocks.GetLength(0); y++)
                {
                    for (var x = 0; x < blocks.GetLength(1); x++)
                    {
                        var block = blocks[y, x];
                        if (block != null && block.Character == word.CharArray[i]) // check if block == letter
                        {
                            if (WordCanBePlacedVertically(blocks, word, new BlockCoordinates(x + 1, y + 1), i + 1, out var placement))
                            {
                                if (placement != null) placements.Add(placement);
                            }
                            else if (WordCanBePlacedHorizontally(blocks, word, new BlockCoordinates(x + 1, y + 1), i + 1, out placement))
                            {
                                if (placement != null) placements.Add(placement);
                            }
                        }
                    }
                }
            }
            return placements;
        }

        private static bool WordCanBePlacedHorizontally(LetterBlock[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word, true);
            var currentBlock = new BlockCoordinates(blockCoordinates.Coordinates.X, blockCoordinates.Coordinates.Y);

            var left = GetPointWithOffset(currentBlock.ArrayCoordinates, -1, 0);
            var right = GetPointWithOffset(currentBlock.ArrayCoordinates, 1, 0);
            if (!OutOfBounds(blocks, left.X, left.Y) && blocks[left.Y, left.X] != null ||
                !OutOfBounds(blocks, right.X, right.Y) && blocks[right.Y, right.X] != null)
            {
                return false;
            }

            placement.LetterBlocks[letterIndex - 1] =
                new LetterBlock(word.CharArray[letterIndex - 1], blockCoordinates);


            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.Coordinates.X--;
                if (currentBlock.Coordinates.X < 1)
                {
                    placement.Expansion.Left++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.CharArray, i, -1, "L", new BlockCoordinates(currentBlock.Coordinates.X - 1, currentBlock.Coordinates.Y), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new BlockCoordinates(currentBlock.Coordinates.X, currentBlock.Coordinates.Y));
            }

            currentBlock = new BlockCoordinates(blockCoordinates.Coordinates.X, blockCoordinates.Coordinates.Y);

            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
            {
                currentBlock.Coordinates.X++;
                if (currentBlock.Coordinates.X > blocks.GetLength(1))
                {
                    placement.Expansion.Right++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.CharArray, i, 1, "R", new BlockCoordinates(currentBlock.Coordinates.X + 1, currentBlock.Coordinates.Y), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new BlockCoordinates(currentBlock.Coordinates.X, currentBlock.Coordinates.Y));
            }

            if (placement.Expansion.Left > 0)
            {
                foreach (var letterBlock in placement.LetterBlocks)
                {
                    letterBlock.Coordinates.Coordinates.X += placement.Expansion.Left;
                }
            }

            return true;
        }

        private static bool WordCanBePlacedVertically(LetterBlock[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word, false);
            var currentBlock = new BlockCoordinates(blockCoordinates.Coordinates.X, blockCoordinates.Coordinates.Y);

            var below = GetPointWithOffset(currentBlock.ArrayCoordinates, 0, 1);
            var above = GetPointWithOffset(currentBlock.ArrayCoordinates, 0, -1);
            if (!OutOfBounds(blocks,below.X, below.Y) && blocks[below.Y, below.X] != null ||
                !OutOfBounds(blocks, above.X, above.Y) && blocks[above.Y, above.X] != null)
            {
                return false;
            }

            placement.LetterBlocks[letterIndex - 1] = new LetterBlock(word.CharArray[letterIndex - 1], blockCoordinates);


            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.Coordinates.Y--;
                if (currentBlock.Coordinates.Y < 1)
                {
                    placement.Expansion.Up++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.CharArray, i, -1, "U", new BlockCoordinates(currentBlock.Coordinates.X, currentBlock.Coordinates.Y - 1), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new BlockCoordinates(currentBlock.Coordinates.X, currentBlock.Coordinates.Y));
            }

            currentBlock = new BlockCoordinates(blockCoordinates.Coordinates.X, blockCoordinates.Coordinates.Y);
            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
            {
                currentBlock.Coordinates.Y++;
                if (currentBlock.Coordinates.Y > blocks.GetLength(0))
                {
                    placement.Expansion.Down++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.CharArray, i, 1, "D", new BlockCoordinates(currentBlock.Coordinates.X, currentBlock.Coordinates.Y + 1), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new BlockCoordinates(currentBlock.Coordinates.X, currentBlock.Coordinates.Y));
            }

            if (placement.Expansion.Up > 0)
            {
                foreach (var letterBlock in placement.LetterBlocks)
                {
                    letterBlock.Coordinates.Coordinates.Y += placement.Expansion.Up;
                }
            }

            return true;
        }

        private static bool LetterCanBePlaced(LetterBlock[,] blocks, BlockCoordinates blockCoordinates, char[] letters, int letterIndex, int nextLetterStep, string direction, BlockCoordinates nextBlockCoordinates, Placement placement)
        {
            var letter = letters[letterIndex - 1];
            var nextBlock = OutOfBounds(blocks, nextBlockCoordinates.ArrayCoordinates.X, nextBlockCoordinates.ArrayCoordinates.Y)
                ? null
                : blocks[nextBlockCoordinates.ArrayCoordinates.Y, nextBlockCoordinates.ArrayCoordinates.X];


            if (blocks[blockCoordinates.ArrayCoordinates.Y, blockCoordinates.ArrayCoordinates.X] != null)
            {
                if (blocks[blockCoordinates.ArrayCoordinates.Y, blockCoordinates.ArrayCoordinates.X].Character != letter)
                {
                    return false;
                }
                return nextBlock == null;
            }

            var nextLetter = letterIndex + nextLetterStep > letters.Length || letterIndex + nextLetterStep < 1
                ? ' '
                : letters[letterIndex + nextLetterStep - 1];

            //Fail if the above/below or left/right blocks have letters when direction is horizontal/vertical
            if (direction == "L" || direction == "R")
            {
                var below = GetPointWithOffset(blockCoordinates.ArrayCoordinates, 0, 1);
                var above = GetPointWithOffset(blockCoordinates.ArrayCoordinates, 0, -1);
                if (!OutOfBounds(blocks, below.X, below.Y) && blocks[below.Y, below.X] != null ||
                    !OutOfBounds(blocks,above.X, above.Y) && blocks[above.Y, above.X] != null)
                {
                    return false;
                }
            }
            else
            {
                var left = GetPointWithOffset(blockCoordinates.ArrayCoordinates, -1, 0);
                var right = GetPointWithOffset(blockCoordinates.ArrayCoordinates, 1, 0);
                if (!OutOfBounds(blocks, right.X,right.Y) && blocks[right.Y,right.X] != null ||
                    !OutOfBounds(blocks, left.X, left.Y) && blocks[left.Y, left.X] != null)
                {
                    return false;
                }
            }
            placement.NewLetters++;
            // True if next two blocks are empty
            return char.IsWhiteSpace(nextLetter) && nextBlock == null
                   // True if next block is empty or next block has same character as next letter
                   || !char.IsWhiteSpace(nextLetter) && (nextBlock == null || nextBlock.Character == nextLetter);
        }

        private static bool OutOfBounds(LetterBlock[,] blocks, int x, int y)
        {
            return !(x < blocks.GetLength(1) && x >= 0 && y < blocks.GetLength(0) && y >= 0);

        }

        private static Point GetPointWithOffset(Point point, int xOffset, int yOffset)
        {
            var newPoint = point;
            newPoint.Offset(xOffset, yOffset);
            return newPoint;
        }

    }
}