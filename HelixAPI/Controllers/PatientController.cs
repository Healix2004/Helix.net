using Helix.Api.Base;
using Helix.Core.Features.Patients.Commands.Models;
using Helix.Core.Features.Patients.Queries.Models;
using Helix.Service.DTOs.PatientDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all patients.
        /// </summary>
        /// <returns>A list of all patient records.</returns>
        /// <response code="200">Returns the list of patients.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetPatientListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific patient by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the patient.</param>
        /// <returns>The patient record matching the given ID.</returns>
        /// <response code="200">Returns the patient record.</response>
        /// <response code="404">Patient not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetPatientByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new patient record.
        /// </summary>
        /// <param name="dto">The patient data to create.</param>
        /// <returns>The newly created patient record.</returns>
        /// <response code="201">Patient created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
        {
            var command = new CreatePatientCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing patient record.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to update.</param>
        /// <param name="dto">The updated patient data.</param>
        /// <returns>The updated patient record.</returns>
        /// <response code="200">Patient updated successfully.</response>
        /// <response code="404">Patient not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto dto)
        {
            var command = new UpdatePatientCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a patient record.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Patient deleted successfully.</response>
        /// <response code="404">Patient not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeletePatientCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
