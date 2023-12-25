using System.Collections.Generic;

namespace CrosswordGenerator.Models.Puzzle
{
    public class Puzzle
    {
        public IEnumerable<PuzzleBlock> Blocks { get; set; }
        public IEnumerable<PuzzleWord> Words { get; set; }
    }
}