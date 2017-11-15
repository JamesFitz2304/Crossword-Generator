using System.Collections.Generic;

namespace Crosswords
{
    public class CrosswordGenerator
    {
        private readonly List<Word> Words;

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

        private bool PlaceFirstWord(Word word)
        {
            blocks = new Block[word.WordLength, 1];
            Placement placement = new Placement(word.WordLength);

            for (int i = 1; i <= word.WordLength; i++)
            {
                BlockCoordinates coordinates = new BlockCoordinates(i, 1);
                placement.Coordinates[i-1] = coordinates;
                blocks[i - 1, 0] = new Block();
            }
            PlaceWordOnBoard(word, placement);

            return PlaceNextWord();
        }

        private bool PlaceNextWord()
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if(Words[i].Placed) continue;
                List<Placement> placements = FindPossibleWordPlacements(Words[i]);

                foreach (Placement placement in placements)
                {
                    PlaceWordOnBoard(Words[i], placement);
                    if (PlaceNextWord())
                    {
                        return true;
                    }
                    //TODO ELSE WILL NEED TO ROLL BACK LAST WORD. REMOVE FROM BOARD
                    //COULD WE SAVE THE STATE OF THE GAME? THE BLOCKS AND WORD LIST? OR IS IT EASIER TO JUST MANUALLY REMOVE THE WORD AND RESIZE ARRAY 
                }
            }
            return false;
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
            placement = new Placement(word.WordLength);
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
            placement = new Placement(word.WordLength);
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
            //TODO if expansion is needed, this is the place to do it!!!
            SetLetterToPlacementCoordinates(word, placement);
            foreach (Letter letter in word.Letters)
            {
                blocks[letter.Coordinates.X - 1, letter.Coordinates.Y - 1].letter = letter;
                blocks[letter.Coordinates.X - 1, letter.Coordinates.Y - 1].Free = false;
            }
            word.Placed = true;
        }

        private static void SetLetterToPlacementCoordinates(Word word, Placement placement)
        {
            for (int i = 0; i<word.WordLength; i++)
            {
                word.Letters[i].Coordinates = placement.Coordinates[i];
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
