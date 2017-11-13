using System;
using System.Collections.Generic;

namespace Crosswords
{
    public class PlacementComparer : IComparer<Placement>
    {

        private int XSize;
        private int YSize;

        public PlacementComparer(int xsize, int ysize)
        {
            XSize = xsize;
            YSize = ysize;
        }

        public int Compare(Placement a, Placement b)
        {
            return Math.Abs((XSize + b.XExpansion) - (YSize + b.YExpansion)) - Math.Abs((XSize + a.XExpansion) - (YSize + a.YExpansion));

        }
    }
}