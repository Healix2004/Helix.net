using Helix.Api.Base;
using Helix.Core.Features.Allergies.Commands.Models;
using Helix.Core.Features.Allergies.Queries.Models;
using Helix.Service.DTOs.AllergyDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient allergy records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AllergyController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all allergy records.
        /// </summary>
        /// <returns>A list of all allergy records.</returns>
        /// <response code="200">Returns the list of allergies.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AllergyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllergyListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific allergy record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the allergy record.</param>
        /// <returns>The allergy record matching the given ID.</returns>
        /// <response code="200">Returns the allergy record.</response>
        /// <response code="404">Allergy record not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetAllergyByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new allergy record.
        /// </summary>
        /// <param name="dto">The allergy data to create.</param>
        /// <returns>The newly created allergy record.</returns>
        /// <response code="201">Allergy created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAllergyDto dto)
        {
            var command = new CreateAllergyCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing allergy record.
        /// </summary>
        /// <param name="id">The unique identifier of the allergy to update.</param>
        /// <param name="dto">The updated allergy data.</param>
        /// <returns>The updated allergy record.</returns>
        /// <response code="200">Allergy updated successfully.</response>
        /// <response code="404">Allergy not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAllergyDto dto)
        {
            var command = new UpdateAllergyCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes an allergy record.
        /// </summary>
        /// <param name="id">The unique identifier of the allergy to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Allergy deleted successfully.</response>
        /// <response code="404">Allergy not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteAllergyCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
