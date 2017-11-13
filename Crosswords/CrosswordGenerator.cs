using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                blocks = new Block[Words[i].WordLength, 1];

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

        private List<Placement> FindPossibleWordPlacements(Word word)
        {
            List<Placement> placements = new List<Placement>();

            for(int i = 0; i<word.Letters.Length; i++)
            {
                foreach (Block block in blocks)
                {
                    if (!block.Free && block.letter.Character == word.Letters[i].Character) // check if block == letter
                    {
                        Placement placement;
                        if (WordCanBePlacedVertically(word, block, i+1, out placement))
                        {
                            if(placement != null) placements.Add(placement);
                        }
                        else if (WordCanBePlacedHorizontally(word, block, i+1, out placement))
                        {
                            if (placement != null) placements.Add(placement);
                        }
                    }
                }
            }
            //add each new possible placement to list
            placements.Sort(new PlacementComparer(blocks.GetLength(0), blocks.GetLength(1))); //sort placements by least expansion required
            return placements;
        }

        private bool WordCanBePlacedHorizontally(Word word, Block block, int letterIndex, out Placement placement)
        {
            placement = new Placement();

            for (int i = block.Coordinates.X; i > block.Coordinates.X - letterIndex; i--) //check all preceding letters
            {
                //if index is less than 0, set x expansion + 1
                // else...
                if(LetterCanBePlaced(block[i-1, block.ArrayY], word.Letters[i-1]))


            }

            return false;
        }

        private bool LetterCanBePlaced(BlockCoordinates block, Letter letter)
        {
            return false;
        }

        private bool WordCanBePlacedVertically(Word word, Block block, int letterIndex, out Placement placement)
        {
            placement = new Placement();
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