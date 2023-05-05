namespace CrosswordGenerator.Models
{
    public class WordCluePair
    {
        public WordCluePair(string word, int id)
        {
            Word = word;
            Id = id;
        }

        public int Id { get; set; }
        public string Word { get; set; }
        public string Clue { get; set; }
    }
}
