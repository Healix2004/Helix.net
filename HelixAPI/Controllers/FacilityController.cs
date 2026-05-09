using Helix.Api.Base;
using Helix.Core.Features.Facilities.Commands.Models;
using Helix.Core.Features.Facilities.Queries.Models;
using Helix.Service.DTOs.FacilitieDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages healthcare facility records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all facility records.
        /// </summary>
        /// <returns>A list of all healthcare facility records.</returns>
        /// <response code="200">Returns the list of facilities.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FacilitieDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetFacilityListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific facility by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the facility.</param>
        /// <returns>The facility record matching the given ID.</returns>
        /// <response code="200">Returns the facility record.</response>
        /// <response code="404">Facility not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetFacilityByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new facility record.
        /// </summary>
        /// <param name="dto">The facility data to create.</param>
        /// <returns>The newly created facility record.</returns>
        /// <response code="201">Facility created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFacilitieDto dto)
        {
            var command = new CreateFacilityCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing facility record.
        /// </summary>
        /// <param name="id">The unique identifier of the facility to update.</param>
        /// <param name="dto">The updated facility data.</param>
        /// <returns>The updated facility record.</returns>
        /// <response code="200">Facility updated successfully.</response>
        /// <response code="404">Facility not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFacilitieDto dto)
        {
            var command = new UpdateFacilityCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a facility record.
        /// </summary>
        /// <param name="id">The unique identifier of the facility to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Facility deleted successfully.</response>
        /// <response code="404">Facility not found.</response>
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
