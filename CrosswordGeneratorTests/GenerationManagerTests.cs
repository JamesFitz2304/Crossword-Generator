using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;
using Moq;
using System.Diagnostics;

namespace CrosswordGeneratorTests
{
    [TestFixture]
    public class GenerationManagerTests
    {
        private GenerationManager _manager;

        private Mock<IGenerator> _generatorMock;

        private readonly List<Word> _twoUnplacedWords = new()
        {
            new Word("Goat"),
            new Word("Turtle"),
            new Word("Yyy"),
            new Word("Zzz"),
        };

        private readonly List<Word> _oneUnplacedWords = new()
        {
            new Word("Goat"),
            new Word("Turtle"),
            new Word("Zzz"),
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

        private readonly List<Word> _15Words3Unplaceable = new()
        {
            new Word("Goat"),
            new Word("Turtle"),
            new Word("Elephant"),
            new Word("Tiger"),
            new Word("Rabbit"),
            new Word("Houmous"),
            new Word("Toaster"),
            new Word("Railroad"),
            new Word("Digger"),
            new Word("Richard"),
            new Word("Daniel"),
            new Word("Liam"),
            new Word("Xxx"),
            new Word("Yyy"),
            new Word("Zzz"),
        };



        private readonly Block[,] _blocks1 = new Block[,]
        {
            {
                new(new Letter('A')), new(new Letter('B'))
            },
            {
                null!, null!
            },
            {
                new(new Letter('C')), new(new Letter('D'))
            }

        };

        private readonly Block[,] _blocks2 = new Block[,]
        {
            {
                new(new Letter('E')), new(new Letter('F'))
            },
            {
                null!, null!
            },
            {
                new(new Letter('G')), new(new Letter('H'))
            }

        };

        [SetUp]
        public void Setup()
        {
            _generatorMock = new Mock<IGenerator>();

            _manager = new GenerationManager(_generatorMock.Object);
        }

        [Test]
        public void GenerateCrosswords_ThrowsExceptionIfWordsContainInvalidCharacters()
        {

            var invalidCharacters = new List<Word>()
        {
            new("Sausage"),
            new("B4con"),
            new("Eggs")
        };

            var exception = Assert.Throws<FormatException>(() => _manager.GenerateCrosswords(invalidCharacters));
            Assert.AreEqual("Word list contained invalid characters", exception?.Message);
        }

        [Test]
        public void GenerateCrosswords_ThrowsExceptionIfLessThanTwoWords()
        {
            var oneWord = new List<Word>()
            {
                new("Sausage")
            };

            var exception = Assert.Throws<FormatException>(() => _manager.GenerateCrosswords(oneWord));
            Assert.AreEqual("Word list must be greater than 1", exception?.Message);
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
            _manager.GenerateCrosswords(_5WordsAllPlaceable, attempts);

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
            _manager.GenerateCrosswords(_5WordsAllPlaceable, attempts, timeout);

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
            var result = _manager.GenerateCrosswords(_5WordsAllPlaceable, 2, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(2, generations.Count);
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
            var result = _manager.GenerateCrosswords(_5WordsAllPlaceable, 2, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(1, generations.Count);
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
            var result = _manager.GenerateCrosswords(_5WordsAllPlaceable, 4, int.MaxValue).ToList();

            // Assert
            var generations = result;
            Assert.AreEqual(2, generations.Count);
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
                new("Moose"),
                new("Goose"),
                new("Xyz")
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
            var successfulGeneration = new Generation(_blocks1, new List<Word>());

            _generatorMock.Setup(g => g.Generate(It.IsAny<IList<Word>>()))
                .Returns(successfulGeneration);

            // Act
            _manager.GenerateCrosswords(_5WordsAllPlaceable);

            // Assert
            _generatorMock.Verify(g => g.Generate(It.IsAny<List<Word>>()), Times.Exactly(3));
        }

        [Test]
        public void EfficiencyTest()
        {
            var watch = new Stopwatch();

            // Arrange
            var realGenerator = new Generator();
            var realManager = new GenerationManager(realGenerator);
            double averageSuccess = 0;
            double averageTime = 0;
            const int timesToRepeat = 10000;
            var wordSelector = 1;
            // Act
            for (var i = 0; i < timesToRepeat; i++)
            {
                var words = wordSelector switch
                {
                    1 => _5WordsAllPlaceable,
                    2 => _10WordsAllPlaceable,
                    3 => _15WordsAllPlaceable,
                    _ => _15Words3Unplaceable,
                };

                watch.Start();

                var result = realManager.GenerateCrosswords(words, timeout: int.MaxValue, cullIdenticals: false).ToList();

                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
                watch.Reset();
                // If all words placeable, success is all words placed
                // If 3 words unplaceable, success is all but 3 words placed
                var success = wordSelector == 4
                    ? Convert.ToInt32(result.Any(x => x.NumberOfUnplacedWords == 3))
                    : Convert.ToInt32(result.Any(x => x.NumberOfUnplacedWords == 0));
                averageSuccess += success;
                wordSelector = wordSelector == 4 ? 1 : wordSelector + 1;
            }

            averageSuccess = (averageSuccess / timesToRepeat) * 100;
            averageTime /= timesToRepeat;

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