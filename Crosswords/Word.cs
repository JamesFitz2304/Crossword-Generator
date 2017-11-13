namespace Crosswords
{
    public class Word
    {
        private readonly Letter[] letters;
        private bool placed;

        public Word(string word)
        {
            letters = new Letter[word.Length];
            int i = 0;
            foreach(char character in word)
            {
                letters[i] = new Letter(character);
            }
        }

        public int WordLength
        {
            get => letters.Length;
        } 

        public Letter[] Letters
        {
            get => letters;
        }

        public bool Placed
        {
            get => placed;
            set { placed = value; }
        }

    }
}
