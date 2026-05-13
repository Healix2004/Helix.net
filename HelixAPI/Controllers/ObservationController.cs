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

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObservationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetObservationListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetObservationByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateObservationDto dto)
        {
            var command = new CreateObservationCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateObservationDto dto)
        {
            var command = new UpdateObservationCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

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
