using Helix.Data.Entities;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllergenCatalogController(IAllergenCatalogService allergenCatalogService) : ControllerBase
    {
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<AllergenCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchAllergies([FromQuery] string query, [FromQuery] int count = 40)
        {
            var matches = await allergenCatalogService.SearchAllergiesAsync(query, count);
            return Ok(matches);
        }
        [HttpGet("top")]
        [ProducesResponseType(typeof(IEnumerable<AllergenCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopAllergies([FromQuery] int count = 10)
        {
            var topAllergies = await allergenCatalogService.GetTopAllergiesAsync(count);
            return Ok(topAllergies);
        }
    }
}