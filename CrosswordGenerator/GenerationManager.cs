using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CrosswordGenerator.Interfaces;

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
                    if (generations.All(gen => !BlocksAreIdentical(generation.blocks, gen.blocks)))
                    {
                        generations.Add(generation);

                    }
                }
            }

            return generations;
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

            var flatBlock1 = blocks1.Cast<Block>();
            var flatBlock2 = blocks2.Cast<Block>();

            var flatBlock3 = blocks1.Cast<Block>().Select(b => b?.letter.Character);
            var flatBlock4 = blocks2.Cast<Block>().Select(b => b?.letter.Character);

            return flatBlock3.SequenceEqual(flatBlock4);

        }
    }
}
