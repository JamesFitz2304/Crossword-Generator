using System.Collections.Generic;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.Generator.Utilities
{
    public class PlacementComparer : IComparer<Placement>
    {
        public int Compare(Placement a, Placement b)
        {
            if (a == null || b == null) return 0;
            return (a.Word.WordLength - a.NewLetters) - (b.Word.WordLength - b.NewLetters);
        }
    }
}