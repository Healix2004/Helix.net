using Helix.Data.Entities;
using Helix.Service.Interfaces; // ADDED: Need this to inject IPatientService
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [ApiController]
        [Route("api/[controller]")]
        public class ChronicDiseaseCatalogController : ControllerBase
        {
            private readonly IChronicDiseaseCatalogService _chronicDiseaseCatalogService;

            public ChronicDiseaseCatalogController(IChronicDiseaseCatalogService chronicDiseaseCatalogService)
            {
                _chronicDiseaseCatalogService = chronicDiseaseCatalogService;
            }

            [HttpGet("search")]
            [ProducesResponseType(typeof(IEnumerable<ChronicDiseaseCatalog>), StatusCodes.Status200OK)]
            public async Task<IActionResult> SearchChronicDiseases([FromQuery] string query, [FromQuery] int count = 40)
            {
                var matches = await _chronicDiseaseCatalogService.SearchChronicDiseasesAsync(query, count);
                return Ok(matches);
            }

            [HttpGet("top")]
            [ProducesResponseType(typeof(IEnumerable<ChronicDiseaseCatalog>), StatusCodes.Status200OK)]
            public async Task<IActionResult> GetTopChronicDiseases([FromQuery] int count = 10)
            {
                var topDiseases = await _chronicDiseaseCatalogService.GetTopChronicDiseasesAsync(count);
                return Ok(topDiseases);
            }
        }
    
}