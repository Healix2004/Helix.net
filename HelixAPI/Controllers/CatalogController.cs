using Helix.Service.DTOs.CatalogDTOs;
using Helix.Service.Interfaces; // ADDED: Need this to inject IPatientService
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController(IMedicalConceptCatalogService catalogService) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ConceptSearchDto>>> SearchConcepts([FromQuery] string searchTerm,[FromQuery] bool? isRadiology = null)
        {
            // 1. Validate the input to protect your database from heavy wildcard queries
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return BadRequest(new { message = "Search term must be at least 2 characters long." });
            }

            // 2. Fetch the data from the service
            var results = await catalogService.SearchConceptsAsync(searchTerm, isRadiology);

            // 3. Return a clean 200 OK response with the JSON array
            return Ok(results);
        }
    }
}