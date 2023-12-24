using CrosswordAPI.Mapper;
using CrosswordAPI.Models;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrosswordAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrosswordController : ControllerBase
    {
 
        private readonly ILogger<CrosswordController> _logger;
        private readonly GenerationManager _generationManager;
        private readonly PuzzleMapper _mapper;

        public CrosswordController(ILogger<CrosswordController> logger, GenerationManager generationManager, PuzzleMapper mapper)
        {
            _logger = logger;
            _generationManager = generationManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<Puzzle>? GenerateCrosswords(IList<WordCluePair> wordCluePairs)
        {
            var generations = _generationManager.GenerateCrosswords(wordCluePairs);
            return _mapper.Map(generations, wordCluePairs);
        }
    }
}