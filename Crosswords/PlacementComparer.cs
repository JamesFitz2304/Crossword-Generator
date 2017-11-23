using System;
using System.Collections.Generic;

namespace Crosswords
{
    public class PlacementComparer : IComparer<Placement>
    {
        public int Compare(Placement a, Placement b)
        {
            int diff = a.NewLetters - b.NewLetters;

            return diff == 0
                ? (a.Expansion.TotalX + a.Expansion.TotalY) - (b.Expansion.TotalX + b.Expansion.TotalY)
                : diff;
        }
    }
}