namespace CrosswordGenerator.Generator.Models
{
    public class Letter
    {
        public BlockCoordinates Coordinates;
        public char Character;

        public Letter(char character)
        {
            Character = character;
        }
    }
}
