using System.Collections.Generic;

namespace Crosswords
{
    public class CrosswordGenerator
    {
        private List<Word> Words;
        private List<Word> PlacedWords;
        private List<Word> RemainingWords;

        //private CrosswordSpace Space = new CrosswordSpace();
        public Block[,] blocks;
        public CrosswordGenerator(List<Word> words)
        {
            Words = words;
        }

        public bool PlaceWords()
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if (PlaceFirstWord(Words[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool PlaceNextWord()
        {
            for (int i = 0; i < RemainingWords.Count; i++)
            {
                List<Placement> placements = FindPossibleWordPlacements(RemainingWords[i]);

                foreach (Placement placement in placements)
                {
                    PlaceWordOnBoard(RemainingWords[i], placement);
                    if (PlaceNextWord())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool PlaceFirstWord(Word word)
        {
            blocks = new Block[word.WordLength, 1];
            Placement placement = new Placement();

            for (int i = 1; i <= word.WordLength; i++)
            {
                BlockCoordinates coordinates = new BlockCoordinates(i, 1);
                blocks[i - 1, 1] = new Block();
                PlaceWordOnBoard(word, placement);
            }

            return PlaceNextWord();
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
                        if (!block.Free && block.letter.Character == word.Letters[i].Character) // check if block == letter
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
            placements.Sort(new PlacementComparer(blocks.GetLength(0), blocks.GetLength(1))); //sort placements by least expansion required
            return placements;
        }

        private bool WordCanBePlacedHorizontally(Word word, Block block, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement();
            placement.Coordinates = new BlockCoordinates[word.Letters.Length];
            placement.Coordinates[letterIndex - 1] = blockCoordinates;
            BlockCoordinates currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (int i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.X--;
                if (currentBlock.X < 1)
                {
                    placement.XExpansion++;
                }
                else if (!LetterCanBePlaced(currentBlock, word.Letters[letterIndex-1]))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = currentBlock;
            }

            currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (int i = letterIndex + 1; i < word.WordLength; i++) //check all preceding letters
            {
                currentBlock.X++;
                if (currentBlock.X > blocks.GetLength(0))
                {
                    placement.XExpansion++;
                }
                else if (!LetterCanBePlaced(currentBlock, word.Letters[letterIndex - 1]))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = currentBlock;
            }
            return true;
        }

        private bool WordCanBePlacedVertically(Word word, Block block, BlockCoordinates blockCoordinates, int letterIndex, out Placement placement)
        {
            placement = new Placement();
            placement.Coordinates = new BlockCoordinates[word.Letters.Length];
            placement.Coordinates[letterIndex - 1] = blockCoordinates;
            BlockCoordinates currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (int i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentBlock.Y--;
                if (currentBlock.Y < 1)
                {
                    placement.YExpansion++;
                }
                else if (!LetterCanBePlaced(currentBlock, word.Letters[letterIndex - 1]))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = currentBlock;
            }

            currentBlock = new BlockCoordinates(blockCoordinates.X, blockCoordinates.Y);
            for (int i = letterIndex + 1; i < word.WordLength; i++) //check all preceding letters
            {
                currentBlock.Y++;
                if (currentBlock.Y > blocks.GetLength(1))
                {
                    placement.YExpansion++;
                }
                else if (!LetterCanBePlaced(currentBlock, word.Letters[letterIndex - 1]))
                {
                    return false;
                }
                placement.Coordinates[i - 1] = currentBlock;
            }
            return true;
        }

        private bool LetterCanBePlaced(BlockCoordinates blockCoordinates, Letter letter)
        {
            return false;
        }

        private bool LetterCanBePlaced()
        {
            return false;
        }

        private void PlaceWordOnBoard(Word word, Placement placement)
        {
            SetLetterToPlacementCoordinates(word, placement);
            foreach (Letter letter in word.Letters)
            {
                blocks[letter.Coordinates.X - 1, letter.Coordinates.Y - 1].letter = letter;
            }
            word.Placed = true;
        }

        private static void SetLetterToPlacementCoordinates(Word word, Placement placement)
        {
            int i = 0;

            foreach (BlockCoordinates coordinates in placement.Coordinates)
            {
                word.Letters[i].Coordinates = coordinates;
            }
        }
    }
}


/* For each letter in the word
 * Try to find that letter already on the board
 * If found, 
 * If there is a letter to the left or right, it would have to be vertical
 * If there is a letter above or below, it would have to be horizontal
 * If letter has other letter horizontally or vertical, it isn't viable
 * 
 * If horizontal, for each previous letter, check it can be placed to the left (no adjacent letters) and place
 * Then do again with all letters to the right
 * 
 * If vertical, do the same but above and below
 * 
 * 
 * If it turns out a letter is impossible to place, go back through all previosuly placed letters and remove their X Y coordinates
 * Try next starting position  
 * */


    //TO DO
            //IF THE BOARD MUST EXPAND, HOW DO WE DEAL WITH THIS? NEGATIVE COORDINATES? - POSSIBLY NEED A METHOD TO SHUFFLE ALL LETTERS
            // CHECK IF WORD CAN BE PLACED VERTICALLY AND HORIZONTALLY 