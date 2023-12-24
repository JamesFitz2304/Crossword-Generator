using CrosswordAPI.Models;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Models;

namespace CrosswordAPI.Mapper
{
    public interface IPuzzleMapper
    {
        IEnumerable<Puzzle> Map(IEnumerable<Generation> generations, IEnumerable<WordCluePair> wordCluePairs);
    }
}