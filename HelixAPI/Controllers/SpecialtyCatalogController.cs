using Helix.Data.Entities;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialtyCatalogController(ISpecialtyCatalogService specialtyCatalogService) : ControllerBase
    {
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<SpecialtyCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchSpecialties([FromQuery] string query, [FromQuery] int count = 40)
        {
            var matches = await specialtyCatalogService.SearchSpecialtiesAsync(query, count);
            return Ok(matches);
        }

        [HttpGet("top")]
        [ProducesResponseType(typeof(IEnumerable<SpecialtyCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopSpecialties([FromQuery] int count = 10)
        {
            var topSpecialties = await specialtyCatalogService.GetTopSpecialtiesAsync(count);
            return Ok(topSpecialties);
        }
    }
}
