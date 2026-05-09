using Helix.Api.Base;
using Helix.Core.Features.Consents.Commands.Models;
using Helix.Core.Features.Consents.Queries.Models;
using Helix.Service.DTOs.ConsentDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient consent records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConsentController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all consent records.
        /// </summary>
        /// <returns>A list of all consent records.</returns>
        /// <response code="200">Returns the list of consents.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConsentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetConsentListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific consent record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the consent record.</param>
        /// <returns>The consent record matching the given ID.</returns>
        /// <response code="200">Returns the consent record.</response>
        /// <response code="404">Consent record not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetConsentByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new consent record.
        /// </summary>
        /// <param name="dto">The consent data to create.</param>
        /// <returns>The newly created consent record.</returns>
        /// <response code="201">Consent created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateConsentDto dto)
        {
            var command = new CreateConsentCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing consent record.
        /// </summary>
        /// <param name="id">The unique identifier of the consent to update.</param>
        /// <param name="dto">The updated consent data.</param>
        /// <returns>The updated consent record.</returns>
        /// <response code="200">Consent updated successfully.</response>
        /// <response code="404">Consent not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConsentDto dto)
        {
            var command = new UpdateConsentCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a consent record.
        /// </summary>
        /// <param name="id">The unique identifier of the consent to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Consent deleted successfully.</response>
        /// <response code="404">Consent not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteConsentCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
