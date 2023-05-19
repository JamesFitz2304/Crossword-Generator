using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.GenerationManager
{
    public class Generation
    {
        public Dictionary<Point, LetterBlock> Blocks;
        public readonly IList<PlacedWord> PlacedWords;
        public readonly IList<string> UnplacedWords;

        public Generation(Dictionary<Point, LetterBlock> blocks, IList<PlacedWord> placedWords,
            IList<string> unplacedWords)
        {
            Blocks = blocks;
            PlacedWords = placedWords;
            UnplacedWords = unplacedWords;
        }

        public int NumberOfUnplacedWords => UnplacedWords.Count;
        public int BlocksSize => XSize * YSize;

        public int XSize => Blocks.Keys.Max(p => p.X);

        public int YSize => Blocks.Keys.Max(p => p.Y);

        public double SizeRatio => (double)Math.Min(XSize, YSize)/Math.Max(XSize, YSize);
    }
}
