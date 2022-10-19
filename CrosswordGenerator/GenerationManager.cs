using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrosswordGenerator.Interfaces;

namespace CrosswordGenerator
{
    public class GenerationManager
    {
        private IGenerator _generator;

        public GenerationManager(IGenerator generator)
        {
            _generator = generator;
        }

        public IEnumerable<Generation> GenerateCrosswords(List<Word> words, int attempts, int timeoutLimit)
        {

            for (var i = 0; i < attempts; i++)
            {
                var shuffledWords = ShuffleWords(words);
                _generator.Generate(shuffledWords);
            }

            return null;
        }

        private IList<Word> ShuffleWords(IEnumerable<Word> words)
        {
            var shuffled = words.ToList();
            var random = new Random();
            var n = shuffled.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                (shuffled[k], shuffled[n]) = (shuffled[n], shuffled[k]);
            }

            return shuffled;
        }
    }
}
