using Helix.Api.Base;
using Helix.Core.Features.Medications.Commands.Models;
using Helix.Core.Features.Medications.Queries.Models;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages medication records prescribed to patients in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all medication records.
        /// </summary>
        /// <returns>A list of all medication records.</returns>
        /// <response code="200">Returns the list of medications.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MedicationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetMedicationListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific medication record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the medication record.</param>
        /// <returns>The medication record matching the given ID.</returns>
        /// <response code="200">Returns the medication record.</response>
        /// <response code="404">Medication not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetMedicationByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new medication record.
        /// </summary>
        /// <param name="dto">The medication data to create.</param>
        /// <returns>The newly created medication record.</returns>
        /// <response code="201">Medication created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMedicationDto dto)
        {
            var command = new CreateMedicationCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing medication record.
        /// </summary>
        /// <param name="id">The unique identifier of the medication to update.</param>
        /// <param name="dto">The updated medication data.</param>
        /// <returns>The updated medication record.</returns>
        /// <response code="200">Medication updated successfully.</response>
        /// <response code="404">Medication not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicationDto dto)
        {
            var command = new UpdateMedicationCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a medication record.
        /// </summary>
        /// <param name="id">The unique identifier of the medication to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Medication deleted successfully.</response>
        /// <response code="404">Medication not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteMedicationCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
