using Helix.Api.Base;
using Helix.Core.Features.Diagnoses.Commands.Models;
using Helix.Core.Features.Diagnoses.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.DiagnoseDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces; // ADDED: For IPatientService
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient diagnosis records in the HELIX healthcare ecosystem.
    /// </summary>
    [Route("api/diagnoses")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class DiagnoseController(IMediator mediator, IPatientService patientService) : AppControllerBase
    {
        // ==========================================
        // 1. PATIENT WORKFLOW
        // ==========================================

        [HttpGet("my-diagnoses")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [ProducesResponseType(typeof(IEnumerable<DiagnoseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyDiagnoses()
        {
            // Safely get the ID from their JWT login token
            Guid patientId = await User.GetPatientIdAsync(patientService);

            // Note: Ensure you have this specific Query defined in your MediatR features!
            var query = new GetDiagnoseListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // 2. DOCTOR WORKFLOW (Requires Consent)
        // ==========================================

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [ProducesResponseType(typeof(IEnumerable<DiagnoseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientDiagnoses(Guid patientId)
        {
            // THE CONSENT CHECK: Looking for the "Diagnoses" scope
            if (!User.HasValidConsent(patientId, "Diagnoses"))
            {
                return Forbid("You do not have active consent to view this patient's medical diagnoses.");
            }

            var query = new GetDiagnoseListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Doctor))] // FIX 2: Only Doctors should be diagnosing patients
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDiagnoseDto dto)
        {
            // Note: If dto requires a DoctorId, you can set it here using: 
            // dto.DoctorId = await User.GetDoctorIdAsync(doctorService);

            var command = new CreateDiagnoseCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(EnRoles.Admin)},{nameof(EnRoles.Doctor)}")]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDiagnoseDto dto)
        {
            // FIX 3: Security check to prevent ID spoofing in the JSON body
            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var command = new UpdateDiagnoseCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        // ==========================================
        // 3. ADMIN WORKFLOW (System Management)
        // ==========================================

        [HttpGet("all")] // RESTful path
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(IEnumerable<DiagnoseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDiagnoseListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(EnRoles.Admin)},{nameof(EnRoles.Doctor)}")]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetDiagnoseByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))] // Deleting medical history should be strictly controlled
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteDiagnoseCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}