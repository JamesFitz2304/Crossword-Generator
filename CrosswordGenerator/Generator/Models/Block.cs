namespace CrosswordGenerator.Generator.Models
{
    public class Block
    {
        public Letter Letter { get; }

        public Block(Letter letter)
        {
            Letter = letter;
        }
    }
}
