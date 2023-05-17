using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator.Utilities;
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
            foreach (var word in words)
            {
                word.Placed = false;
            }
            var firstWord = words.OrderByDescending(w => w.WordLength).First();
            var blocks = new LetterBlock[1, firstWord.WordLength];
            var placedWords = new List<PlacedWord>();
            PlaceFirstWord(ref blocks, firstWord, placedWords);
            PlaceRestOfWords(ref blocks, words, placedWords);
            var unplacedWords = words.Where(w => !w.Placed).Select(w => w.WordString).ToList();
            return new Generation(blocks, words, placedWords, unplacedWords);
        }

        private static void PlaceFirstWord(ref LetterBlock[,] blocks, Word word, IList<PlacedWord> placedWords)
        {
            var placement = new Placement(word, true);
            for (var i = 1; i <= word.WordLength; i++)
            {
                var coordinates = new BlockCoordinates(i, 1);
                var newBlock = new LetterBlock(word.CharArray[i - 1])
                {
                    Coordinates = coordinates
                };

                placement.LetterBlocks[i - 1] = newBlock;
            }

            PlaceWordOnBoard(ref blocks, placement, placedWords);
        }

        private void PlaceRestOfWords(ref LetterBlock[,] blocks, IList<Word> words, List<PlacedWord> placedWords)
        {
            while (true)
            {
                var placements = new List<Placement>();
                var unplacedWords = words.Where(word => !word.Placed).ToList();

                foreach (var word in unplacedWords)
                {
                    placements.AddRange(_placementFinder.FindWordPlacements(blocks, word));
                }

                if (!placements.Any())
                {
                    // Return if no more _words can be placed
                    return;
                }

                var orderedPlacements = placements
                    .OrderByDescending(p => p.ExistingLettersUsed)
                    .ThenBy(p => p.Expansion.TotalX + p.Expansion.TotalY)
                    .ThenByDescending(p => p.Word.WordLength);

                var bestPlacements = orderedPlacements.Take(3).ToList();
                var randomPlacement = bestPlacements[new Random().Next(0, bestPlacements.Count-1)];
                PlaceWordOnBoard(ref blocks, randomPlacement, placedWords);
                unplacedWords.Remove(randomPlacement.Word);

                if (!unplacedWords.Any())
                {
                    return;
                }
            }
        }

        private static void PlaceWordOnBoard(ref LetterBlock[,] blocks, Placement placement,
            IList<PlacedWord> placedWords)
        {
            if (placement.Expansion.TotalX != 0 || placement.Expansion.TotalY != 0)
                ExpandCrosswordSpace(ref blocks, placement, placedWords);

            //SetLetterToPlacementCoordinates(placement);
            foreach (var block in placement.LetterBlocks)
            {
                blocks[block.Coordinates.ArrayCoordinates.Y, block.Coordinates.ArrayCoordinates.X] = block;
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

        private static void ExpandCrosswordSpace(ref LetterBlock[,] blocks, Placement placement, IList<PlacedWord> placedWords)
        {
            var newBlocks = new LetterBlock[blocks.GetLength(0) + Math.Abs(placement.Expansion.TotalY), blocks.GetLength(1) + Math.Abs(placement.Expansion.TotalX)];

            for (var y = 0; y < blocks.GetLength(0); y++)
            {
                for (var x = 0; x < blocks.GetLength(1); x++)
                {
                    blocks[y, x]?.Coordinates.ShiftCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                    newBlocks[y + placement.Expansion.Up, x + placement.Expansion.Left] = blocks[y, x];
                }
            }

            if (placement.Expansion.Left > 0 || placement.Expansion.Up > 0)
            {
                foreach (var placedWord in placedWords)
                {
                    placedWord.ShiftFirstLetterCoordinates(placement.Expansion.Left, placement.Expansion.Up);
                }
            }

            blocks = newBlocks;
        }
    }
}
