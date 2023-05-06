using System.Collections.Generic;
using System.Linq;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.GenerationManager
{
    public class Generation
    {
        public Block[,] Blocks;
        public readonly IList<Word> Words;

        public int NumberOfUnplacedWords => Words.Count(word => !word.Placed);

        public Generation(Block[,] blocks, IList<Word> words)
        {
            Blocks = blocks;
            Words = words;
        }
    }
}
