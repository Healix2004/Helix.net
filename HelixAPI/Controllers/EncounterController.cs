using Helix.Api.Base;
using Helix.Core.Features.Encounters.Commands.Models;
using Helix.Core.Features.Encounters.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.EncounterDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages clinical encounter records in the Helix healthcare system.
    /// An encounter represents a clinical visit or interaction between a patient and a provider.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(EnRoles.Doctor))]

    public class EncounterController(IMediator mediator) : AppControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EncounterDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetEncounterListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetEncounterByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateEncounterDto dto)
        {
            var command = new CreateEncounterCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEncounterDto dto)
        {
            var command = new UpdateEncounterCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

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
