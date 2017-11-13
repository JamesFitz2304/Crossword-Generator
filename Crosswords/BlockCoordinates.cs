namespace Crosswords
{
    public class BlockCoordinates
    {
        public int Y;
        public int X;

        public int ArrayY => Y - 1;
        public int ArrayX => X - 1;
    }
}