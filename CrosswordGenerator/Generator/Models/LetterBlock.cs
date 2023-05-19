using System.Drawing;

namespace CrosswordGenerator.Generator.Models
{
    public class LetterBlock
    {
        public Point Coordinates;
        public char Character;

        public LetterBlock(char character)
        {
            Character = character;
        }

        public LetterBlock(char character, Point coordinates)
        {
            Character = character;
            Coordinates = coordinates;
        }
    }
}
