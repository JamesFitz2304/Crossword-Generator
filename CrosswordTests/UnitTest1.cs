using System;
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
                new Word("Hello") ,
                new Word("Heat") 
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsTrue(generator.Generate());
        }

        [TestMethod]
        public void WhenThreeWordsCanExist_GenerationIsSuccessful()
        {
            List<Word> Words = new List<Word>
            {
                new Word("Hello"),
                new Word("Heat"),
                new Word("Toad")

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

        [TestMethod]
        public void WhenFirstTwoWordsCannotExistAlone_ThirdWordAllowsThemToExist_GenerationIsSuccessful()
        {
            List<Word> Words = new List<Word>
            {
                new Word("CAT"),
                new Word("DOG"),
                new Word("TOAD")
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsTrue(generator.Generate());
        }

        [TestMethod]
        public void WhenOnlyOneWord_GenerationIsUnSuccessful()
        {
            List<Word> Words = new List<Word>
            {
                new Word("CAT")
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsFalse(generator.Generate());
        }

        [TestMethod]
        public void WhenAWordContainsANonAlphabeticCharacter_ThrowError()
        {
            List<Word> Words = new List<Word>
            {
                new Word("CAT"),
                new Word("DOG"),
                new Word("SNA.KE/")
            };
            CrosswordGenerator generator;
            Assert.ThrowsException<FormatException>(() => generator = new CrosswordGenerator(Words));
        }

    }
}
