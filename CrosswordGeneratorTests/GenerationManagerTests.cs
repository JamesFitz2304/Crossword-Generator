using System.Collections;
using CrosswordGenerator;
using CrosswordGenerator.Interfaces;
using Moq;

namespace CrosswordGeneratorTests
{
    [TestFixture]
    public class GenerationManagerTests
    {
        private GenerationManager _manager;

        private Mock<IGenerator> _generatorMock;

        private readonly List<Word> _unplacedWordsOne = new List<Word>()
        {
            new Word("WordOne"),
        };

        private readonly List<Word> _unplacedWordsTwo = new List<Word>()
        {
            new Word("WordOne"),
            new Word("WordTwo")
        };


        private readonly List<Word> _wordsAllPlaceable = new()
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
            new Word("Bat"),
            new Word("Antelope"),
            new Word("Tortoise")
        };

        private readonly Block[,] _blocks1 = new Block[,]
        {
            {
                new Block(new Letter('A')), new Block(new Letter('B'))
            },
            {
                null!, null!
            },
            {
                new Block(new Letter('C')), new Block(new Letter('D'))
            }

        };

        private readonly Block[,] _blocks2 = new Block[,]
        {
            {
                new Block(new Letter('E')), new Block(new Letter('F'))
            },
            {
                null!, null!
            },
            {
                new Block(new Letter('G')), new Block(new Letter('H'))
            }

        };

        [SetUp]
        public void Setup()
        {
            _generatorMock = new Mock<IGenerator>();

            _manager = new GenerationManager(_generatorMock.Object);
        }

        [Test]
        public void GenerateCrosswords_ShufflesWordsWithEachGeneration()
        {
            // Arrange
            var sequence = new MockSequence();
            const int attempts = 2;
            IList<Word> list1 = new List<Word>();
            IList<Word> list2 = new List<Word>();

            _generatorMock.InSequence(sequence).Setup(g => g.Generate(It.IsAny<IList<Word>>())).Returns(CreateDefaultGeneration)
                .Callback<IList<Word>>(l => list1 = l);
            _generatorMock.InSequence(sequence).Setup(g => g.Generate(It.IsAny<IList<Word>>())).Returns(CreateDefaultGeneration)
                .Callback<IList<Word>>(l => list2 = l);

            // Act
            _manager.GenerateCrosswords(_wordsAllPlaceable, attempts);

            // Assert
            CollectionAssert.AreNotEqual(_wordsAllPlaceable, list1);
            CollectionAssert.AreNotEqual(_wordsAllPlaceable, list2);
            CollectionAssert.AreNotEqual(list1, list2);
        }

        [Test]
        public void GenerateCrosswords_GeneratesForSetNumberOfAttempts()
        {

            // Arrange
            const int attempts = 10;
            SetUpDefaultGeneration();

            // Act
            _manager.GenerateCrosswords(new List<Word>(), attempts);

            // Assert
            _generatorMock.Verify(g => g.Generate(It.IsAny<List<Word>>()), Times.Exactly(attempts));
        }

        [Test]
        public void GenerateCrosswords_ReturnsAfterTimeout()
        {
            // Arrange
            const int timeout = 1000;
            const int attempts = 10;
            _generatorMock.Setup(g => g.Generate(It.IsAny<List<Word>>()))
                .Returns(CreateDefaultGeneration)
                .Callback(() => Thread.Sleep(200));

            // Act
            _manager.GenerateCrosswords(new List<Word>(), attempts, timeout);

            // Assert
            _generatorMock.Verify(g => g.Generate(It.IsAny<List<Word>>()), Times.AtMost(9));
        }

        [Test]
        public void GenerateCrosswords_WhenNewGenerationPlacedMoreWords_ClearListAndAddNewGeneration()
        {
            // Arrange
            var generationLess = new Generation()
            {
                UnplacedWords = _unplacedWordsTwo
            };

            var generationMore = new Generation()
            {
                UnplacedWords = _unplacedWordsOne
            };

            _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationLess)
                .Returns(generationMore);

            // Act
            var result = _manager.GenerateCrosswords(new List<Word>(), 2, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(1, generations.Count());
            Assert.AreEqual(generationMore, generations.First());
        }

        [Test]
        public void GenerateCrosswords_WhenNewGenerationPlacedSameAmountOfWords_AddNewGeneration()
        {
            // Arrange
            var generationOne = new Generation()
            {
                UnplacedWords = _unplacedWordsTwo,
                blocks = _blocks1
            };

            var generationTwo = new Generation()
            {
                UnplacedWords = _unplacedWordsTwo,
                blocks = _blocks2

            };

            _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationOne)
                .Returns(generationTwo);

            // Act
            var result = _manager.GenerateCrosswords(new List<Word>(), 2, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(2, generations.Count());
            Assert.AreEqual(generationOne, generations[0]);
            Assert.AreEqual(generationTwo, generations[1]);

        }

        [Test]
        public void GenerateCrosswords_WhenNewGenerationPlacedLessWords_DoNotAddNewGeneration()
        {
            // Arrange
            var generationMore = new Generation()
            {
                UnplacedWords = _unplacedWordsOne
            };

            var generationLess = new Generation()
            {
                UnplacedWords = _unplacedWordsTwo
            };

            _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationMore)
                .Returns(generationLess);

            // Act
            var result = _manager.GenerateCrosswords(new List<Word>(), 2, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(1, generations.Count());
            Assert.AreEqual(generationMore, generations.First());

        }

        [Test]
        public void GenerateCrosswords_WhenBlocksAreIdentical_DoNotAddNewGeneration()
        {
            // Arrange
            var generationOne = new Generation()
            {
                UnplacedWords = _unplacedWordsOne,
                blocks = _blocks1
            };

            var generationTwo = new Generation()
            {
                UnplacedWords = _unplacedWordsOne,
                blocks = _blocks1
            };

            _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationOne)
                .Returns(generationTwo);

            // Act
            var result = _manager.GenerateCrosswords(new List<Word>(), 2, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(1, generations.Count());
            Assert.AreEqual(generationOne, generations.First());

        }

        [Test]
        public void GenerateCrosswords_WhenBlocksAreNotIdentical_AddNewGeneration()
        {
            // Arrange
            var generationOne = new Generation()
            {
                UnplacedWords = _unplacedWordsOne,
                blocks = _blocks1
            };

            var generationTwo = new Generation()
            {
                UnplacedWords = _unplacedWordsOne,
                blocks = _blocks2
            };

            _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationOne)
                .Returns(generationTwo);

            // Act
            var result = _manager.GenerateCrosswords(new List<Word>(), 2, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(2, generations.Count());
            Assert.AreEqual(generationOne, generations[0]);
            Assert.AreEqual(generationTwo, generations[1]);
        }

        private Generation CreateDefaultGeneration()
        {
            return new Generation()
            {
                blocks = _blocks1,
                UnplacedWords = _unplacedWordsOne
            };

        }

        private void SetUpDefaultGeneration()
        {
            _generatorMock.Setup(g => g.Generate(It.IsAny<List<Word>>())).Returns(CreateDefaultGeneration);
        }

    }
}