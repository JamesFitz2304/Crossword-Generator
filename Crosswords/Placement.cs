namespace Crosswords
{
    public class Placement
    {
        public BlockCoordinates[] Coordinates;
        public Word Word;
        public Expansion Expansion = new Expansion();
        public int NewLetters = 0;

        public Placement(Word word)
        {
            Coordinates = new BlockCoordinates[word.WordLength];
            this.Word = word;
        }
    }
}