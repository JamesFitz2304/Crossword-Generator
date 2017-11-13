using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Crosswords
{
    public class Letter
    {
        public BlockCoordinates Coordinates;
        public char Character;

        public Letter(char character)
        {
            Character = character;
        }
    }
}
