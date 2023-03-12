using System.Diagnostics;
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

        private readonly List<Word> _twoUnplacedWords = new List<Word>()
        {
            new Word("WordOne"),
        };

        private readonly List<Word> _oneUnplacedWords = new List<Word>()
        {
            new Word("WordOne"),
            new Word("WordTwo")
        };


        private readonly List<Word> _5WordsAllPlaceable = new()
        {
            new Word("Goat"),
            new Word("Turtle"),
            new Word("Elephant"),
            new Word("Tiger"),
            new Word("Rabbit")
        };

        private readonly List<Word> _10WordsAllPlaceable = new()
        {
            new Word("Goat"),
            new Word("Turtle"),
            new Word("Elephant"),
            new Word("Tiger"),
            new Word("Rabbit"),
            new Word("Baby"),
            new Word("Yacht"),
            new Word("Toaster"),
            new Word("Railroad"),
            new Word("Digger")
        };

        private readonly List<Word> _15WordsAllPlaceable = new()
        {
            new Word("Goat"),
            new Word("Turtle"),
            new Word("Elephant"),
            new Word("Tiger"),
            new Word("Rabbit"),
            new Word("Baby"),
            new Word("Yacht"),
            new Word("Toaster"),
            new Word("Railroad"),
            new Word("Digger"),
            new Word("Richard"),
            new Word("Daniel"),
            new Word("Liam"),
            new Word("Michael"),
            new Word("Larry")
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
            _manager.GenerateCrosswords(_5WordsAllPlaceable, attempts);

            // Assert
            CollectionAssert.AreNotEqual(_5WordsAllPlaceable, list1);
            CollectionAssert.AreNotEqual(_5WordsAllPlaceable, list2);
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
            var result = realManager.GenerateCrosswords(_5WordsAllPlaceable, timeout: int.MaxValue);

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
            var words = new List<Word>()
            {
                new Word("Moose"),
                new Word("Goose"),
                new Word("Xyz")
            };

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
            double averageTime = 0;
            int timesToRepeat = 100;
            var wordSelector = 1;
            // Act
            for (var i = 0; i < timesToRepeat; i++)
            {
                List<Word> words;
                if (wordSelector == 1)
                {
                    words = _5WordsAllPlaceable;
                }
                else if (wordSelector % 2 == 0)
                {
                    words = _10WordsAllPlaceable;
                }
                else
                {
                    words = _15WordsAllPlaceable;
                }

                watch.Start();

                var result = realManager.GenerateCrosswords(words, attempts: attempts, timeout: int.MaxValue, cullIdenticals: false).ToList();
               
                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
                watch.Reset();
                var allWordsPlaced = result.Count(x => x.NumberOfUnplacedWords == 0);
                averageSuccess += (allWordsPlaced / result.Count()) * 100;
                wordSelector = wordSelector == 3 ? 1 : wordSelector + 1;
            }

            averageSuccess = (averageSuccess / timesToRepeat);
            averageTime = (averageTime / timesToRepeat);

            // Assert
            TestContext.WriteLine($"Success rate: {averageSuccess}%");
            TestContext.WriteLine($"Average time: {averageTime}ms");

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