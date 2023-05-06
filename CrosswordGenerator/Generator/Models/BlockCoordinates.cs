namespace CrosswordGenerator.Generator.Models
{
    public class BlockCoordinates
    {
        public BlockCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int Y { get; set; }
        public int X { get; set; }

        public int ArrayY => Y - 1;
        public int ArrayX => X - 1;

        public void ShiftCoordinates(int x, int y)
        {
            X += x;
            Y += y;
        }
    }
}
