using Helix.Api.Base;
using Helix.Core.Features.Drugs.Commands.Models;
using Helix.Core.Features.Drugs.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.DrugDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Provides access to drug data in the Helix healthcare system.
    /// Drug data is sourced from an external drug data service.
    /// Standard CRUD mutations (POST/PUT/DELETE) are not supported by the external data source.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(EnRoles.Doctor))]
    public class DrugController(IMediator mediator) : AppControllerBase
    {

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DrugDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDrugListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DrugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetDrugByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDrugDto dto)
        {
            var command = new CreateDrugCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDrugDto dto)
        {
            var command = new UpdateDrugCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDrugCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }

    public record ServerIpRequest(string Ip);
}
