using System;
using System.Collections.Generic;

namespace Crosswords
{
    public class CrosswordGenerator
    {
        private readonly List<Word> Words;

        //private CrosswordSpace Space = new CrosswordSpace();
        public Block[,] blocks;

        private int wordsPlaced = 0;
        public CrosswordGenerator(List<Word> words)
        {
            Words = words;
        }

        public bool Generate()
        {
            for (int i = 0; i < Words.Count; i++)
            {
                wordsPlaced = 0;
                if (PlaceFirstWord(Words[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool PlaceFirstWord(Word word)
        {
            blocks = new Block[word.WordLength, 1];
            Placement placement = new Placement(word.WordLength);

            for (int i = 1; i <= word.WordLength; i++)
            {
                BlockCoordinates coordinates = new BlockCoordinates(i, 1);
                placement.Coordinates[i-1] = coordinates;
            }
            PlaceWordOnBoard(word, placement);
            wordsPlaced++;
            return PlaceNextWord();
        }

        private bool PlaceNextWord()
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].Placed) continue;
                List<Placement> placements = FindPossibleWordPlacements(Words[i]);

                foreach (Placement placement in placements)
                {
                    Block[,] blockState = blocks;
                    PlaceWordOnBoard(Words[i], placement);
                    wordsPlaced++;
                    if (PlaceNextWord() || wordsPlaced == Words.Count)
                    {
                        return true;
                    }
                    blocks = blockState;
                    ReverseWordPlacement(placement, Words[i]);
                    return false;
                }
            }
            return false;
        }

        private void ReverseWordPlacement(Placement placement, Word word)
        {
            word.Placed = false;
            wordsPlaced--;
            if (placement.Expansion.TotalX == 0 && placement.Expansion.TotalY == 0) return;
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    if (blocks[x, y] != null) blocks[x, y].letter.Coordinates.ShiftCoordinates(-placement.Expansion.Left, -placement.Expansion.Up);
                }
            }
        }

        private List<Placement> FindPossibleWordPlacements(Word word)
        {
            List<Placement> placements = new List<Placement>();

            for (int i = 0; i < word.Letters.Length; i++)
            {
                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    for (int y = 0; y < blocks.GetLength(1); y++)
                    {
                        Block block = blocks[x, y];
                        if (block != null && block.letter.Character == word.Letters[i].Character) // check if block == letter
                        {
                            Placement placement;
                             if (WordCanBePlacedVertically(word, block, new BlockCoordinates(x+1, y+1),  i + 1, out placement))
                            {
                                if (placement != null) placements.Add(placement);
                            }
                            else if (WordCanBePlacedHorizontally(word, block, new BlockCoordinates(x + 1, y + 1), i + 1, out placement))
                            {
                                if (placement != null) placements.Add(placement);
                            }
                        }
                    }
                }
            }
            //add each new possible placement to list
            placements.Sort(new PlacementComparer()); //sort placements by least new letters added to board
            return placements;
        }

        private bool WordCanBePlacedHorizontally(Word word, Block block, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word.WordLength);
            BlockCoordinates currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(currentBlock.ArrayX - 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayX - 1, currentBlock.ArrayY] != null ||
                !OutOfBounds(currentBlock.ArrayX + 1, currentBlock.ArrayY) && blocks[currentBlock.ArrayX + 1, currentBlock.ArrayY] != null)
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
                else if (!LetterCanBePlaced(currentBlock, word.Letters, i, -1, "L", new BlockCoordinates(currentBlock.X-1, currentBlock.Y), placement))
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
                else if (!LetterCanBePlaced(currentBlock, word.Letters, i, 1, "R", new BlockCoordinates(currentBlock.X + 1, currentBlock.Y), placement))
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

        private bool WordCanBePlacedVertically(Word word, Block block, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement(word.WordLength);
            BlockCoordinates currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);

            if (!OutOfBounds(currentBlock.ArrayX, currentBlock.ArrayY + 1) && blocks[currentBlock.ArrayX, currentBlock.ArrayY + 1] != null ||
                !OutOfBounds(currentBlock.ArrayX, currentBlock.ArrayY - 1) && blocks[currentBlock.ArrayX , currentBlock.ArrayY - 1] != null)
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
                else if (!LetterCanBePlaced(currentBlock, word.Letters, i, -1 , "U", new BlockCoordinates(currentBlock.X, currentBlock.Y - 1), placement))
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
                else if (!LetterCanBePlaced(currentBlock, word.Letters, i, 1 , "D", new BlockCoordinates(currentBlock.X, currentBlock.Y + 1), placement))
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

        private bool LetterCanBePlaced(BlockCoordinates blockCoordinates, Letter[] letters, int letterIndex, int nextLetterStep, string direction, BlockCoordinates nextBlockCoordinates, Placement placement)
        {
            Letter letter = letters[letterIndex-1];
            Block nextBlock = OutOfBounds(nextBlockCoordinates.ArrayX, nextBlockCoordinates.ArrayY)
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

            Letter nextLetter = letterIndex + nextLetterStep > letters.Length || letterIndex+nextLetterStep < 1 ? null : letters[letterIndex+nextLetterStep-1];

            //Fail if the above/below or left/right blocks have letters when direction is horizontal/vertical
            if (direction == "L" || direction == "R")
            {
                if (!OutOfBounds(blockCoordinates.ArrayX, blockCoordinates.ArrayY + 1) && blocks[blockCoordinates.ArrayX, blockCoordinates.ArrayY + 1] != null ||
                    !OutOfBounds(blockCoordinates.ArrayX, blockCoordinates.ArrayY - 1) && blocks[blockCoordinates.ArrayX, blockCoordinates.ArrayY - 1] != null)
                {
                    return false;
                }
            }
            else
            {
                if (!OutOfBounds(blockCoordinates.ArrayX + 1, blockCoordinates.ArrayY) && blocks[blockCoordinates.ArrayX + 1, blockCoordinates.ArrayY] != null ||
                    !OutOfBounds(blockCoordinates.ArrayX - 1, blockCoordinates.ArrayY) && blocks[blockCoordinates.ArrayX - 1, blockCoordinates.ArrayY] != null)
                {
                    return false;
                }
            }
            placement.NewLetters++;
            return nextLetter == null && nextBlock == null || nextLetter != null && (nextBlock == null || nextBlock.letter.Character == nextLetter.Character);
        }


        private void PlaceWordOnBoard(Word word, Placement placement)
        {
            if (placement.Expansion.TotalX != 0 || placement.Expansion.TotalY != 0)
                ExpandCrosswordSpace(placement);
            SetLetterToPlacementCoordinates(word, placement);
            foreach (Letter letter in word.Letters)
            {
                blocks[letter.Coordinates.ArrayX, letter.Coordinates.ArrayY] = new Block(letter);
            }
            word.Placed = true;
        }

        private void ExpandCrosswordSpace(Placement placement)
        {
            Block[,] newBlocks = new Block[blocks.GetLength(0) + Math.Abs(placement.Expansion.TotalX), blocks.GetLength(1) + Math.Abs(placement.Expansion.TotalY)];

            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    if (blocks[x, y] != null) blocks[x, y].letter.Coordinates.ShiftCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                    newBlocks[x + placement.Expansion.Left, y + placement.Expansion.Up] = blocks[x, y];
                }
            }

            blocks = newBlocks;

        }


        private void SetLetterToPlacementCoordinates(Word word, Placement placement)
        {
            for (int i = 0; i<word.WordLength; i++)
            {
                word.Letters[i].Coordinates = placement.Coordinates[i];
            }
        }

        private bool OutOfBounds(int x, int y)
        {
            return!(x < blocks.GetLength(0) && x>=0 && y < blocks.GetLength(1) && y>=0);
        }
    }
}
