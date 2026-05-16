using Helix.Api.Base;
using Helix.Core.Features.LabTestResults.Commands.Models;
using Helix.Core.Features.LabTestResults.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using Helix.Service.Services.PatientService;
using Hl7.Fhir.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages laboratory test result records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LabTestResultController(IMediator mediator,IPatientService patientService) : AppControllerBase
    {
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [HttpGet]
        public async Task<IActionResult> GetPatientLabResult()
        {
            var query = new GetLabTestResultListForPatientQuery(GetPatientId());
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpGet("Get-All")]
        [ProducesResponseType(typeof(IEnumerable<LabTestResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetLabTestResultListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }
        [Authorize(Roles = nameof(EnRoles.Admin))]

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetLabTestResultByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLabTestResultDto dto)
        {
            var command = new CreateLabTestResultCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLabTestResultDto dto)
        {
            var command = new UpdateLabTestResultCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteLabTestResultCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        #region Help
        private Guid GetPatientId()
        {
            // 1. Extract the user ID from the JWT Claims
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // 2. get the patient record based on the user ID
            var patient = patientService.GetPatientByUserIdAsync(userIdString).Result;
            if (patient != null)
            {
                return patient.Id;
            }

            throw new UnauthorizedAccessException("Invalid patient ID.");
        }

        #endregion
    }
}
