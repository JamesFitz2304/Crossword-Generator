using System.Collections.Generic;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.Generator.Utilities
{
    public class PlacementComparer : IComparer<Placement>
    {
        public int Compare(Placement a, Placement b)
        {
            if (a.Word.WordLength - (a.Expansion.TotalY + a.Expansion.TotalX) < b.Word.WordLength - (b.Expansion.TotalY + b.Expansion.TotalX))
            {
                return 1;
            }

            if (a.Word.WordLength - (a.Expansion.TotalY + a.Expansion.TotalX) > b.Word.WordLength - (b.Expansion.TotalY + b.Expansion.TotalX))
            {
                return -1;
            }

            return 0;
        }
    }
}