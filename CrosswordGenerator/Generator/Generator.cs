using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CrosswordGenerator.GenerationManager;
// ReSharper disable InvertIf

namespace CrosswordGenerator.Generator
{
    public class Generator : IGenerator
    {
        private readonly IPlacementFinder _placementFinder;

        public Generator(IPlacementFinder placementFinder)
        {
            _placementFinder = placementFinder;
        }

        public Generation Generate(IList<Word> words)
        {
            foreach (var word in words) word.Placed = false;

            var firstWord = words.First();

            var blocksDict = new Dictionary<Point, LetterBlock>();
            var placedWords = new List<PlacedWord>();

            PlaceFirstWord(blocksDict, firstWord, placedWords);
            PlaceRestOfWords(blocksDict, words, placedWords);

            var unplacedWords = words.Where(w => !w.Placed).Select(w => w.WordString).ToList();

            return new Generation(blocksDict, placedWords, unplacedWords);
        }

        private static void PlaceFirstWord(Dictionary<Point, LetterBlock> blocksDict, Word word,
            ICollection<PlacedWord> placedWords)
        {
            var placement = new Placement(word, true);
            for (var i = 1; i <= word.WordLength; i++)
            {
                var coordinates = new Point(i, 1);
                var newBlock = new LetterBlock(word.CharArray[i - 1])
                {
                    Coordinates = coordinates
                };

                placement.LetterBlocks[i - 1] = newBlock;
            }

            PlaceWordOnBoard(blocksDict, placement, placedWords);
        }

        private void PlaceRestOfWords(Dictionary<Point, LetterBlock> blocksDict, IList<Word> words,
            ICollection<PlacedWord> placedWords)
        {
            while (true)
            {
                var placements = new List<Placement>();
                var unplacedWords = words.Where(word => !word.Placed).ToList();

                foreach (var word in unplacedWords)
                {
                    placements.AddRange(_placementFinder.FindWordPlacements(blocksDict, word));
                }

                if (!placements.Any())
                {
                    return;
                }

                var orderedPlacements = placements
                    .OrderByDescending(p => p.ExistingLettersUsed)
                    .ThenBy(p => p.Expansion.TotalX + p.Expansion.TotalY)
                    .ThenByDescending(p => p.Word.WordLength);

                var bestPlacements = orderedPlacements.Take(3).ToList();
                var randomPlacement = bestPlacements[new Random().Next(0, bestPlacements.Count-1)];
                PlaceWordOnBoard(blocksDict, randomPlacement, placedWords);
                unplacedWords.Remove(randomPlacement.Word);

                if (!unplacedWords.Any())
                {
                    return;
                }
            }
        }

        private static void PlaceWordOnBoard(Dictionary<Point, LetterBlock> blocksDict, Placement placement,
            ICollection<PlacedWord> placedWords)
        {
            if (placement.Expansion.TotalX != 0 || placement.Expansion.TotalY != 0)
                ExpandCrosswordSpace(blocksDict, placement, placedWords);

            foreach (var block in placement.LetterBlocks)
            {
                blocksDict.TryAdd(block.Coordinates, block);
            }

            placement.Word.Placed = true;
            placedWords.Add(new PlacedWord
            (
                placement.Word.Id,
                placement.Word.WordString,
                placement.Across,
                placement.FirstLetterCoordinates
                )
            );
        }

        private static void ExpandCrosswordSpace(Dictionary<Point, LetterBlock> blocksDict, Placement placement, IEnumerable<PlacedWord> placedWords)
        {
            var points = blocksDict.Keys
                .OrderByDescending(point => point.Y)
                .ThenByDescending(point => point.X);

            foreach (var point in points)
            {
                var block = blocksDict[point];
                block.Coordinates.Offset(placement.Expansion.Left, placement.Expansion.Up);
                blocksDict.Remove(point);
                blocksDict.Add(block.Coordinates, block);
            }

            if (placement.Expansion.Left > 0 || placement.Expansion.Up > 0)
            {
                foreach (var placedWord in placedWords)
                {
                    placedWord.ShiftFirstLetterCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                }
            }
        }
    }
}
