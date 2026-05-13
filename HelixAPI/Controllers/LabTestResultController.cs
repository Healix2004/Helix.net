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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetLabTestResultByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLabTestResultDto dto)
        {
            var command = new CreateLabTestResultCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLabTestResultDto dto)
        {
            var command = new UpdateLabTestResultCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

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
