//using CrosswordGenerator;

//namespace CrosswordGeneratorTests
//{
//    [TestFixture]
//    public class GeneratorTests
//    {
//        [Test]
//        public void WhenTwoWordsCanExist_GenerationIsSuccessful()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("Hello") ,
//                new Word("Heat") 
//            };
//            Generator generator = new Generator(Words);
//            //Assert.IsTrue(generator.Generate());
//        }

//        [Test]
//        public void WhenThreeWordsCanExist_GenerationIsSuccessful()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("Hello"),
//                new Word("Heat"),
//                new Word("Toad")

//            };
//            Generator generator = new Generator(Words);
//            //Assert.IsTrue(generator.Generate());
//        }



//        [Test]
//        public void WhenTwoWordsCannotExistAlone_GenerationIsUnsuccessful()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("Beer") ,
//                new Word("Food")
//            };
//            Generator generator = new Generator(Words);
//            //Assert.IsFalse(generator.Generate());
//        }

//        [Test]
//        public void WhenFirstTwoWordsCannotExistAlone_ThirdWordAllowsThemToExist_GenerationIsSuccessful()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("CAT"),
//                new Word("DOG"),
//                new Word("TOAD")
//            };
//            Generator generator = new Generator(Words);
//            //Assert.IsTrue(generator.Generate());
//        }

//        [Test]
//        public void WhenOnlyOneWord_GenerationIsUnSuccessful()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("CAT")
//            };
//            Generator generator = new Generator(Words);
//            //Assert.IsFalse(generator.Generate());
//        }

//        [Test]
//        public void WhenAWordContainsANonAlphabeticCharacter_ThrowError()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("CAT"),
//                new Word("DOG"),
//                new Word("SNA.KE/")
//            };
//            Generator generator;
//            Assert.Throws<FormatException>(() => generator = new Generator(Words));
//        }

//        [Test] public void WhenOneWordCantBePlaced_UnplacedWordCountEquals1()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("CAT"),
//                new Word("COW"),
//                new Word("BEE")
//            };
//            Generator generator = new Generator(Words);
//            //Assert.IsFalse(generator.Generate());
//            Assert.AreEqual( 1, generator.UnplacedWords.Count);
//        }

//        [Test]
//        public void AnimalWordsTest_NeverFourAdjacentOccupiedBlocks()
//        {
//            List<Word> Words = new List<Word>
//            {
//                new Word("Dog"),
//                new Word("Cat"),
//                new Word("Chicken"),
//                new Word("Cow"),
//                new Word("Monkey"),
//                new Word("Salmon"),
//                new Word("Goat"),
//                new Word("Worm"),
//                new Word("Wasp"),
//                new Word("Bee"),
//                new Word("Ostrich"),
//                new Word("Parrot"),
//                new Word("Frog"),
//                new Word("Skunk"),
//                new Word("Tiger"),
//                new Word("Rabbit"),
//                new Word("Bat"),
//                new Word("Antelope"),
//                new Word("Tortoise")
//            };
//            Generator generator = new Generator(Words);
//            //generator.Generate();
//            Block[,] blocks = generator.blocks;
//            for (int y = 0; y < blocks.GetLength(1) - 1; y++)
//            {
//                for (int x = 0; x < blocks.GetLength(0) - 1; x++)
//                {
//                    if (blocks[x, y] == null) continue;
//                    Assert.IsFalse(blocks[x + 1, y] != null && blocks[x, y + 1] != null &&
//                                   blocks[x + 1, y + 1] != null);
//                }
//            }


//        }

//    }
//}
