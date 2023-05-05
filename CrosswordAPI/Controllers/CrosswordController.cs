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

        public CrosswordController(ILogger<CrosswordController> logger, GenerationManager generationManager)
        {
            _logger = logger;
            _generationManager = generationManager;
        }

        [HttpGet]
        public IEnumerable<Generation>? GenerateCrosswords(IList<WordCluePair> wordCluePairs)
        {
            return _generationManager.GenerateCrosswords(wordCluePairs);
        }
    }
}