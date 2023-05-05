﻿using System.Collections.Generic;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.GenerationManager
{
    public class Generation
    {
        public Block[,] Blocks;
        private readonly IList<Word> _unplacedWords;
        public int NumberOfUnplacedWords => _unplacedWords.Count;

        public Generation(Block[,] blocks, IList<Word> unplacedWords)
        {
            Blocks = blocks;
            _unplacedWords = unplacedWords;
        }
    }
}