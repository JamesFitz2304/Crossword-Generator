using System.Collections.Generic;
using System.Linq;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.GenerationManager
{
    public class Generation
    {
        public LetterBlock[,] Blocks;
        public readonly IList<Word> Words;

        public int NumberOfUnplacedWords => Words.Count(word => !word.Placed);

        public Generation(LetterBlock[,] blocks, IList<Word> words, List<PlacedWord> placedWords)
        {
            Blocks = blocks;
            Words = words;
        }
    }
}
