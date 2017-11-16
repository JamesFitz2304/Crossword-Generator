using System.Collections.Generic;
using Crosswords;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrosswordTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void WhenTwoWordsCanExist_GenerationIsSuccessful()
        {
            List<Word> Words = new List<Word>
            {
                new Word("Bacon") ,
                new Word("Orange") 
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsTrue(generator.Generate());
        }

        [TestMethod]
        public void WhenThreeWordsCanExist_GenerationIsSuccessful()
        {
            List<Word> Words = new List<Word>
            {
                new Word("Toilet"),
                new Word("Lemon"),
                new Word("Bee"),
                new Word("Beans"),
                new Word("Sausage"),
                new Word("Meateater"),
                new Word("Saudi"),
                new Word("Poo"),
                new Word("Anus"),
                new Word("Sod")
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsTrue(generator.Generate());
        }



        [TestMethod]
        public void WhenTwoWordsCannotExistAlone_GenerationIsUnsuccessful()
        {
            List<Word> Words = new List<Word>
            {
                new Word("Beer") ,
                new Word("Food")
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsFalse(generator.Generate());
        }

    }
}
