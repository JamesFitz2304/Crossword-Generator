using System.Drawing;

namespace CrosswordGenerator.Generator.Models
{
    public class Placement
    {
        public LetterBlock[] LetterBlocks;
        public Expansion Expansion = new Expansion();
        public Word Word { get; set; }
        public int NewLetters = 0;
        public bool Across { get; set; }
        public int ExistingLettersUsed => Word.WordLength - NewLetters;
        public Point FirstLetterCoordinates => LetterBlocks[0].Coordinates?.Coordinates ?? Point.Empty;
 
        public Placement(Word word, bool across)
        {
            LetterBlocks = new LetterBlock[word.WordLength];
            Word = word;
            Across = across;
        }

    }
}