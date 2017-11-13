using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosswords
{
    public class Block
    {
        public BlockCoordinates Coordinates;
        public bool Free;
        public Letter letter;

        public Block(BlockCoordinates coordinates)
        {
            Coordinates = coordinates;
            Free = true;
        }

        public int ArrayX => Coordinates.X - 1;
        public int ArrayY => Coordinates.Y - 1;
    }
}
