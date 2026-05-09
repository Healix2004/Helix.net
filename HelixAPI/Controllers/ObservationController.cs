using Helix.Api.Base;
using Helix.Core.Features.Observations.Commands.Models;
using Helix.Core.Features.Observations.Queries.Models;
using Helix.Service.DTOs.ObservationDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages clinical observation records in the Helix healthcare system.
    /// Observations include measurements, findings, and assessments such as vital signs and lab values.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all observation records.
        /// </summary>
        /// <returns>A list of all clinical observation records.</returns>
        /// <response code="200">Returns the list of observations.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObservationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetObservationListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific observation record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the observation record.</param>
        /// <returns>The observation record matching the given ID.</returns>
        /// <response code="200">Returns the observation record.</response>
        /// <response code="404">Observation not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetObservationByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new clinical observation record.
        /// </summary>
        /// <param name="dto">The observation data to create.</param>
        /// <returns>The newly created observation record.</returns>
        /// <response code="201">Observation created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateObservationDto dto)
        {
            var command = new CreateObservationCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing observation record.
        /// </summary>
        /// <param name="id">The unique identifier of the observation to update.</param>
        /// <param name="dto">The updated observation data.</param>
        /// <returns>The updated observation record.</returns>
        /// <response code="200">Observation updated successfully.</response>
        /// <response code="404">Observation not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateObservationDto dto)
        {
            var command = new UpdateObservationCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes an observation record.
        /// </summary>
        /// <param name="id">The unique identifier of the observation to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Observation deleted successfully.</response>
        /// <response code="404">Observation not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteObservationCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
