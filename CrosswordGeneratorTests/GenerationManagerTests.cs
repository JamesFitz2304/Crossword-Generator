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

        [SetUp]
        public void Setup()
        {
            _generatorMock = new Mock<IGenerator>();

            _manager = new GenerationManager(_generatorMock.Object);
        }

        [Test]
        public void GenerationManager_ShufflesWordsWithEachGeneration()
        {
            // Arrange
            var sequence = new MockSequence();
            IList<Word> list1 = new List<Word>();
            IList<Word> list2 = new List<Word>();

            _generatorMock.InSequence(sequence).Setup(g => g.Generate(It.IsAny<IList<Word>>()))
                .Callback<IList<Word>>(l => list1 = l);
            _generatorMock.InSequence(sequence).Setup(g => g.Generate(It.IsAny<IList<Word>>()))
                .Callback<IList<Word>>(l => list2 = l);

            // Act
            _manager.GenerateCrosswords(_wordsAllPlaceable, 2, 0);

            // Assert
            CollectionAssert.AreNotEqual(_wordsAllPlaceable, list1);
            CollectionAssert.AreNotEqual(_wordsAllPlaceable, list2);
            CollectionAssert.AreNotEqual(list1, list2);
        }
    }
}