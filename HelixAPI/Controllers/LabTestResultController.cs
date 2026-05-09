using Helix.Api.Base;
using Helix.Core.Features.LabTestResults.Commands.Models;
using Helix.Core.Features.LabTestResults.Queries.Models;
using Helix.Service.DTOs.LabTestResultDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages laboratory test result records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LabTestResultController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all lab test result records.
        /// </summary>
        /// <returns>A list of all lab test result records.</returns>
        /// <response code="200">Returns the list of lab test results.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LabTestResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetLabTestResultListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific lab test result by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the lab test result.</param>
        /// <returns>The lab test result matching the given ID.</returns>
        /// <response code="200">Returns the lab test result.</response>
        /// <response code="404">Lab test result not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetLabTestResultByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new lab test result record.
        /// </summary>
        /// <param name="dto">The lab test result data to create.</param>
        /// <returns>The newly created lab test result record.</returns>
        /// <response code="201">Lab test result created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLabTestResultDto dto)
        {
            var command = new CreateLabTestResultCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing lab test result record.
        /// </summary>
        /// <param name="id">The unique identifier of the lab test result to update.</param>
        /// <param name="dto">The updated lab test result data.</param>
        /// <returns>The updated lab test result record.</returns>
        /// <response code="200">Lab test result updated successfully.</response>
        /// <response code="404">Lab test result not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLabTestResultDto dto)
        {
            var command = new UpdateLabTestResultCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a lab test result record.
        /// </summary>
        /// <param name="id">The unique identifier of the lab test result to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Lab test result deleted successfully.</response>
        /// <response code="404">Lab test result not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteLabTestResultCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
