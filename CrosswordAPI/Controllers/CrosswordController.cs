using CrosswordGenerator;
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

        [HttpGet(Name = "GenerateCrosswords")]
        public IEnumerable<Generation>? Get(IEnumerable<string> wordStrings)
        {
            return _generationManager.GenerateCrosswords(wordStrings);
        }
    }
}