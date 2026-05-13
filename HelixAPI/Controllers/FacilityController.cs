using Helix.Api.Base;
using Helix.Core.Features.Facilities.Commands.Models;
using Helix.Core.Features.Facilities.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.FacilitieDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages healthcare facility records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(EnRoles.Admin))]

    public class FacilityController(IMediator mediator) : AppControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FacilitieDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetFacilityListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetFacilityByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFacilitieDto dto)
        {
            var command = new CreateFacilityCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFacilitieDto dto)
        {
            var command = new UpdateFacilityCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteFacilityCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
