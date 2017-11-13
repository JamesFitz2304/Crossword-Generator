using System.Collections.Generic;
using Crosswords;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrosswordTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void WhenTwoWordsCanExistAlone_GenerationIsSuccessful()
        {
            List<Word> Words = new List<Word>
            {
                new Word("Bacon") ,
                new Word("Orange") 
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsTrue(generator.PlaceWords());
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
            Assert.IsFalse(generator.PlaceWords());
        }

    }
}
