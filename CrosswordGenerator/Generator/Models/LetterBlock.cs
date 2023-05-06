namespace CrosswordGenerator.Generator.Models
{
    public class LetterBlock
    {
        public BlockCoordinates Coordinates;
        public char Character;

        public LetterBlock(char character)
        {
            Character = character;
        }

        public LetterBlock(char character, BlockCoordinates coordinates)
        {
            Character = character;
            Coordinates = coordinates;
        }
    }
}
