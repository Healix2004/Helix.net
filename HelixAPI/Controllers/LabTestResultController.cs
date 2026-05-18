using Helix.Api.Base;
using Helix.Core.Features.LabTestResults.Commands.Models;
using Helix.Core.Features.LabTestResults.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/lab-results")] // FIX 1: Explicit RESTful routing instead of [controller]
    [ApiController]
    public class LabTestResultController(IMediator mediator, IPatientService patientService) : AppControllerBase
    {
        // ==========================================================
        // 1. FOR THE PATIENT (No Consent Needed)
        // ==========================================================
        [HttpGet("my-labs")] // FIX 2: Explicit path to prevent route collisions
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> GetPatientLabResult()
        {
            // Safely extracting the patient ID without blocking threads!
            var patientId = await User.GetPatientIdAsync(patientService);

            var query = new GetLabTestResultListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================================
        // 2. FOR THE DOCTOR (Requires the 2-Hour Consent Token)
        // ==========================================================
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetPatientLabRecords(Guid patientId)
        {
            // THE CONSENT CHECK: Looking for the "Labs" scope we defined earlier
            if (!User.HasValidConsent(patientId, "Labs"))
            {
                var forbiddenResponse = new Helix.Core.Bases.Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    Message = "You do not have active consent to view this patient's Lab records."
                };
                return NewResult(forbiddenResponse);
            }

            // Note: You may need to create this specific Query in your MediatR Features!
            var query = new GetLabTestResultListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // ADMIN & LAB TECH WORKFLOW (Management)
        // ==========================================

        [HttpGet("all")] // RESTful standard (changed from "Get-All")
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(IEnumerable<LabTestResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetLabTestResultListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Admin")] // Allowed Doctors to look up specific IDs (if they know them)
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetLabTestResultByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // CRITICAL FIX 3: Locked down the Create endpoint
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLabTestResultDto dto)
        {
            var command = new CreateLabTestResultCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(LabTestResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLabTestResultDto dto)
        {
            var command = new UpdateLabTestResultCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteLabTestResultCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}