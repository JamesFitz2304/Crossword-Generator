namespace CrosswordGenerator.Generator.Models
{
    public class Placement
    {
        public LetterBlock[] LetterBlocks;
        public Expansion Expansion = new Expansion();
        public Word Word;
        public int NewLetters = 0;

        public Placement(Word word)
        {
            LetterBlocks = new LetterBlock[word.WordLength];
            Word = word;
        }

    }
}