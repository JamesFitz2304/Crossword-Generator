using System.Collections.Generic;
using System.Drawing;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.Generator.Interfaces
{
    public interface IPlacementFinder
    {
        IEnumerable<Placement> FindWordPlacements(Dictionary<Point, LetterBlock> blocksDict, Word word);
    }
}