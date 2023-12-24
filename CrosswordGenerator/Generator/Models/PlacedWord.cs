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
            Start  = coordinates;
        }

        public int Id { get; }

        public string Word { get; }

        public bool Across { get; }

        public Point Start { get; private set; }

        public void ShiftFirstLetterCoordinates(int x, int y)
        {
            Start = new Point(Start.X + x, Start.Y + y);
        }
    }

}

