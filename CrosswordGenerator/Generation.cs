using System;
using System.Collections.Generic;
using System.Text;

namespace CrosswordGenerator
{
    public class Generation
    {
        public Block[,] blocks;
        public List<Word> UnplacedWords = new List<Word>();

        public int NumberOfUnplacedWords => UnplacedWords.Count;
    }
}
