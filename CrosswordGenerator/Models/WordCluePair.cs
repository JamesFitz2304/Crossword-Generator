namespace CrosswordGenerator.Models
{
    public class WordCluePair
    {
        public WordCluePair(string word, int id = 0)
        {
            Word = word.ToUpper();
            Id = id;
        }

        public int Id { get; set; }
        public string Word { get; set; }
        public string Clue { get; set; }
    }
}
