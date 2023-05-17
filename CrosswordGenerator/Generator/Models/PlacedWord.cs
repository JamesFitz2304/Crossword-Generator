using System.Drawing;

namespace CrosswordGenerator.Generator.Models
{
    public class PlacedWord
    {
        public PlacedWord(int id, string word, bool across, Point coordinates)
        {
            Id = id;
            Word = word;
            Across = across;
            FirstLetterCoordinates  = coordinates;
        }

        public int Id { get; }

        public string Word { get; }

        public bool Across { get; }

        public Point FirstLetterCoordinates { get; private set; }

        public void ShiftFirstLetterCoordinates(int x, int y)
        {
            FirstLetterCoordinates = new Point(FirstLetterCoordinates.X + x, FirstLetterCoordinates.Y + y);
        }
    }

}

