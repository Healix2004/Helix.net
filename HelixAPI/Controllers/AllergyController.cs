using Helix.Api.Base;
using Helix.Core.Features.Allergies.Commands.Models;
using Helix.Core.Features.Allergies.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.AllergyDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AllergyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllergyListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetAllergyByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPost]
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAllergyDto dto)
        {
            var command = new CreateAllergyCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAllergyDto dto)
        {
            var command = new UpdateAllergyCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
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
