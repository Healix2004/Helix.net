using Helix.Data.Entities;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationCatalogController(IMedicationCatalogService medicationCatalogService) : ControllerBase
    {
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<MedicationCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchMedications([FromQuery] string query, [FromQuery] int count = 40)
        {
            var matches = await medicationCatalogService.SearchMedicationsAsync(query, count);
            return Ok(matches);
        }
        [HttpGet("top")]
        [ProducesResponseType(typeof(IEnumerable<MedicationCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopMedications([FromQuery] int count = 10)
        {
            var topMedications = await medicationCatalogService.GetTopMedicationsAsync(count);
            return Ok(topMedications);
        }
    }
}