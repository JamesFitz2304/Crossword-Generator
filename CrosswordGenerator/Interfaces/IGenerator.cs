using System.Collections.Generic;

namespace CrosswordGenerator.Interfaces
{
    public interface IGenerator
    {
        Generation Generate(IList<Word> words);
    }
}