namespace CrosswordGenerator.Models
{
    public class WordCluePair
    {
        public WordCluePair(string word, string clue = "", int id = 0)
        {
            Word = word.ToUpper();
            Clue = clue;
            Id = id;
        }

        public int Id { get; set; }
        public string Word { get; set; }
        public string Clue { get; set; }
    }
}
