namespace Crosswords
{
    public class Placement
    {
        public BlockCoordinates[] Coordinates;
        public Expansion Expansion = new Expansion();
        public int NewLetters = 0;

        public Placement(int wordLength)
        {
            Coordinates = new BlockCoordinates[wordLength];
        }
    }
}