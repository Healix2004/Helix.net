using Helix.Api.Base;
using Helix.Data.Enums;
using Helix.Service.DTOs.Terminology;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(EnRoles.Doctor))]
    public class TerminologyController(ITerminologyService terminologyService) : AppControllerBase
    {
        private const string DefaultSystem = "http://snomed.info/sct";

        /// <summary>
        /// Search for codes by text filter
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<CodingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string? text, [FromQuery] string? system)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Parameter 'text' is required and cannot be empty.");
            }

            var systemUri = string.IsNullOrWhiteSpace(system) ? DefaultSystem : system;
            var results = await terminologyService.LookupCodesAsync(text, systemUri);

            // Map service DTOs to Core DTOs for API contract
            var response = results.Select(c => new CodingDto
            {
                System = c.System,
                Code = c.Code,
                Display = c.Display
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Validate that a code exists in the given system
        /// </summary>
        [HttpGet("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Validate([FromQuery] string? code, [FromQuery] string? system)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Parameter 'code' is required and cannot be empty.");
            }

            var systemUri = string.IsNullOrWhiteSpace(system) ? DefaultSystem : system;
            var isValid = await terminologyService.ValidateCodeAsync(systemUri, code);
            return Ok(new { isValid });
        }

        /// <summary>
        /// Get the display name for a code
        /// </summary>
        [HttpGet("display")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDisplay([FromQuery] string? code, [FromQuery] string? system)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Parameter 'code' is required and cannot be empty.");
            }

            var systemUri = string.IsNullOrWhiteSpace(system) ? DefaultSystem : system;
            var display = await terminologyService.GetDisplayForCodeAsync(systemUri, code);

            if (display == null)
            {
                return NotFound();
            }

            return Ok(new { display });
        }
    }
}
