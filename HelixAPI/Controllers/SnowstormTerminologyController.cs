using System.Collections.Generic;
using System.Threading.Tasks;
using Helix.Api.Base;
using Helix.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hl7.Fhir.Model;
using Helix.Service.Interfaces;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Controller for managing SNOMED CT terminology using Snowstorm FHIR API.
    /// </summary>
    /// <param name="snowstormService">Service for interacting with Snowstorm terminology.</param>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(EnRoles.Doctor))]
    public class SnowstormTerminologyController(ISnowstormTerminologyService snowstormService) : AppControllerBase
    {
        private readonly ISnowstormTerminologyService _snowstormService = snowstormService;

        /// <summary>
        /// Looks up a specific SNOMED CT code and returns its details as a FHIR CodeSystem.ConceptDefinition.
        /// </summary>
        /// <param name="snomedCode">The SNOMED CT code to look up (e.g., "22298006").</param>
        /// <returns>A FHIR CodeSystem.ConceptDefinition or NotFound if not found.</returns>
        [HttpGet("lookup/{snomedCode}")]
        [ProducesResponseType(typeof(CodeSystem.ConceptDefinitionComponent), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LookupSnomedCode(string snomedCode)
        {
            if (string.IsNullOrWhiteSpace(snomedCode))
            {
                return BadRequest("SNOMED CT code cannot be empty.");
            }

            var concept = await _snowstormService.LookupSnomedCodeAsync(snomedCode);

            if (concept == null)
            {
                return NotFound($"SNOMED CT code {snomedCode} not found.");
            }

            return Ok(concept);
        }

        /// <summary>
        /// Searches for SNOMED CT codes by a search term and returns them as FHIR CodeableConcepts.
        /// </summary>
        /// <param name="searchTerm">The term to search for (e.g., "diabetes").</param>
        /// <returns>A list of FHIR CodeableConcepts.</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CodeableConcept>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SearchSnomedCodes([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var codes = await _snowstormService.SearchSnomedCodesAsync(searchTerm);

            return Ok(codes);
        }

        /// <summary>
        /// Validates if a given SNOMED CT code exists and is active.
        /// </summary>
        /// <param name="snomedCode">The SNOMED CT code to validate.</param>
        /// <returns>True if the code is valid, false otherwise.</returns>
        [HttpGet("validate/{snomedCode}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ValidateSnomedCode(string snomedCode)
        {
            if (string.IsNullOrWhiteSpace(snomedCode))
            {
                return BadRequest("SNOMED CT code cannot be empty.");
            }

            var isValid = await _snowstormService.ValidateSnomedCodeAsync(snomedCode);

            return Ok(isValid);
        }

        /// <summary>
        /// Creates a basic FHIR CodeableConcept for a given SNOMED CT code and display text.
        /// </summary>
        /// <param name="snomedCode">The SNOMED CT code.</param>
        /// <param name="display">The display text for the SNOMED CT code.</param>
        /// <returns>A FHIR CodeableConcept.</returns>
        [HttpGet("codeableconcept")]
        [ProducesResponseType(typeof(CodeableConcept), 200)]
        [ProducesResponseType(400)]
        public IActionResult CreateSnomedCodeableConcept([FromQuery] string snomedCode, [FromQuery] string display)
        {
            if (string.IsNullOrWhiteSpace(snomedCode) || string.IsNullOrWhiteSpace(display))
            {
                return BadRequest("SNOMED CT code and display cannot be empty.");
            }

            var codeableConcept = _snowstormService.CreateSnomedCodeableConcept(snomedCode, display);
            return Ok(codeableConcept);
        }
    }
}
