using System.Linq;

namespace CrosswordGenerator.Generator.Models
{
    public class Word
    {
        public Word(string word, int id)
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

        public int WordLength => Letters.Length;

        public Letter[] Letters { get; }

        public bool Placed { get; set; } = false;

        public string WordAsString => new string(Letters.Select(x => x.Character).ToArray());

    }

}

