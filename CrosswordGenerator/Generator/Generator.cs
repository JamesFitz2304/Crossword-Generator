using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            foreach (var word in words)
            {
                word.Placed = false;
            }
            var firstWord = words.OrderByDescending(w => w.WordLength).First();
            var blocks = new LetterBlock[1, firstWord.WordLength];
            var placedWords = new List<PlacedWord>();
            PlaceFirstWord(ref blocks, firstWord, placedWords);
            PlaceRestOfWords(ref blocks, words, placedWords);
            var unplacedWords = words.Where(w => !w.Placed).Select(w => w.WordString).ToList();
            return new Generation(blocks, words, placedWords, unplacedWords);
        }

        private static void PlaceFirstWord(ref LetterBlock[,] blocks, Word word, IList<PlacedWord> placedWords)
        {
            var placement = new Placement(word, true);
            for (var i = 1; i <= word.WordLength; i++)
            {
                var coordinates = new BlockCoordinates(i, 1);
                var newBlock = new LetterBlock(word.CharArray[i - 1])
                {
                    Coordinates = coordinates
                };

                placement.LetterBlocks[i - 1] = newBlock;
            }

            PlaceWordOnBoard(ref blocks, placement, placedWords);
        }

        private void PlaceRestOfWords(ref LetterBlock[,] blocks, IList<Word> words, List<PlacedWord> placedWords)
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

                var orderedPlacements = placements
                    .OrderByDescending(p => p.ExistingLettersUsed)
                    .ThenBy(p => p.Expansion.TotalX + p.Expansion.TotalY)
                    .ThenByDescending(p => p.Word.WordLength);

                var bestPlacements = orderedPlacements.Take(3).ToList();
                var randomPlacement = bestPlacements[new Random().Next(0, bestPlacements.Count-1)];
                PlaceWordOnBoard(ref blocks, randomPlacement, placedWords);
                unplacedWords.Remove(randomPlacement.Word);

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
            //add each new possible placement to list
            return placements;
        }

        private bool WordCanBePlacedHorizontally(LetterBlock[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word, true);
            var currentBlock = new BlockCoordinates(blockCoordinates.Coordinates.X, blockCoordinates.Coordinates.Y);

            if (!OutOfBounds(blocks, currentBlock.ArrayCoordinates.X - 1, currentBlock.ArrayCoordinates.Y) && blocks[currentBlock.ArrayCoordinates.Y, currentBlock.ArrayCoordinates.X - 1] != null ||
                !OutOfBounds(blocks, currentBlock.ArrayCoordinates.X + 1, currentBlock.ArrayCoordinates.Y) && blocks[currentBlock.ArrayCoordinates.Y, currentBlock.ArrayCoordinates.X + 1] != null)
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
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1],new BlockCoordinates(currentBlock.Coordinates.X, currentBlock.Coordinates.Y));
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

        private bool WordCanBePlacedVertically(LetterBlock[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word, false);
            var currentBlock = new BlockCoordinates(blockCoordinates.Coordinates.X, blockCoordinates.Coordinates.Y);

            if (!OutOfBounds(blocks, currentBlock.ArrayCoordinates.X, currentBlock.ArrayCoordinates.Y + 1) && blocks[currentBlock.ArrayCoordinates.Y + 1, currentBlock.ArrayCoordinates.X] != null ||
                !OutOfBounds(blocks, currentBlock.ArrayCoordinates.X, currentBlock.ArrayCoordinates.Y - 1) && blocks[currentBlock.ArrayCoordinates.Y - 1, currentBlock.ArrayCoordinates.X] != null)
            {
                return false;
            }

            placement.LetterBlocks[letterIndex - 1] = new LetterBlock(word.CharArray[letterIndex-1], blockCoordinates);


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

        private bool LetterCanBePlaced(LetterBlock[,] blocks, BlockCoordinates blockCoordinates, char[] letters, int letterIndex, int nextLetterStep, string direction, BlockCoordinates nextBlockCoordinates, Placement placement)
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
                if (!OutOfBounds(blocks, blockCoordinates.ArrayCoordinates.X, blockCoordinates.ArrayCoordinates.Y + 1) && blocks[blockCoordinates.ArrayCoordinates.Y + 1, blockCoordinates.ArrayCoordinates.X] != null ||
                    !OutOfBounds(blocks, blockCoordinates.ArrayCoordinates.X, blockCoordinates.ArrayCoordinates.Y - 1) && blocks[blockCoordinates.ArrayCoordinates.Y - 1, blockCoordinates.ArrayCoordinates.X] != null)
                {
                    return false;
                }
            }
            else
            {
                if (!OutOfBounds(blocks, blockCoordinates.ArrayCoordinates.X + 1, blockCoordinates.ArrayCoordinates.Y) && blocks[blockCoordinates.ArrayCoordinates.Y, blockCoordinates.ArrayCoordinates.X + 1] != null ||
                    !OutOfBounds(blocks, blockCoordinates.ArrayCoordinates.X - 1, blockCoordinates.ArrayCoordinates.Y) && blocks[blockCoordinates.ArrayCoordinates.Y, blockCoordinates.ArrayCoordinates.X - 1] != null)
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

        private static void PlaceWordOnBoard(ref LetterBlock[,] blocks, Placement placement,
            IList<PlacedWord> placedWords)
        {
            if (placement.Expansion.TotalX != 0 || placement.Expansion.TotalY != 0)
                ExpandCrosswordSpace(ref blocks, placement, placedWords);

            //SetLetterToPlacementCoordinates(placement);
            foreach (var block in placement.LetterBlocks)
            {
                blocks[block.Coordinates.ArrayCoordinates.Y, block.Coordinates.ArrayCoordinates.X] = block;
            }

            placement.Word.Placed = true;
            placedWords.Add(new PlacedWord
            (
                placement.Word.Id,
                placement.Word.WordString,
                placement.Across,
                placement.FirstLetterCoordinates
                )
            );
        }

        private static void ExpandCrosswordSpace(ref LetterBlock[,] blocks, Placement placement, IList<PlacedWord> placedWords)
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

            if (placement.Expansion.Left > 0 || placement.Expansion.Up > 0)
            {
                foreach (var placedWord in placedWords)
                {
                    placedWord.ShiftFirstLetterCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                }
            }

            blocks = newBlocks;
        }


        //private static void SetLetterToPlacementCoordinates(Placement placement)
        //{
        //    for (var i = 0; i < placement.Word.WordLength; i++)
        //    {
        //        placement.Word.CharArray[i].Coordinates = placement.Coordinates[i];
        //    }
        //}

        private static bool OutOfBounds(LetterBlock[,] blocks, int x, int y)
        {
            return !(x < blocks.GetLength(1) && x >= 0 && y < blocks.GetLength(0) && y >= 0);
        }
    }
}
