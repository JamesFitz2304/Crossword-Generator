using System.Collections.Generic;

namespace CrosswordGenerator
{
    public class Generation
    {
        public Block[,] Blocks;
        private readonly List<Word> _unplacedWords;
        public int NumberOfUnplacedWords => _unplacedWords.Count;

        public Generation(Block[,] blocks, List<Word> unplacedWords)
        {
            Blocks = blocks;
            _unplacedWords = unplacedWords;
        }
    }
}
