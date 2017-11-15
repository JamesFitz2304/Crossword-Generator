namespace Crosswords
{
    public class Placement
    {
        public BlockCoordinates[] Coordinates;
        public int XExpansion = 0;
        public int YExpansion = 0;

        public Placement(int wordLength)
        {
            Coordinates = new BlockCoordinates[wordLength];
        }
    }
}