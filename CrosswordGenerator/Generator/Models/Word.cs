using System;
using System.Linq;

namespace CrosswordGenerator.Generator.Models
{
    public class Word
    {
        public Word(string word, int id = 0)
        {
            word = word.ToUpper();
            Letters = new Letter[word.Length];
            for (var i = 0; i < word.Length; i++)
            {
                Letters[i] = new Letter(word[i]);
            }

            Id = id;
        }

        public int Id { get; }

        public bool Placed { get; set; }

        public int WordLength => Letters.Length;

        public Letter[] Letters { get; }

        public string WordAsString => new string(Letters.Select(l => l.Character).ToArray());

        public string LetterCoordinates => string.Join(", ", Letters.Select(l => $"({l.Coordinates.X}, {l.Coordinates.Y})"));



    }

}

