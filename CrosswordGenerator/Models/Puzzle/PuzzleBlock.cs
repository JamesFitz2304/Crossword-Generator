using System.Collections.Generic;
using System.Drawing;

namespace CrosswordGenerator.Models.Puzzle
{
    public class PuzzleBlock
    {
        public Point Coordinate { get; set; }
        public char Character { get; set; }
        public IEnumerable<PuzzleWord> WordStarts { get; set; }
    }
}