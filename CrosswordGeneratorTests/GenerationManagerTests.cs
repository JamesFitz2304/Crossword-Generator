using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;
using Moq;
using System.Diagnostics;
using CrosswordGenerator.Generator;
using CrosswordGenerator.Models;

namespace CrosswordGeneratorTests
{
    [TestFixture]
    public class GenerationManagerTests
    {
        private GenerationManager _manager;

        private Mock<IGenerator> _generatorMock;

        private const int Best5Size = 56;
        private const int Best10Size = 120;
        private const int Best15Size = 168;
        private const int Best15UnSize = 143;

        private readonly List<WordCluePair> _twoUnplacedWords = new()
        {
            new WordCluePair("Goat"),
            new WordCluePair("Turtle"),
            new WordCluePair("Yyy"),
            new WordCluePair("Zzz"),
        };

        private readonly List<WordCluePair> _oneUnplacedWords = new()
        {
            new WordCluePair("Goat"),
            new WordCluePair("Turtle"),
            new WordCluePair("Zzz"),
        };


        private readonly List<WordCluePair> _5WordsAllPlaceable = new()
        {
            new WordCluePair("Goat"),
            new WordCluePair("Turtle"),
            new WordCluePair("Elephant"),
            new WordCluePair("Tiger"),
            new WordCluePair("Rabbit")
        };

        private readonly List<WordCluePair> _10WordsAllPlaceable = new()
        {
            new WordCluePair("Goat"),
            new WordCluePair("Turtle"),
            new WordCluePair("Elephant"),
            new WordCluePair("Tiger"),
            new WordCluePair("Rabbit"),
            new WordCluePair("Baby"),
            new WordCluePair("Yacht"),
            new WordCluePair("Toaster"),
            new WordCluePair("Railroad"),
            new WordCluePair("Digger")
        };

        private readonly List<WordCluePair> _15WordsAllPlaceable = new()
        {
            new WordCluePair("Goat"),
            new WordCluePair("Turtle"),
            new WordCluePair("Elephant"),
            new WordCluePair("Tiger"),
            new WordCluePair("Rabbit"),
            new WordCluePair("Baby"),
            new WordCluePair("Yacht"),
            new WordCluePair("Toaster"),
            new WordCluePair("Railroad"),
            new WordCluePair("Digger"),
            new WordCluePair("Richard"),
            new WordCluePair("Daniel"),
            new WordCluePair("Liam"),
            new WordCluePair("Michael"),
            new WordCluePair("Larry")
        };

        private readonly List<WordCluePair> _15Words3Unplaceable = new()
        {
            new WordCluePair("Goat"),
            new WordCluePair("Turtle"),
            new WordCluePair("Elephant"),
            new WordCluePair("Tiger"),
            new WordCluePair("Rabbit"),
            new WordCluePair("Houmous"),
            new WordCluePair("Toaster"),
            new WordCluePair("Railroad"),
            new WordCluePair("Digger"),
            new WordCluePair("Richard"),
            new WordCluePair("Daniel"),
            new WordCluePair("Liam"),
            new WordCluePair("Xxx"),
            new WordCluePair("Yyy"),
            new WordCluePair("Zzz"),
        };



        private readonly LetterBlock[,] _blocks1 = new LetterBlock[,]
        {
            {
              new LetterBlock('A'), new LetterBlock('B')
            },
            {
                null!, null!
            },
            {
               new LetterBlock('C'),new LetterBlock('D')
            }

        };

        private readonly LetterBlock[,] _blocks2 = new LetterBlock[,]
        {
            {
                new LetterBlock('E'), new LetterBlock('F')
            },
            {
                null!, null!
            },
            {
                new LetterBlock('G'), new LetterBlock('H')
            }

        };

        private PlacementFinder _realPlacementFinder;
        private Generator _realGenerator;
        private GenerationManager _realManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _realPlacementFinder = new PlacementFinder();
            _realGenerator = new Generator(_realPlacementFinder);
            _realManager = new GenerationManager(_realGenerator);
        }

        [SetUp]
        public void Setup()
        {
            _generatorMock = new Mock<IGenerator>();

            _manager = new GenerationManager(_generatorMock.Object);
        }

        [Test]
        public void GenerateCrosswords_ThrowsExceptionIfWordsContainInvalidCharacters()
        {

            var invalidCharacters = new List<WordCluePair>()
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
            var oneWord = new List<WordCluePair>()
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
                .Callback<IList<Word>>(l => list1 = (List<Word>)l);
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

        //[Test]
        //public void GenerateCrosswords_WhenNewGenerationPlacedSameAmountOfWords_AddNewGeneration()
        //{
        //    // Arrange
        //    var generationOne = new Generation(_blocks1);

        //    var generationTwo = new Generation(_blocks2);

        //    _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationOne)
        //        .Returns(generationTwo);

        //    // Act
        //    var result = _manager.GenerateCrosswords(_5WordsAllPlaceable, 2, int.MaxValue).ToList();

        //    // Assert
        //    var generations = result;
        //    Assert.AreEqual(2, generations.Count);
        //    Assert.AreEqual(generationOne, generations[0]);
        //    Assert.AreEqual(generationTwo, generations[1]);

        //}

        //[Test]
        //public void GenerateCrosswords_WhenNewGenerationPlacedLessWords_DoNotAddNewGeneration()
        //{
        //    // Arrange
        //    var generationMore = new Generation(_blocks1);

        //    var generationLess = new Generation(_blocks1);

        //    _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationMore)
        //        .Returns(generationLess);

        //    // Act
        //    var result = _manager.GenerateCrosswords(_5WordsAllPlaceable, 2, int.MaxValue).ToList();

        //    // Assert
        //    var generations = result;
        //    Assert.AreEqual(1, generations.Count);
        //    Assert.AreEqual(generationMore, generations.First());

        //}

        //[Test]
        //public void GenerateCrosswords_RemovesDuplicateGenerations()
        //{
        //    // Arrange
        //    var generationOne = new Generation(_blocks1);
        //    var generationTwo = new Generation(_blocks2);
        //    var generationThree = new Generation(_blocks1);
        //    var generationFour = new Generation(_blocks2);

        //    _generatorMock.SetupSequence(g => g.Generate(It.IsAny<List<Word>>())).Returns(generationOne)
        //        .Returns(generationTwo).Returns(generationThree).Returns(generationFour);

        //    // Act
        //    var result = _manager.GenerateCrosswords(_5WordsAllPlaceable, 4, int.MaxValue).ToList();

        //    // Assert
        //    var generations = result;
        //    Assert.AreEqual(2, generations.Count);
        //    Assert.AreEqual(generationOne, generations[0]);
        //    Assert.AreEqual(generationTwo, generations[1]);
        //}

        [Test]
        [Repeat(100)]
        public void GenerateCrosswords_WhenGivenAListOfWordsThatCanAllBePlaced_ShouldGenerateCrosswordWithAllWordsPlaced()
        {
            // Arrange
            var placementFinder = new PlacementFinder();
            var realGenerator = new Generator(placementFinder);
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
            var words = new List<WordCluePair>()
            {
                new("Moose"),
                new("Goose"),
                new("Xyz")
            };

            // Act
            var result = _realManager.GenerateCrosswords(words, timeout: int.MaxValue);

            // Assert
            var unplacedWords = result.First().NumberOfUnplacedWords;
            Assert.AreEqual(1, unplacedWords);
        }

        //[Test]
        //public void GenerateCrosswords_WhenTenCompleteCrosswordsGenerated_ShouldStopGeneratingAndReturn()
        //{
        //    // Arrange
        //    var successfulGeneration = new Generation(_blocks1);

        //    _generatorMock.Setup(g => g.Generate(It.IsAny<IList<Word>>()))
        //        .Returns(successfulGeneration);

        //    // Act
        //    _manager.GenerateCrosswords(_5WordsAllPlaceable);

        //    // Assert
        //    _generatorMock.Verify(g => g.Generate(It.IsAny<List<Word>>()), Times.Exactly(3));
        //}

        [Test]
        public void EfficiencyTest()
        {
            var watch = new Stopwatch();

            var lowest5Size = int.MaxValue;
            var lowest10Size = int.MaxValue;
            var lowest15Size = int.MaxValue;
            var lowest15UnSize = int.MaxValue;

            double averageSizeScore = 0;

            // Arrange
            const int maxAttempts = 100;
            const int maxGenerations = 50;
            const int timesToRepeat = 10;

            double averageSuccess = 0;
            double averageTime = 0;
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

                var result = _realManager.GenerateCrosswords(words, maxAttempts: maxAttempts, maxGenerations: maxGenerations, int.MaxValue, cullIdenticals: false).ToList();

                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
                watch.Reset();
                // If all words placeable, success is all words placed
                // If 3 words unplaceable, success is all but 3 words placed
                var success = wordSelector == 4
                    ? Convert.ToInt32(result.Any(x => x.NumberOfUnplacedWords == 3))
                    : Convert.ToInt32(result.Any(x => x.NumberOfUnplacedWords == 0));
                averageSuccess += success;


                if (result.Any())
                {
                    var blockSize = result.Min(r => r.BlocksSize);
                    switch (wordSelector)
                    {
                        case 1:
                            averageSizeScore += (double)Best5Size / blockSize;
                            lowest5Size = blockSize < lowest5Size ? blockSize : lowest5Size;
                            break;
                        case 2:
                            averageSizeScore += (double)Best10Size / blockSize;
                            lowest10Size = blockSize < lowest10Size ? blockSize : lowest10Size;
                            break;
                        case 3:
                            averageSizeScore += (double)Best15Size / blockSize;
                            lowest15Size = blockSize < lowest15Size ? blockSize : lowest15Size;
                            break;
                        case 4:
                            averageSizeScore += (double)Best15UnSize / blockSize;
                            lowest15UnSize = blockSize < lowest15UnSize ? blockSize : lowest15UnSize;
                            break;

                    }
                }
                wordSelector = wordSelector == 4 ? 1 : wordSelector + 1;
            }

            averageSuccess = (averageSuccess / timesToRepeat) * 100;
            averageSizeScore = (averageSizeScore / timesToRepeat) * 100;
            averageTime /= timesToRepeat;

            // Assert
            TestContext.WriteLine($"Success rate: {averageSuccess}%");
            TestContext.WriteLine($"Average time: {averageTime}ms");
            TestContext.WriteLine();
            TestContext.WriteLine($"Lowest size 5 words: {lowest5Size} ({Best5Size})");
            TestContext.WriteLine($"Lowest size 10 words: {lowest10Size} ({Best10Size})");
            TestContext.WriteLine($"Lowest size 15 words: {lowest15Size} ({Best15Size})");
            TestContext.WriteLine($"Lowest size 15 words with 3 unplaceable: {lowest15UnSize} ({Best15UnSize})");
            TestContext.WriteLine($"Average size score: {averageSizeScore:F}%");


            Assert.True(averageSuccess >= 99.00);
        }

        [Test]
        public void ProgressiveEfficiencyTest()
        {
            var watch = new Stopwatch();
            // Arrange
            const int timesToRepeat = 100;
            const int maxAttempts = 35;

            for (var maxGenerations = 1; maxGenerations <= 35; maxGenerations++)
            {
                double averageSizeScore = 0;
                double averageSuccess = 0;
                double averageTime = 0;
                var wordSelector = 1;

                // Act
                for (var i = 1; i <= timesToRepeat; i++)
                {
                    var words = wordSelector switch
                    {
                        1 => _5WordsAllPlaceable,
                        2 => _10WordsAllPlaceable,
                        3 => _15WordsAllPlaceable,
                        _ => _15Words3Unplaceable,
                    };

                    watch.Start();

                    var result = _realManager.GenerateCrosswords(words, maxAttempts: maxAttempts,
                        maxGenerations: maxGenerations, int.MaxValue, cullIdenticals: false).ToList();

                    watch.Stop();
                    averageTime += watch.ElapsedMilliseconds;
                    watch.Reset();
                    // If all words placeable, success is all words placed
                    // If 3 words unplaceable, success is all but 3 words placed
                    var success = wordSelector == 4
                        ? Convert.ToInt32(result.Any(x => x.NumberOfUnplacedWords == 3))
                        : Convert.ToInt32(result.Any(x => x.NumberOfUnplacedWords == 0));
                    averageSuccess += success;

                    if (result.Any())
                    {
                        var blockSize = result.Min(r => r.BlocksSize);
                        switch (wordSelector)
                        {
                            case 1:
                                averageSizeScore += (double)Best5Size / blockSize;
                                break;
                            case 2:
                                averageSizeScore += (double)Best10Size / blockSize;
                                break;
                            case 3:
                                averageSizeScore += (double)Best15Size / blockSize;
                                break;
                            case 4:
                                averageSizeScore += (double)Best15UnSize / blockSize;
                                break;
                        }
                    }
                    wordSelector = wordSelector == 4 ? 1 : wordSelector + 1;
                }

                averageSuccess = (averageSuccess / timesToRepeat) * 100;
                averageSizeScore = (averageSizeScore / timesToRepeat) * 100;
                averageTime /= timesToRepeat;

                // Assert
                TestContext.WriteLine($"-- MAX Generations: {maxGenerations} --");
                TestContext.WriteLine($"Success rate: {averageSuccess}%");
                TestContext.WriteLine($"Average time: {averageTime}ms");
                TestContext.WriteLine($"Average size score: {averageSizeScore:F}%");
                TestContext.WriteLine();
            }
        }


        private Generation CreateDefaultGeneration()
        {
            return null;
            //return new Generation(_blocks1);

        }

        private void SetUpDefaultGeneration()
        {
            _generatorMock.Setup(g => g.Generate(It.IsAny<List<Word>>())).Returns(CreateDefaultGeneration);
        }

    }
}