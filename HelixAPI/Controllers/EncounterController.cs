using Helix.Api.Base;
using Helix.Core.Features.Encounters.Commands.Models;
using Helix.Core.Features.Encounters.Queries.Models;
using Helix.Service.DTOs.EncounterDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages clinical encounter records in the Helix healthcare system.
    /// An encounter represents a clinical visit or interaction between a patient and a provider.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EncounterController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all encounter records.
        /// </summary>
        /// <returns>A list of all encounter records.</returns>
        /// <response code="200">Returns the list of encounters.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EncounterDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetEncounterListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific encounter record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the encounter.</param>
        /// <returns>The encounter record matching the given ID.</returns>
        /// <response code="200">Returns the encounter record.</response>
        /// <response code="404">Encounter not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetEncounterByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new encounter record.
        /// </summary>
        /// <param name="dto">The encounter data to create.</param>
        /// <returns>The newly created encounter record.</returns>
        /// <response code="201">Encounter created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateEncounterDto dto)
        {
            var command = new CreateEncounterCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing encounter record.
        /// </summary>
        /// <param name="id">The unique identifier of the encounter to update.</param>
        /// <param name="dto">The updated encounter data.</param>
        /// <returns>The updated encounter record.</returns>
        /// <response code="200">Encounter updated successfully.</response>
        /// <response code="404">Encounter not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEncounterDto dto)
        {
            var command = new UpdateEncounterCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes an encounter record.
        /// </summary>
        /// <param name="id">The unique identifier of the encounter to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Encounter deleted successfully.</response>
        /// <response code="404">Encounter not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteEncounterCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
