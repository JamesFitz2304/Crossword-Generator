using System.Drawing;

namespace CrosswordGenerator.Models.Puzzle
{
    public class PuzzleWord
    {
        public string Word { get; set; }
        public string Clue { get; set; }
        public int Order { get; set; }
        public Point Start { get; set; }
        public bool Across { get; set; }
    }
}