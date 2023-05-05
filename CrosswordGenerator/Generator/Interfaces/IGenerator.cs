using System.Collections.Generic;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.Generator.Interfaces
{
    public interface IGenerator
    {
        Generation Generate(IList<Word> words);
    }
}