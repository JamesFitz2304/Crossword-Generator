using System.Collections.Generic;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.Generator.Interfaces
{
    public interface IPlacementFinder
    {
        IEnumerable<Placement> FindWordPlacements(LetterBlock[,] blocks, Word word);
    }
}