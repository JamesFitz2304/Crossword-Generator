using System;
using System.Collections.Generic;
using System.Linq;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.GenerationManager
{
    public class Generation
    {
        public LetterBlock[,] Blocks;
        public readonly IList<PlacedWord> PlacedWords;
        public readonly IList<string> UnplacedWords;

        public Generation(LetterBlock[,] blocks, IList<Word> words, IList<PlacedWord> placedWords, IList<string> unplacedWords)
        {
            Blocks = blocks;
            PlacedWords = placedWords;
            UnplacedWords = unplacedWords;
        }

        public int NumberOfUnplacedWords => UnplacedWords.Count;
        public int BlocksSize => Blocks.GetLength(0) * Blocks.GetLength(1);

        public int XSize => Blocks.GetLength(1);

        public int YSize => Blocks.GetLength(0);

        public double SizeRatio => (double)Math.Min(XSize, YSize)/Math.Max(XSize, YSize);
    }
}
