using System.Drawing;
using CrosswordGenerator.Generator.Models;

namespace CrosswordAPI.Models
{
    public class Puzzle
    {
        public LetterBlock[,] Blocks;
        public IEnumerable<PuzzleWord> Words { get; set; }
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