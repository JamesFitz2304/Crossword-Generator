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

        [TestMethod] public void WhenOneWordCantBePlaced_UnplacedWordCountEquals1()
        {
            List<Word> Words = new List<Word>
            {
                new Word("CAT"),
                new Word("COW"),
                new Word("BEE")
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            Assert.IsFalse(generator.Generate());
            Assert.AreEqual(generator.UnplacedWords.Count, 1);
        }

        [TestMethod]
        public void AnimalWordsTest_NeverFourAdjacentOccupiedBlocks()
        {
            List<Word> Words = new List<Word>
            {
                new Word("Dog"),
                new Word("Cat"),
                new Word("Chicken"),
                new Word("Cow"),
                new Word("Monkey"),
                new Word("Salmon"),
                new Word("Goat"),
                new Word("Worm"),
                new Word("Wasp"),
                new Word("Bee"),
                new Word("Ostrich"),
                new Word("Parrot"),
                new Word("Frog"),
                new Word("Skunk"),
                new Word("Tiger"),
                new Word("Rabbit"),
                new Word("BAT")
            };
            CrosswordGenerator generator = new CrosswordGenerator(Words);
            generator.Generate();
            Block[,] blocks = generator.blocks;
            for (int y = 0; y < blocks.GetLength(1) - 1; y++)
            {
                for (int x = 0; x < blocks.GetLength(0) - 1; x++)
                {
                    if (blocks[x, y] == null) continue;
                    Assert.IsFalse(blocks[x + 1, y] != null && blocks[x, y + 1] != null &&
                                   blocks[x + 1, y + 1] != null);
                }
            }


        }

    }
}
