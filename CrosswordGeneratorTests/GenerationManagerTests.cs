using System.Collections;
using System.Diagnostics;
using CrosswordGenerator;
using CrosswordGenerator.Interfaces;
using Moq;
using NUnit.Framework.Internal;

namespace CrosswordGeneratorTests
{
    [TestFixture]
    public class GenerationManagerTests
    {
        private GenerationManager _manager;

        private Mock<IGenerator> _generatorMock;

        private readonly List<Word> _twoUnplacedWords = new List<Word>()
        {
            new Word("WordOne"),
        };

        private readonly List<Word> _oneUnplacedWords = new List<Word>()
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
            var generationLess = new Generation(_blocks1, _oneUnplacedWords);

            var generationMore = new Generation(_blocks1, _twoUnplacedWords);

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
            var generationOne = new Generation(_blocks1, _oneUnplacedWords);

            var generationTwo = new Generation(_blocks2, _oneUnplacedWords);

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
            var generationMore = new Generation(_blocks1, _twoUnplacedWords);

            var generationLess = new Generation(_blocks1, _oneUnplacedWords);

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
        public void GenerateCrosswords_RemovesDuplicateGenerations()
        {
            // Arrange
            var generationOne = new Generation(_blocks1, _twoUnplacedWords);
            var generationTwo = new Generation(_blocks2, _twoUnplacedWords);
            var generationThree = new Generation(_blocks1, _twoUnplacedWords);
            var generationFour = new Generation(_blocks2, _twoUnplacedWords);

            _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationOne)
                .Returns(generationTwo).Returns(generationThree).Returns(generationFour);

            // Act
            var result = _manager.GenerateCrosswords(new List<Word>(), 4, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(2, generations.Count());
            Assert.AreEqual(generationOne, generations[0]);
            Assert.AreEqual(generationTwo, generations[1]);


        }

        [Test]
        [Repeat(100)]
        public void GenerateCrosswords_WhenGivenAListOfWordsThatCanAllBePlaced_ShouldGenerateCrosswordWithAllWordsPlaced()
        {
            // Arrange
            var realGenerator = new Generator();
            var realManager = new GenerationManager(realGenerator);

            // Act
            var result = realManager.GenerateCrosswords(_wordsAllPlaceable, timeout: int.MaxValue);

            // Assert
            Assert.AreEqual(0, result.First().NumberOfUnplacedWords);
        }

        [Test]
        [Repeat(1000)]
        public void GenerateCrosswords_WhenOneWordCanNotBePlaced_ShouldGenerateCrosswordWithOneWordUnplaced()
        {
            // Arrange
            var realGenerator = new Generator();
            var realManager = new GenerationManager(realGenerator);
            var words = _wordsAllPlaceable.ToList();
            words.Add(new Word("XYZ"));

            // Act
            var result = realManager.GenerateCrosswords(words, timeout: int.MaxValue);

            // Assert
            var unplacedWords = result.First().NumberOfUnplacedWords;
            Assert.AreEqual(1, unplacedWords);
        }

        [Test]
        public void GenerateCrosswords_WhenTenCompleteCrosswordsGenerated_ShouldStopGeneratingAndReturn()
        {
            // Arrange
            var words = new List<Word>();
            var successfulGeneration = new Generation(_blocks1, new List<Word>());

            _generatorMock.Setup(g => g.Generate(It.IsAny<IList<Word>>()))
                .Returns(successfulGeneration);

            // Act
            _manager.GenerateCrosswords(words);

            // Assert
            _generatorMock.Verify(g => g.Generate(It.IsAny<List<Word>>()), Times.Exactly(10));
        }

        [Test]
        public void EfficencyTest()
        {
            var watch = new Stopwatch();

            // Arrange
            var realGenerator = new Generator();
            var realManager = new GenerationManager(realGenerator);
            const int attempts = 100;
            double averageSuccess = 0;

            watch.Start();
            // Act
            for (var i = 0; i < 1000; i++)
            {
                var result = realManager.GenerateCrosswords(_wordsAllPlaceable, attempts: attempts, timeout: int.MaxValue, cullIdenticals: false);
                
                averageSuccess += (result.Count() / (double)attempts) * 100;
            }

            averageSuccess = (averageSuccess / 1000);

            watch.Stop();
            // Assert
            TestContext.WriteLine($"Success rate: {averageSuccess}%");
            TestContext.WriteLine($"Time taken: {watch.Elapsed.TotalSeconds}s");

            Assert.True(true);
        }

        private Generation CreateDefaultGeneration()
        {
            return new Generation(_blocks1, _twoUnplacedWords);

        }

        private void SetUpDefaultGeneration()
        {
            _generatorMock.Setup(g => g.Generate(It.IsAny<List<Word>>())).Returns(CreateDefaultGeneration);
        }

    }
}