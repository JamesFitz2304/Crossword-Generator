using System.Collections.Generic;
using System.Linq;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.GenerationManager
{
    public class Generation
    {
        public Block[,] Blocks;
        private IList<Word> _words;

        public int NumberOfUnplacedWords => _words.Count(word => word.Placed);

        public Generation(Block[,] blocks, IList<Word> words)
        {
            Blocks = blocks;
            _words = words;
        }
    }
}
