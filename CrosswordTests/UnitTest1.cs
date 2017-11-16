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
                new Word("WHEN"),
                new Word("YOU"),
                new Word("WERE"),
                new Word("YOUNG"),
                new Word("BLING"),
                new Word("MISTER"),
                new Word("BRIGHTSIDE"),
                new Word("SOMEBODY"),
                new Word("TOLD"),
                new Word("ME"),
                new Word("MISS"),
                new Word("ATOMIC"),
                new Word("BOMB"),
                new Word("MY"),
                new Word("LIST")
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
