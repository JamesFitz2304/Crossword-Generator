using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CrosswordGenerator.Interfaces;
using Microsoft.VisualBasic.CompilerServices;

namespace CrosswordGenerator
{
    public class GenerationManager
    {
        private IGenerator _generator;

        public GenerationManager(IGenerator generator)
        {
            _generator = generator;
        }

        public IEnumerable<Generation> GenerateCrosswords(List<Word> words, int attempts = 100, int timeout = 3000)
        {

            var generations = new List<Generation>();
            var leastUnplacedWords = int.MaxValue;

            var watch = new Stopwatch();
            watch.Start();
      

            for (var i = 0; i < attempts; i++)
            {
                if (watch.ElapsedMilliseconds > timeout) break;
                var shuffledWords = ShuffleWords(words);
                var generation = _generator.Generate(shuffledWords);
                var unplacedWords = generation.UnplacedWords.Count;

                if (unplacedWords < leastUnplacedWords)
                {
                    leastUnplacedWords = unplacedWords;
                    generations.Clear();
                    generations.Add(generation);
                }
                else if (unplacedWords == leastUnplacedWords)
                {
                    generations.Add(generation);
                }
            }

            RemoveIdenticalGenerations(generations); 
            
            return generations;
        }

        private void RemoveIdenticalGenerations(IList<Generation> generations)
        {
            for (var x = 0; x < generations.Count - 1; x++)
            {
                for (var y = x + 1; y < generations.Count; y++)
                {
                    var xBlocks = generations[x].blocks;
                    var yBlocks = generations[y].blocks;
                    if (BlocksAreIdentical(xBlocks, yBlocks))
                    {
                        generations.RemoveAt(y);
                        y--;
                    }
                }
            }
        }


        private IList<Word> ShuffleWords(IEnumerable<Word> words)
        {
            var shuffled = words.ToList();
            var random = new Random();
            var n = shuffled.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                (shuffled[k], shuffled[n]) = (shuffled[n], shuffled[k]);
            }

            return shuffled;
        }

        private bool BlocksAreIdentical(Block[,] blocks1, Block[,] blocks2)
        {
            if (Enumerable.Range(0, blocks1.Rank).Any(dimension => blocks1.GetLength(dimension) != blocks2.GetLength(dimension)))
            {
                return false;
            }

            var flatBlocks1 = blocks1.Cast<Block>().Select(b => b?.letter.Character);
            var flatBlocks2 = blocks2.Cast<Block>().Select(b => b?.letter.Character);

            return flatBlocks1.SequenceEqual(flatBlocks2);

        }
    }
}
