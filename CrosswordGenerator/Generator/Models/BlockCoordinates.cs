using System.Drawing;

namespace CrosswordGenerator.Generator.Models
{
    public class BlockCoordinates
    {
        public BlockCoordinates(int x, int y)
        {
            Coordinates = new Point(x, y);
        }

        public Point Coordinates;

        public Point ArrayCoordinates => new Point(Coordinates.X-1, Coordinates.Y-1);

        public void ShiftCoordinates(int x, int y)
        {
            Coordinates = new Point(Coordinates.X+x, Coordinates.Y+y);
        }
    }
}
