using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using CrosswordGenerator.Models;

namespace CrosswordGenerator.GenerationManager
{
    public class GenerationManager
    {
        private readonly IGenerator _generator;
        private const int DefaultAttempts = 8;
        private const int MaxAllPlaced = 3;

        public GenerationManager(IGenerator generator)
        {
            _generator = generator;
        }

        public IEnumerable<Generation> GenerateCrosswords(IList<WordCluePair> wordCluePairs, int attempts = DefaultAttempts, int timeout = 3000, bool cullIdenticals = true)
        {
            if (wordCluePairs.Count < 2)
            {
                throw new FormatException("Word list must be greater than 1");
            }
            if (!AllWordsValid(wordCluePairs))
            {
                throw new FormatException("Word list contained invalid characters");
            }

            var words = wordCluePairs.Select(x => new Word(x.Word, x.Id)).ToList();

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


                if (generations.Count(x => x.NumberOfUnplacedWords == 0) >= MaxAllPlaced)
                {
                    break;
                }
            }

            if (cullIdenticals)
                RemoveIdenticalGenerations(generations);

            // Setup word numbers and clues here?

            // Puzzle: LetterBlocks, Unplaceable Words, Words with number & across & clue

            return generations;
        }

        private static bool AllWordsValid(IEnumerable<WordCluePair> wordCluePairs)
        {
            var regex = new Regex(@"[^A-Z]");
            if (wordCluePairs.Any(word => regex.IsMatch(word.Word) || string.IsNullOrWhiteSpace(word.Word)))
            {
                return false;
            }
            return true;
        }

        private static void RemoveIdenticalGenerations(IList<Generation> generations)
        {
            for (var x = 0; x < generations.Count - 1; x++)
            {
                for (var y = x + 1; y < generations.Count; y++)
                {
                    var xLetterBlocks = generations[x].Blocks;
                    var yLetterBlocks = generations[y].Blocks;
                    if (BlocksAreIdentical(xLetterBlocks, yLetterBlocks))
                    {
                        generations.RemoveAt(y);
                        y--;
                    }
                }
            }
        }

        private static IList<Word> ShuffleWords(IEnumerable<Word> words)
        {
            var shuffled = words.OrderByDescending(x => x.WordLength).ToList();

            // always start with the longest word
            const int numberToSort = 1;
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

        private static bool BlocksAreIdentical(LetterBlock[,] LetterBlocks1, LetterBlock[,] LetterBlocks2)
        {
            if (Enumerable.Range(0, LetterBlocks1.Rank).Any(dimension => LetterBlocks1.GetLength(dimension) != LetterBlocks2.GetLength(dimension)))
            {
                return false;
            }

            var flatLetterBlocks1 = LetterBlocks1.Cast<LetterBlock>().Select(b => b?.Character);
            var flatLetterBlocks2 = LetterBlocks2.Cast<LetterBlock>().Select(b => b?.Character);

            return flatLetterBlocks1.SequenceEqual(flatLetterBlocks2);
        }
    }
}