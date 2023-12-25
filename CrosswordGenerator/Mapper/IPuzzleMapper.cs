using System.Collections.Generic;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Models;
using CrosswordGenerator.Models.Puzzle;

namespace CrosswordGenerator.Mapper
{
    public interface IPuzzleMapper
    {
        Puzzle Map(Generation generation, IEnumerable<WordCluePair> wordCluePairs);
        IEnumerable<Puzzle> Map(IEnumerable<Generation> generations, IEnumerable<WordCluePair> wordCluePairs);
    }
}