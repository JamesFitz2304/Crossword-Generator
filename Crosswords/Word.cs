namespace Crosswords
{
    public class Word
    {
        private readonly Letter[] letters;
        private bool placed;

        public Word(string word)
        {
            word = word.ToUpper();
            letters = new Letter[word.Length];
            for(int i = 0; i < word.Length; i++)
            {
                letters[i] = new Letter(word[i]);
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

        public string WordAsString
        {


            get
            {
                string word = "";
                foreach (Letter letter in letters)
                {
                    word += letter.Character;
                }
                return word;
            }
        }

    }
}
