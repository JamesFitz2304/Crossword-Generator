using CrosswordGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrosswordGenerator
{
    public class GenerationManager
    {
        private readonly IGenerator _generator;
        private const int DefaultAttempts = 200;

        public GenerationManager(IGenerator generator)
        {
            _generator = generator;
        }

        public IEnumerable<Generation> GenerateCrosswords(List<Word> words, int attempts = DefaultAttempts, int timeout = 3000, bool cullIdenticals = true)
        {
            if (!AllWordsValid(words))
            {
                throw new FormatException("Word list contained invalid characters");
            }

            var generations = new List<Generation>();
            var leastUnplacedWords = int.MaxValue;

            var watch = new Stopwatch();
            watch.Start();

            for (var i = 0; i < attempts; i++)
            {
                if (watch.ElapsedMilliseconds > timeout) break;
                var shuffledWords = ShuffleWords(words);
                var generation = _generator.Generate(shuffledWords);

                if (generation.NumberOfUnplacedWords < leastUnplacedWords)
                {
                    leastUnplacedWords = generation.NumberOfUnplacedWords;
                }

                generations.Add(generation);


                if (generations.Count(x => x.NumberOfUnplacedWords == 0) >= 10)
                {
                    break;
                }
            }

            if (cullIdenticals)
                RemoveIdenticalGenerations(generations);

            return generations;
        }

        private bool AllWordsValid(IList<Word> words)
        {
            var regex = new Regex(@"[^A-Z]");
            if (words.Any(word => regex.IsMatch(word.WordAsString) || string.IsNullOrWhiteSpace(word.WordAsString)))
            {
                return false;
            }
            return true;
        }

        private void RemoveIdenticalGenerations(IList<Generation> generations)
        {
            for (var x = 0; x < generations.Count - 1; x++)
            {
                for (var y = x + 1; y < generations.Count; y++)
                {
                    var xBlocks = generations[x].Blocks;
                    var yBlocks = generations[y].Blocks;
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
            var shuffled = words.OrderByDescending(x => x.WordLength).ToList();
            var numberToSort = 2;
            var longest = shuffled.Take(numberToSort).ToList();
            shuffled.RemoveRange(0, numberToSort);
            var random = new Random();
            var n = shuffled.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                (shuffled[k], shuffled[n]) = (shuffled[n], shuffled[k]);
            }
            shuffled.InsertRange(0, longest);
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