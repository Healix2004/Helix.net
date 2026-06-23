using Helix.Api.Base;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RxNavTerminologyController : ControllerBase
    {
        private readonly IRxNavTerminologyService _rxNavService;

        public RxNavTerminologyController(IRxNavTerminologyService rxNavService)
        {
            _rxNavService = rxNavService;
        }

        [HttpGet("rxcui/{drugName}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRxcui(string drugName)
        {
            var rxcui = await _rxNavService.GetRxcuiByDrugNameAsync(drugName);

            if (string.IsNullOrEmpty(rxcui))
                return NotFound($"No RXCUI found for drug: {drugName}");

            return Ok(new { DrugName = drugName, Rxcui = rxcui });
        }

        [HttpGet("medication/{rxcui}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMedicationByRxcui(string rxcui)
        {
            var medication = await _rxNavService.GetFhirMedicationByRxcuiAsync(rxcui);

            if (medication == null)
                return NotFound($"No medication details found for RXCUI: {rxcui}");

            return Ok(medication);
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchMedications([FromQuery] string query)
        {
            var medications = await _rxNavService.SearchFhirMedicationsAsync(query);
            return Ok(medications);
        }

        // NEW: The approximate search endpoint for "Search As You Type" behavior
        [HttpGet("approximate-search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchApproximateMedications([FromQuery] string term, [FromQuery] int maxEntries = 5)
        {
            var medications = await _rxNavService.SearchApproximateMedicationsAsync(term, maxEntries);
            return Ok(medications);
        }
    }
}