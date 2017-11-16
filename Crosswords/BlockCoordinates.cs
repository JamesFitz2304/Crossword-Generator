namespace Crosswords
{
    public class BlockCoordinates
    {
        public int Y;
        public int X;

        public int ArrayY => Y - 1;
        public int ArrayX => X - 1;

        public BlockCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void ShiftCoordinates(int x, int y)
        {
            X += x;
            Y += y;
        }

        public BlockCoordinates()
        {
            Y = 0;
            X = 0;
        }
    }
}
