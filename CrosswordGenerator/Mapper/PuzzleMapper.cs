using System.Collections.Generic;
using System.Linq;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator.Models;
using CrosswordGenerator.Models;
using CrosswordGenerator.Models.Puzzle;

namespace CrosswordGenerator.Mapper
{
    public class PuzzleMapper : IPuzzleMapper
    {
        public Puzzle Map(Generation generation, IEnumerable<WordCluePair> wordCluePairs)
        {
            return new Puzzle
            {
                Blocks = MapBlocks(generation.Blocks),
                Words = MapWords(generation.PlacedWords, wordCluePairs)
            };
        }

        public IEnumerable<Puzzle> Map(IEnumerable<Generation> generations, IEnumerable<WordCluePair> wordCluePairs)
        {
            return generations.Select(g => new Puzzle
            {
                Blocks = MapBlocks(g.Blocks),
                Words = MapWords(g.PlacedWords, wordCluePairs)
            });
        }

        private static IEnumerable<PuzzleBlock> MapBlocks(LetterBlock[,] blocks)
        {
            return blocks.Cast<LetterBlock>().Where(b => b != null).Select(b => new PuzzleBlock
            {
                Character = b.Character,
                Coordinate = b.Coordinates.Coordinates
            }).OrderBy(p => p.Coordinate.X).ThenBy(p => p.Coordinate.Y);
        }

        private static IEnumerable<PuzzleWord> MapWords(IEnumerable<PlacedWord> words, IEnumerable<WordCluePair> wordCluePairs)
        {
            var puzzleWords = words.Select(word => new PuzzleWord
            {
                Word = word.Word,
                Across = word.Across,
                Start = word.Start,
                Clue = wordCluePairs.First(wcp => wcp.Id == word.Id).Clue
            }).ToList();

            puzzleWords = puzzleWords.OrderBy(p => p.Start.X).ThenBy(p => p.Start.Y).ToList();
            var i = 1;
            puzzleWords.ForEach(p => p.Order = i++);
            return puzzleWords;
        }

    }
}