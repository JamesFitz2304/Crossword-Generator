using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator.Utilities;
// ReSharper disable InvertIf

namespace CrosswordGenerator.Generator
{
    public class Generator : IGenerator
    {
        public Generation Generate(IList<Word> words)
        {
            var firstWord = words.OrderByDescending(w => w.WordLength).First();
            var blocks = new LetterBlock[1, firstWord.WordLength];
            PlaceFirstWord(ref blocks, firstWord);
            PlaceRestOfWords(ref blocks, words);
            return new Generation(blocks, words);
        }

        private static void PlaceFirstWord(ref LetterBlock[,] blocks, Word word)
        {
            var placement = new Placement(word);
            for (var i = 1; i <= word.WordLength; i++)
            {
                var coordinates = new BlockCoordinates(i, 1);
                var newBlock = new LetterBlock(word.WordAsCharArray[i - 1])
                {
                    Coordinates = coordinates
                };

                placement.LetterBlocks[i - 1] = newBlock;
            }

            PlaceWordOnBoard(ref blocks, placement);
        }

        private void PlaceRestOfWords(ref LetterBlock[,] blocks, IList<Word> words)
        {
            while (true)
            {
                var placements = new List<Placement>();
                var unplacedWords = words.Where(word => !word.Placed).ToList();

                foreach (var word in unplacedWords)
                {
                    placements.AddRange(FindPossibleWordPlacements(blocks, word));
                }

                if (!placements.Any())
                {
                    // Return if no more _words can be placed
                    return;
                }

                placements.Sort(new PlacementComparer()); //sort placements by least new letters added to board
                var bestPlacement = placements.First();
                //ToDo: Maybe choose a random placement, instead of the "best" one. Otherwise the same solutions will keep being generated.
                PlaceWordOnBoard(ref blocks, bestPlacement);
                unplacedWords.Remove(bestPlacement.Word);

                if (!unplacedWords.Any())
                {
                    return;
                }
            }
        }

        private IEnumerable<Placement> FindPossibleWordPlacements(LetterBlock[,] blocks, Word word)
        {
            var placements = new List<Placement>();

            for (var i = 0; i < word.WordLength; i++)
            {
                for (var y = 0; y < blocks.GetLength(0); y++)
                {
                    for (var x = 0; x < blocks.GetLength(1); x++)
                    {
                        var block = blocks[y, x];
                        if (block != null && block.Character == word.WordAsCharArray[i]) // check if block == letter
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
            //add each new possible placement to list
            return placements;
        }

        private bool WordCanBePlacedHorizontally(LetterBlock[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word);
            var currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(blocks, currentBlock.ArrayX - 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayY, currentBlock.ArrayX - 1] != null ||
                !OutOfBounds(blocks, currentBlock.ArrayX + 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayY, currentBlock.ArrayX + 1] != null)
            {
                return false;
            }

            placement.LetterBlocks[letterIndex - 1] =
                new LetterBlock(word.WordAsCharArray[letterIndex - 1], blockCoordinates);


            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.X--;
                if (currentBlock.X < 1)
                {
                    placement.Expansion.Left++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.WordAsCharArray, i, -1, "L", new BlockCoordinates(currentBlock.X - 1, currentBlock.Y), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.WordAsCharArray[i - 1],new BlockCoordinates(currentBlock.X, currentBlock.Y));
            }

            currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
            {
                currentBlock.X++;
                if (currentBlock.X > blocks.GetLength(1))
                {
                    placement.Expansion.Right++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.WordAsCharArray, i, 1, "R", new BlockCoordinates(currentBlock.X + 1, currentBlock.Y), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.WordAsCharArray[i - 1], new BlockCoordinates(currentBlock.X, currentBlock.Y));
            }

            if (placement.Expansion.Left > 0)
            {
                foreach (var letterBlock in placement.LetterBlocks)
                {
                    letterBlock.Coordinates.X += placement.Expansion.Left;
                }
            }

            return true;
        }

        private bool WordCanBePlacedVertically(LetterBlock[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word);
            var currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(blocks, currentBlock.ArrayX, currentBlock.ArrayY + 1) && blocks[currentBlock.ArrayY + 1, currentBlock.ArrayX] != null ||
                !OutOfBounds(blocks, currentBlock.ArrayX, currentBlock.ArrayY - 1) && blocks[currentBlock.ArrayY - 1, currentBlock.ArrayX] != null)
            {
                return false;
            }

            placement.LetterBlocks[letterIndex - 1] = new LetterBlock(word.WordAsCharArray[letterIndex-1], blockCoordinates);


            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.Y--;
                if (currentBlock.Y < 1)
                {
                    placement.Expansion.Up++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.WordAsCharArray, i, -1, "U", new BlockCoordinates(currentBlock.X, currentBlock.Y - 1), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.WordAsCharArray[i - 1], new BlockCoordinates(currentBlock.X, currentBlock.Y));
            }

            currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
            {
                currentBlock.Y++;
                if (currentBlock.Y > blocks.GetLength(0))
                {
                    placement.Expansion.Down++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocks, currentBlock, word.WordAsCharArray, i, 1, "D", new BlockCoordinates(currentBlock.X, currentBlock.Y + 1), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.WordAsCharArray[i - 1], new BlockCoordinates(currentBlock.X, currentBlock.Y));
            }

            if (placement.Expansion.Up > 0)
            {
                foreach (var letterBlock in placement.LetterBlocks)
                {
                    letterBlock.Coordinates.Y += placement.Expansion.Up;
                }
            }

            return true;
        }

        private bool LetterCanBePlaced(LetterBlock[,] blocks, BlockCoordinates blockCoordinates, char[] letters, int letterIndex, int nextLetterStep, string direction, BlockCoordinates nextBlockCoordinates, Placement placement)
        {
            var letter = letters[letterIndex - 1];
            var nextBlock = OutOfBounds(blocks, nextBlockCoordinates.ArrayX, nextBlockCoordinates.ArrayY)
                ? null
                : blocks[nextBlockCoordinates.ArrayY, nextBlockCoordinates.ArrayX];


            if (blocks[blockCoordinates.ArrayY, blockCoordinates.ArrayX] != null)
            {
                if (blocks[blockCoordinates.ArrayY, blockCoordinates.ArrayX].Character != letter)
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
                if (!OutOfBounds(blocks, blockCoordinates.ArrayX, blockCoordinates.ArrayY + 1) && blocks[blockCoordinates.ArrayY + 1, blockCoordinates.ArrayX] != null ||
                    !OutOfBounds(blocks, blockCoordinates.ArrayX, blockCoordinates.ArrayY - 1) && blocks[blockCoordinates.ArrayY - 1, blockCoordinates.ArrayX] != null)
                {
                    return false;
                }
            }
            else
            {
                if (!OutOfBounds(blocks, blockCoordinates.ArrayX + 1, blockCoordinates.ArrayY) && blocks[blockCoordinates.ArrayY, blockCoordinates.ArrayX + 1] != null ||
                    !OutOfBounds(blocks, blockCoordinates.ArrayX - 1, blockCoordinates.ArrayY) && blocks[blockCoordinates.ArrayY, blockCoordinates.ArrayX - 1] != null)
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

        private static void PlaceWordOnBoard(ref LetterBlock[,] blocks, Placement placement)
        {
            if (placement.Expansion.TotalX != 0 || placement.Expansion.TotalY != 0)
                ExpandCrosswordSpace(ref blocks, placement);

            //SetLetterToPlacementCoordinates(placement);
            foreach (var block in placement.LetterBlocks)
            {
                blocks[block.Coordinates.ArrayY, block.Coordinates.ArrayX] = block;
            }
            placement.Word.Placed = true;
        }

        private static void ExpandCrosswordSpace(ref LetterBlock[,] blocks, Placement placement)
        {
            var newBlocks = new LetterBlock[blocks.GetLength(0) + Math.Abs(placement.Expansion.TotalY), blocks.GetLength(1) + Math.Abs(placement.Expansion.TotalX)];

            for (var y = 0; y < blocks.GetLength(0); y++)
            {
                for (var x = 0; x < blocks.GetLength(1); x++)
                {
                    blocks[y, x]?.Coordinates.ShiftCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                    newBlocks[y + placement.Expansion.Up, x + placement.Expansion.Left] = blocks[y, x];
                }
            }
            blocks = newBlocks;
        }


        //private static void SetLetterToPlacementCoordinates(Placement placement)
        //{
        //    for (var i = 0; i < placement.Word.WordLength; i++)
        //    {
        //        placement.Word.WordAsCharArray[i].Coordinates = placement.Coordinates[i];
        //    }
        //}

        private static bool OutOfBounds(LetterBlock[,] blocks, int x, int y)
        {
            return !(x < blocks.GetLength(1) && x >= 0 && y < blocks.GetLength(0) && y >= 0);
        }
    }
}
