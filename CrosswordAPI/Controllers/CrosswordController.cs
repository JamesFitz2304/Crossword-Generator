using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Mapper;
using CrosswordGenerator.Models;
using CrosswordGenerator.Models.Puzzle;
using Microsoft.AspNetCore.Mvc;

namespace CrosswordAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrosswordController : ControllerBase
    {
 
        private readonly ILogger<CrosswordController> _logger;
        private readonly IGenerationManager _generationManager;
        private readonly IPuzzleMapper _mapper;

        public CrosswordController(ILogger<CrosswordController> logger, IGenerationManager generationManager, IPuzzleMapper mapper)
        {
            _logger = logger;
            _generationManager = generationManager;
            _mapper = mapper;
        }

        [HttpPost]
        public IEnumerable<Puzzle>? GenerateCrosswords(IList<WordCluePair> wordCluePairs)
        {
            var generations = _generationManager.GenerateCrosswords(wordCluePairs);
            return _mapper.Map(generations, wordCluePairs);
        }
    }
}