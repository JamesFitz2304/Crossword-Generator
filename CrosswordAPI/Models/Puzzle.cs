using System.Drawing;

namespace CrosswordAPI.Models
{
    public class Puzzle
    {
        public IEnumerable<PuzzleBlock> Blocks { get; set; }
        public IEnumerable<PuzzleWord> Words { get; set; }
    }

    public class PuzzleBlock
    {
        public Point Coordinate { get; set; }
        public char Character { get; set; }
    }

    public class PuzzleWord
    {
        public string Word { get; set; }
        public string Clue { get; set; }
        public int Order { get; set; }
        public Point Start { get; set; }
        public bool Across { get; set; }
    }
}