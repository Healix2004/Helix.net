using Helix.Data.Entities;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcedureCatalogController : ControllerBase
    {
        private readonly IProcedureCatalogService _procedureService;

        public ProcedureCatalogController(IProcedureCatalogService procedureService)
        {
            _procedureService = procedureService;
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ProcedureCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchProcedures([FromQuery] string query, [FromQuery] int count = 40)
        {
            var matches = await _procedureService.SearchProceduresAsync(query, count);
            return Ok(matches);
        }

        [HttpGet("top")]
        [ProducesResponseType(typeof(IEnumerable<ProcedureCatalog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopProcedures([FromQuery] int count = 10)
        {
            var topProcedures = await _procedureService.GetTopProceduresAsync(count);
            return Ok(topProcedures);
        }
    }
}