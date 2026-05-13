using Helix.Api.Base;
using Helix.Core.Features.Consents.Commands.Models;
using Helix.Core.Features.Consents.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.ConsentDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConsentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetConsentListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetConsentByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateConsentDto dto)
        {
            var command = new CreateConsentCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConsentDto dto)
        {
            var command = new UpdateConsentCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Admin))]
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
