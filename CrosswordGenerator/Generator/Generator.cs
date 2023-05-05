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
            var unplacedWords = words.ToList();
            var firstWord = words.OrderByDescending(w => w.WordLength).First();
            var blocks = new Block[firstWord.WordLength, 1];
            PlaceFirstWord(ref blocks, firstWord);
            unplacedWords.Remove(firstWord);
            PlaceRestOfWords(ref blocks, unplacedWords);
            return new Generation(blocks, unplacedWords);
        }

        private static void PlaceFirstWord(ref Block[,] blocks, Word word)
        {
            var placement = new Placement(word);

            for (var i = 1; i <= word.WordLength; i++)
            {
                var coordinates = new BlockCoordinates(i, 1);
                placement.Coordinates[i - 1] = coordinates;
            }

            PlaceWordOnBoard(ref blocks, placement);
        }

        private static void PlaceRestOfWords(ref Block[,] blocks, List<Word> unplacedWords)
        {
            while (true)
            {
                var placements = new List<Placement>();

                foreach (var word in unplacedWords)
                {
                    placements.AddRange(FindPossibleWordPlacements(ref blocks, word));
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

        private static IEnumerable<Placement> FindPossibleWordPlacements(ref Block[,] blocks, Word word)
        {
            var placements = new List<Placement>();

            for (var i = 0; i < word.Letters.Length; i++)
            {
                for (var y = 0; y < blocks.GetLength(1); y++)
                {
                    for (var x = 0; x < blocks.GetLength(0); x++)
                    {
                        var block = blocks[x, y];
                        if (block != null && block.letter.Character == word.Letters[i].Character) // check if block == letter
                        {
                            if (WordCanBePlacedVertically(ref blocks, word, new BlockCoordinates(x + 1, y + 1), i + 1, out var placement))
                            {
                                if (placement != null) placements.Add(placement);
                            }
                            else if (WordCanBePlacedHorizontally(ref blocks, word, new BlockCoordinates(x + 1, y + 1), i + 1, out placement))
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

        private static bool WordCanBePlacedHorizontally(ref Block[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word);
            var currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(ref blocks, currentBlock.ArrayX - 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayX - 1, currentBlock.ArrayY] != null ||
                !OutOfBounds(ref blocks, currentBlock.ArrayX + 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayX + 1, currentBlock.ArrayY] != null)
            {
                return false;
            }

            placement.Coordinates = new BlockCoordinates[word.Letters.Length];
            placement.Coordinates[letterIndex - 1] = blockCoordinates;


            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.X--;
                if (currentBlock.X < 1)
                {
                    placement.Expansion.Left++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(ref blocks, currentBlock, word.Letters, i, -1, "L", new BlockCoordinates(currentBlock.X - 1, currentBlock.Y), placement))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = new BlockCoordinates(currentBlock.X, currentBlock.Y);
            }

            currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all preceding letters
            {
                currentBlock.X++;
                if (currentBlock.X > blocks.GetLength(0))
                {
                    placement.Expansion.Right++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(ref blocks, currentBlock, word.Letters, i, 1, "R", new BlockCoordinates(currentBlock.X + 1, currentBlock.Y), placement))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = new BlockCoordinates(currentBlock.X, currentBlock.Y);
            }

            if (placement.Expansion.Left > 0)
            {
                foreach (var coordinate in placement.Coordinates)
                {
                    coordinate.X += placement.Expansion.Left;
                }
            }

            return true;
        }

        private static bool WordCanBePlacedVertically(ref Block[,] blocks, Word word, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word);
            var currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(ref blocks, currentBlock.ArrayX, currentBlock.ArrayY + 1) && blocks[currentBlock.ArrayX, currentBlock.ArrayY + 1] != null ||
                !OutOfBounds(ref blocks, currentBlock.ArrayX, currentBlock.ArrayY - 1) && blocks[currentBlock.ArrayX, currentBlock.ArrayY - 1] != null)
            {
                return false;
            }

            placement.Coordinates = new BlockCoordinates[word.Letters.Length];
            placement.Coordinates[letterIndex - 1] = blockCoordinates;


            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.Y--;
                if (currentBlock.Y < 1)
                {
                    placement.Expansion.Up++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(ref blocks, currentBlock, word.Letters, i, -1, "U", new BlockCoordinates(currentBlock.X, currentBlock.Y - 1), placement))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = new BlockCoordinates(currentBlock.X, currentBlock.Y);
            }

            currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
            {
                currentBlock.Y++;
                if (currentBlock.Y > blocks.GetLength(1))
                {
                    placement.Expansion.Down++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(ref blocks, currentBlock, word.Letters, i, 1, "D", new BlockCoordinates(currentBlock.X, currentBlock.Y + 1), placement))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = new BlockCoordinates(currentBlock.X, currentBlock.Y);
            }

            if (placement.Expansion.Up > 0)
            {
                foreach (var coordinate in placement.Coordinates)
                {
                    coordinate.Y += placement.Expansion.Up;
                }
            }

            return true;
        }

        private static bool LetterCanBePlaced(ref Block[,] blocks, BlockCoordinates blockCoordinates, Letter[] letters, int letterIndex, int nextLetterStep, string direction, BlockCoordinates nextBlockCoordinates, Placement placement)
        {
            var letter = letters[letterIndex - 1];
            var nextBlock = OutOfBounds(ref blocks, nextBlockCoordinates.ArrayX, nextBlockCoordinates.ArrayY)
                ? null
                : blocks[nextBlockCoordinates.ArrayX, nextBlockCoordinates.ArrayY];


            if (blocks[blockCoordinates.ArrayX, blockCoordinates.ArrayY] != null)
            {
                if (blocks[blockCoordinates.ArrayX, blockCoordinates.ArrayY].letter.Character != letter.Character)
                {
                    return false;
                }
                return nextBlock == null;
            }

            var nextLetter = letterIndex + nextLetterStep > letters.Length || letterIndex + nextLetterStep < 1 
                ? null 
                : letters[letterIndex + nextLetterStep - 1];

            //Fail if the above/below or left/right blocks have letters when direction is horizontal/vertical
            if (direction == "L" || direction == "R")
            {
                if (!OutOfBounds(ref blocks, blockCoordinates.ArrayX, blockCoordinates.ArrayY + 1) && blocks[blockCoordinates.ArrayX, blockCoordinates.ArrayY + 1] != null ||
                    !OutOfBounds(ref blocks, blockCoordinates.ArrayX, blockCoordinates.ArrayY - 1) && blocks[blockCoordinates.ArrayX, blockCoordinates.ArrayY - 1] != null)
                {
                    return false;
                }
            }
            else
            {
                if (!OutOfBounds(ref blocks, blockCoordinates.ArrayX + 1, blockCoordinates.ArrayY) && blocks[blockCoordinates.ArrayX + 1, blockCoordinates.ArrayY] != null ||
                    !OutOfBounds(ref blocks, blockCoordinates.ArrayX - 1, blockCoordinates.ArrayY) && blocks[blockCoordinates.ArrayX - 1, blockCoordinates.ArrayY] != null)
                {
                    return false;
                }
            }
            placement.NewLetters++;
            return nextLetter == null && nextBlock == null || nextLetter != null && (nextBlock == null || nextBlock.letter.Character == nextLetter.Character);
        }


        private static void PlaceWordOnBoard(ref Block[,] blocks, Placement placement)
        {
            if (placement.Expansion.TotalX != 0 || placement.Expansion.TotalY != 0)
                blocks = ExpandCrosswordSpace(ref blocks, placement);

            SetLetterToPlacementCoordinates(placement);
            foreach (var letter in placement.Word.Letters)
            {
                blocks[letter.Coordinates.ArrayX, letter.Coordinates.ArrayY] = new Block(letter);
            }
            placement.Word.Placed = true;
        }

        private static Block[,] ExpandCrosswordSpace(ref Block[,] blocks, Placement placement)
        {
            var newBlocks = new Block[blocks.GetLength(0) + Math.Abs(placement.Expansion.TotalX), blocks.GetLength(1) + Math.Abs(placement.Expansion.TotalY)];

            for (var y = 0; y < blocks.GetLength(1); y++)
            {
                for (var x = 0; x < blocks.GetLength(0); x++)
                {
                    blocks[x, y]?.letter.Coordinates.ShiftCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                    newBlocks[x + placement.Expansion.Left, y + placement.Expansion.Up] = blocks[x, y];
                }
            }
            return newBlocks;
        }


        private static void SetLetterToPlacementCoordinates(Placement placement)
        {
            for (var i = 0; i < placement.Word.WordLength; i++)
            {
                placement.Word.Letters[i].Coordinates = placement.Coordinates[i];
            }
        }

        private static bool OutOfBounds(ref Block[,] blocks, int x, int y)
        {
            return !(x < blocks.GetLength(0) && x >= 0 && y < blocks.GetLength(1) && y >= 0);
        }
    }
}
