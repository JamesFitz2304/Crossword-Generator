using System;
using System.Linq;

namespace CrosswordGenerator.Generator.Models
{
    public class Word
    {
        public Word(string word, int id = 0)
        {
            WordString = word.ToUpper();
            Id = id;
        }

        public string WordString { get; set; }

        public int Id { get; }

        public bool Placed { get; set; }

        public int WordLength => WordString.Length;

        public char[] WordAsCharArray => WordString.ToCharArray();

        //public string LetterCoordinates => string.Join(", ", Letters.Select(l => $"({l.Coordinates.X}, {l.Coordinates.Y})"));



    }

}

