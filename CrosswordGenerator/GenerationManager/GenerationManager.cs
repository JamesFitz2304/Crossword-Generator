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
    public interface IGenerationManager
    {
        IEnumerable<Generation> GenerateCrosswords(IList<WordCluePair> wordCluePairs, int maxAttempts = 35,
            int maxGenerations = 15, int timeout = 3000, bool cullIdenticals = true);
    }

    public class GenerationManager : IGenerationManager
    {
        private readonly IGenerator _generator;

        public GenerationManager(IGenerator generator)
        {
            _generator = generator;
        }

        public IEnumerable<Generation> GenerateCrosswords(IList<WordCluePair> wordCluePairs, int maxAttempts = 35,
            int maxGenerations = 15, int timeout = 3000, bool cullIdenticals = true)
        {
            if (wordCluePairs.Count < 2)
            {
                throw new FormatException("Word list must be greater than 1");
            }
            if (!AllWordsValid(wordCluePairs))
            {
                throw new FormatException("Word list contained invalid characters");
            }

            var words = wordCluePairs.Select(x => new Word(x.Word, x.Id)).OrderByDescending(x => x.WordLength).ToList();

            var generations = new List<Generation>();
            var leastUnplacedWords = int.MaxValue;

            var watch = new Stopwatch();
            watch.Start();

            for (var i = 0; i < maxAttempts; i++)
            {
                if (watch.ElapsedMilliseconds > timeout) break;
                var generation = _generator.Generate(words);

                if (generation.SizeRatio < 0.8)
                {
                    continue;
                }

                if (generation.NumberOfUnplacedWords > leastUnplacedWords) 
                    continue;

                if (generation.NumberOfUnplacedWords < leastUnplacedWords)
                {
                    generations.Clear();
                    leastUnplacedWords = generation.NumberOfUnplacedWords;
                }

                generations.Add(generation);


                if (generations.Count(x => x.NumberOfUnplacedWords == 0) >= maxGenerations)
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

        private static bool BlocksAreIdentical(LetterBlock[,] letterBlocks1, LetterBlock[,] letterBlocks2)
        {
            if (Enumerable.Range(0, letterBlocks1.Rank).Any(dimension => letterBlocks1.GetLength(dimension) != letterBlocks2.GetLength(dimension)))
            {
                return false;
            }

            var flatLetterBlocks1 = letterBlocks1.Cast<LetterBlock>().Select(b => b?.Character);
            var flatLetterBlocks2 = letterBlocks2.Cast<LetterBlock>().Select(b => b?.Character);

            return flatLetterBlocks1.SequenceEqual(flatLetterBlocks2);
        }

    }
}

//ToDo: Create puzzle model with all completed puzzle fields. Should include hints, placement info etc
//ToDo: Convert from using array of blocks to a Dictionary<point, block>
//ToDo: Extract out placement finding to PlacementFinder?