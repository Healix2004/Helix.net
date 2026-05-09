using Helix.Api.Base;
using Helix.Core.Features.TerminologyCodeLookups.Commands.Models;
using Helix.Core.Features.TerminologyCodeLookups.Queries.Models;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages terminology code lookup records in the Helix healthcare system.
    /// Terminology codes map clinical concepts to standardized coding systems (e.g., SNOMED CT, LOINC, ICD-10).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TerminologyCodeLookupController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all terminology code lookup records.
        /// </summary>
        /// <returns>A list of all terminology code lookups.</returns>
        /// <response code="200">Returns the list of terminology code lookups.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TerminologyCodeLookupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetTerminologyCodeLookupListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific terminology code lookup by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the terminology code lookup.</param>
        /// <returns>The terminology code lookup matching the given ID.</returns>
        /// <response code="200">Returns the terminology code lookup.</response>
        /// <response code="404">Terminology code lookup not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TerminologyCodeLookupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetTerminologyCodeLookupByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new terminology code lookup entry.
        /// </summary>
        /// <param name="dto">The terminology code lookup data to create.</param>
        /// <returns>The newly created terminology code lookup.</returns>
        /// <response code="201">Terminology code lookup created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TerminologyCodeLookupDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTerminologyCodeLookupDto dto)
        {
            var command = new CreateTerminologyCodeLookupCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing terminology code lookup entry.
        /// </summary>
        /// <param name="id">The unique identifier of the terminology code lookup to update.</param>
        /// <param name="dto">The updated terminology code lookup data.</param>
        /// <returns>The updated terminology code lookup.</returns>
        /// <response code="200">Terminology code lookup updated successfully.</response>
        /// <response code="404">Terminology code lookup not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TerminologyCodeLookupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTerminologyCodeLookupDto dto)
        {
            var command = new UpdateTerminologyCodeLookupCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a terminology code lookup entry.
        /// </summary>
        /// <param name="id">The unique identifier of the terminology code lookup to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Terminology code lookup deleted successfully.</response>
        /// <response code="404">Terminology code lookup not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteTerminologyCodeLookupCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
