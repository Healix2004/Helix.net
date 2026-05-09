using Helix.Api.Base;
using Helix.Core.Features.Diagnoses.Commands.Models;
using Helix.Core.Features.Diagnoses.Queries.Models;
using Helix.Service.DTOs.DiagnoseDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient diagnosis records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnoseController(IMediator mediator) : AppControllerBase
    {
        /// <summary>
        /// Retrieves all diagnosis records.
        /// </summary>
        /// <returns>A list of all diagnosis records.</returns>
        /// <response code="200">Returns the list of diagnoses.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DiagnoseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDiagnoseListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Retrieves a specific diagnosis record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the diagnosis record.</param>
        /// <returns>The diagnosis record matching the given ID.</returns>
        /// <response code="200">Returns the diagnosis record.</response>
        /// <response code="404">Diagnosis record not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetDiagnoseByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        /// <summary>
        /// Creates a new diagnosis record.
        /// </summary>
        /// <param name="dto">The diagnosis data to create.</param>
        /// <returns>The newly created diagnosis record.</returns>
        /// <response code="201">Diagnosis created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDiagnoseDto dto)
        {
            var command = new CreateDiagnoseCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Updates an existing diagnosis record.
        /// </summary>
        /// <param name="id">The unique identifier of the diagnosis to update.</param>
        /// <param name="dto">The updated diagnosis data.</param>
        /// <returns>The updated diagnosis record.</returns>
        /// <response code="200">Diagnosis updated successfully.</response>
        /// <response code="404">Diagnosis not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiagnoseDto dto)
        {
            var command = new UpdateDiagnoseCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        /// <summary>
        /// Deletes a diagnosis record.
        /// </summary>
        /// <param name="id">The unique identifier of the diagnosis to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Diagnosis deleted successfully.</response>
        /// <response code="404">Diagnosis not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDiagnoseCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
