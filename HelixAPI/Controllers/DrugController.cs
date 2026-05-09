using Helix.Api.Base;
using Helix.Core.Features.Drugs.Commands.Models;
using Helix.Core.Features.Drugs.Queries.Models;
using Helix.Service.DTOs.DrugDTOs;
using MediatR;
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
    public class DrugController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all available drugs from the external drug data service.
        /// </summary>
        /// <returns>A list of all available drugs.</returns>
        /// <response code="200">Returns the list of drugs.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DrugDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDrugListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific drug by its ID from the external drug data service.
        /// </summary>
        /// <param name="id">The unique drug identifier.</param>
        /// <returns>The drug matching the given ID.</returns>
        /// <response code="200">Returns the drug.</response>
        /// <response code="404">Drug not found in the external drug data service.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DrugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetDrugByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Not supported. Drug creation is managed by the external drug data service.
        /// </summary>
        /// <param name="dto">Drug creation data (stub — not processed by the external service).</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Drug creation is not supported via this endpoint.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDrugDto dto)
        {
            var command = new CreateDrugCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Not supported. Drug updates are managed by the external drug data service.
        /// </summary>
        /// <param name="id">The drug ID (stub — not processed by the external service).</param>
        /// <param name="dto">Drug update data (stub).</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Drug update is not supported via this endpoint.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDrugDto dto)
        {
            var command = new UpdateDrugCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Not supported. Drug deletion is managed by the external drug data service.
        /// </summary>
        /// <param name="id">The drug ID (stub — not processed by the external service).</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Drug deletion is not supported via this endpoint.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDrugCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }

    /// <summary>
    /// Request body for setting the external drug data server IP address.
    /// </summary>
    public record ServerIpRequest(string Ip);
}
