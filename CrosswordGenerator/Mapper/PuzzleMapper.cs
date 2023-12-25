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
            var words = MapWords(generation.PlacedWords, wordCluePairs).ToList();
            return new Puzzle
            {
                Blocks = MapBlocks(generation.Blocks, words),
                Words = words
            };
        }

        public IEnumerable<Puzzle> Map(IEnumerable<Generation> generations, IEnumerable<WordCluePair> wordCluePairs)
        {
            return generations.Select(g =>
            {
                var words = MapWords(g.PlacedWords, wordCluePairs).ToList();
                return new Puzzle
                {
                    Blocks = MapBlocks(g.Blocks, words),
                    Words = words
                };
            });
        }

        private static IEnumerable<PuzzleBlock> MapBlocks(LetterBlock[,] blocks, IEnumerable<PuzzleWord> words)
        {
            var puzzleBlocks =  blocks.Cast<LetterBlock>().Where(b => b != null).Select(b => new PuzzleBlock
            {
                Character = b.Character,
                Coordinate = b.Coordinates.Coordinates,
                WordStarts = words.Where(w => w.Start == b.Coordinates.Coordinates)
            }).OrderBy(p => p.Coordinate.X).ThenBy(p => p.Coordinate.Y).ToList();


            // Assign order to words
            var blocksWithStarts = puzzleBlocks.Where(pb => pb.WordStarts.Any())
                .OrderBy(pb => pb.Coordinate.Y).ThenBy(pb => pb.Coordinate.X);

            var i = 1;
            foreach (var block in blocksWithStarts)
            {
                foreach (var word in block.WordStarts)
                {
                    word.Order = i;
                }
                i++;
            }
            return puzzleBlocks;
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

            return puzzleWords.OrderBy(p => p.Start.Y).ThenBy(p => p.Start.X).ToList();
        }

    }
}