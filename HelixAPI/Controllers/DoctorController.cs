using Helix.Api.Base;
using Helix.Core.Features.Doctors.Commands.Models;
using Helix.Core.Features.Doctors.Queries.Models;
using Helix.Service.DTOs.DoctorDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages doctor records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all doctors.
        /// </summary>
        /// <returns>A list of all doctor records.</returns>
        /// <response code="200">Returns the list of doctors.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DoctorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDoctorListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific doctor by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the doctor.</param>
        /// <returns>The doctor record matching the given ID.</returns>
        /// <response code="200">Returns the doctor record.</response>
        /// <response code="404">Doctor not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetDoctorByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new doctor record.
        /// </summary>
        /// <param name="dto">The doctor data to create.</param>
        /// <returns>The newly created doctor record.</returns>
        /// <response code="201">Doctor created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
        {
            var command = new CreateDoctorCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing doctor record.
        /// </summary>
        /// <param name="id">The unique identifier of the doctor to update.</param>
        /// <param name="dto">The updated doctor data.</param>
        /// <returns>The updated doctor record.</returns>
        /// <response code="200">Doctor updated successfully.</response>
        /// <response code="404">Doctor not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDto dto)
        {
            var command = new UpdateDoctorCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a doctor record.
        /// </summary>
        /// <param name="id">The unique identifier of the doctor to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Doctor deleted successfully.</response>
        /// <response code="404">Doctor not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDoctorCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
