using System;
using System.Drawing;
using System.Linq;

namespace CrosswordGenerator.Generator.Models
{
    public class PlacedWord
    {
        public PlacedWord(int id, bool across, Point coordinates)
        {
            Id = id;
            Across = across;
            FirstLetterCoordinates  = coordinates;
        }

        public int Id { get; }

        public bool Across { get; }

        public Point FirstLetterCoordinates { get; }



    }

}

