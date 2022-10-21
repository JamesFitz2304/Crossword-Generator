using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CrosswordGenerator.Interfaces;

namespace CrosswordGenerator
{
    public class Generator : IGenerator
    {
        private int wordsPlaced = 0;

        public Generation Generate(IList<Word> words)
        {
            
            var unplacedWords = words.ToList();
            var firstWord = words.OrderByDescending(w => w.WordLength).First();
            Block[,] blocks = new Block[firstWord.WordLength, 1];
            PlaceFirstWord(ref blocks, firstWord);
            unplacedWords.Remove(firstWord);
            PlaceRestOfWords(ref blocks, unplacedWords);
            return new Generation(blocks, unplacedWords);
        }

        private void PlaceFirstWord(ref Block[,] blocks, Word word)
        {
            Placement placement = new Placement(word);

            for (int i = 1; i <= word.WordLength; i++)
            {
                BlockCoordinates coordinates = new BlockCoordinates(i, 1);
                placement.Coordinates[i - 1] = coordinates;
            }

            PlaceWordOnBoard(ref blocks, placement);
            wordsPlaced++;
        }

        private Generation PlaceRestOfWords(ref Block[,] blocks, List<Word> unplacedWords)
        {
            while (true)
            {
                var placements = new List<Placement>();

                //var wordToPlace = unplacedWords.First();

                foreach (var word in unplacedWords)
                {
                    placements.AddRange(FindPossibleWordPlacements(ref blocks, word));
                }

                if (!placements.Any())
                {
                    // Return if no more _words can be placed
                    return new Generation(blocks, unplacedWords);
                }

                placements.Sort(new PlacementComparer()); //sort placements by least new letters added to board
                var bestPlacement = placements.First();
                //ToDo: Maybe choose a random placement, instead of the "best" one. Otherwise the same solutions will keep being generated.
                PlaceWordOnBoard(ref blocks, bestPlacement);

                //ToDo: Do I still need this Placed attribute?
                unplacedWords.Remove(bestPlacement.Word);

                if (!unplacedWords.Any())
                {
                    return new Generation(blocks, unplacedWords);
                }
            }
        }

        private List<Placement> FindPossibleWordPlacements(ref Block[,] blocks, Word word)
        {
            List<Placement> placements = new List<Placement>();

            for (int i = 0; i < word.Letters.Length; i++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    for (int x = 0; x < blocks.GetLength(0); x++)
                    {
                        Block block = blocks[x, y];
                        if (block != null && block.letter.Character == word.Letters[i].Character) // check if block == letter
                        {
                            Placement placement;
                            if (WordCanBePlacedVertically(ref blocks, word, block, new BlockCoordinates(x + 1, y + 1), i + 1, out placement))
                            {
                                if (placement != null) placements.Add(placement);
                            }
                            else if (WordCanBePlacedHorizontally(ref blocks, word, block, new BlockCoordinates(x + 1, y + 1), i + 1, out placement))
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

        private bool WordCanBePlacedHorizontally(ref Block[,] blocks, Word word, Block block, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word);
            BlockCoordinates currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(ref blocks, currentBlock.ArrayX - 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayX - 1, currentBlock.ArrayY] != null ||
                !OutOfBounds(ref blocks, currentBlock.ArrayX + 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayX + 1, currentBlock.ArrayY] != null)
            {
                return false;
            }

            placement.Coordinates = new BlockCoordinates[word.Letters.Length];
            placement.Coordinates[letterIndex - 1] = blockCoordinates;


            for (int i = letterIndex - 1; i > 0; i--) //check all preceding letters
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
            for (int i = letterIndex + 1; i <= word.WordLength; i++) //check all preceding letters
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

        private bool WordCanBePlacedVertically(ref Block[,] blocks, Word word, Block block, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word);
            BlockCoordinates currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(ref blocks, currentBlock.ArrayX, currentBlock.ArrayY + 1) && blocks[currentBlock.ArrayX, currentBlock.ArrayY + 1] != null ||
                !OutOfBounds(ref blocks, currentBlock.ArrayX, currentBlock.ArrayY - 1) && blocks[currentBlock.ArrayX, currentBlock.ArrayY - 1] != null)
            {
                return false;
            }

            placement.Coordinates = new BlockCoordinates[word.Letters.Length];
            placement.Coordinates[letterIndex - 1] = blockCoordinates;


            for (int i = letterIndex - 1; i > 0; i--) //check all preceding letters
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
            for (int i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
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

        private bool LetterCanBePlaced(ref Block[,] blocks, BlockCoordinates blockCoordinates, Letter[] letters, int letterIndex, int nextLetterStep, string direction, BlockCoordinates nextBlockCoordinates, Placement placement)
        {
            Letter letter = letters[letterIndex - 1];
            Block nextBlock = OutOfBounds(ref blocks, nextBlockCoordinates.ArrayX, nextBlockCoordinates.ArrayY)
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

            Letter nextLetter = letterIndex + nextLetterStep > letters.Length || letterIndex + nextLetterStep < 1 ? null : letters[letterIndex + nextLetterStep - 1];

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


        private void PlaceWordOnBoard(ref Block[,] blocks, Placement placement)
        {
            if (placement.Expansion.TotalX != 0 || placement.Expansion.TotalY != 0)
                blocks = ExpandCrosswordSpace(ref blocks, placement);
            SetLetterToPlacementCoordinates(placement);
            foreach (Letter letter in placement.Word.Letters)
            {
                blocks[letter.Coordinates.ArrayX, letter.Coordinates.ArrayY] = new Block(letter);
            }
            placement.Word.Placed = true;
        }

        private Block[,] ExpandCrosswordSpace(ref Block[,] blocks, Placement placement)
        {
            var newBlocks = new Block[blocks.GetLength(0) + Math.Abs(placement.Expansion.TotalX), blocks.GetLength(1) + Math.Abs(placement.Expansion.TotalY)];

            for (var y = 0; y < blocks.GetLength(1); y++)
            {
                for (var x = 0; x < blocks.GetLength(0); x++)
                {
                    if (blocks[x, y] != null) blocks[x, y].letter.Coordinates.ShiftCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                    newBlocks[x + placement.Expansion.Left, y + placement.Expansion.Up] = blocks[x, y];
                }
            }
            return newBlocks;
        }


        private void SetLetterToPlacementCoordinates(Placement placement)
        {
            for (int i = 0; i < placement.Word.WordLength; i++)
            {
                placement.Word.Letters[i].Coordinates = placement.Coordinates[i];
            }
        }

        private bool OutOfBounds(ref Block[,] blocks, int x, int y)
        {
            return !(x < blocks.GetLength(0) && x >= 0 && y < blocks.GetLength(1) && y >= 0);
        }
    }
}
