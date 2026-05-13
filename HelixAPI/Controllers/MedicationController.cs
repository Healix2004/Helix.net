using Helix.Api.Base;
using Helix.Core.Features.Medications.Commands.Models;
using Helix.Core.Features.Medications.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MedicationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetMedicationListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetMedicationByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPost]
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMedicationDto dto)
        {
            var command = new CreateMedicationCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicationDto dto)
        {
            var command = new UpdateMedicationCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
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
