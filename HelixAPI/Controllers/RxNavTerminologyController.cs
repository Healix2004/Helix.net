using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helix.Api.Base;
using Helix.Core.DTOs.Terminology;
using Helix.Data.Enums;
using Helix.Service.Services.RxNavTerminology;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hl7.Fhir.Model;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Controller for managing drug terminology using RxNav and FHIR standards.
    /// </summary>
    /// <param name="rxNavService">Service for interacting with RxNav terminology.</param>
    /// <param name="medicationSorter">Sorter for FHIR Medication resources.</param>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(EnRoles.Doctor))]
    public class RxNavTerminologyController(RxNavTerminologyService rxNavService) : AppControllerBase
    {
        private readonly RxNavTerminologyService _rxNavService = rxNavService;

        /// <summary>
        /// Searches for medications by name and optionally sorts the results using FHIR _sort parameter.
        /// </summary>
        /// <param name="query">The drug name or search term.</param>
        /// <param name="_sort">FHIR _sort parameter for ordering results (e.g., "code", "-status").</param>
        /// <returns>A list of FHIR Medication resources.</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Medication>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SearchMedications([FromQuery] string query, [FromQuery(Name = "_sort")] string? _sort = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            // Retrieve medications from the RxNav service
            var medications = await _rxNavService.SearchFhirMedicationsAsync(query);

            // Apply FHIR-compliant sorting if the _sort parameter is provided
            if (!string.IsNullOrWhiteSpace(_sort))
            {
            }

            return Ok(medications);
        }

        /// <summary>
        /// Retrieves a specific FHIR Medication resource by its RxCUI.
        /// </summary>
        /// <param name="rxcui">The RxNorm Concept Unique Identifier.</param>
        /// <returns>A FHIR Medication resource.</returns>
        [HttpGet("{rxcui}")]
        [ProducesResponseType(typeof(Medication), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMedicationByRxcui(string rxcui)
        {
            if (string.IsNullOrWhiteSpace(rxcui))
            {
                return BadRequest("RxCUI cannot be empty.");
            }

            // Retrieve the specific medication by RxCUI
            var medication = await _rxNavService.GetFhirMedicationByRxcuiAsync(rxcui);

            if (medication == null)
            {
                return NotFound($"Medication with RxCUI {rxcui} not found.");
            }

            return Ok(medication);
        }
    }
}
