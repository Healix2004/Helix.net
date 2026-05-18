using Helix.Api.Base;
using Helix.Core.Bases; // ADDED: For manual Response<T> wrappers
using Helix.Core.Features.Observations.Commands.Models;
using Helix.Core.Features.Observations.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.ObservationDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces; // ADDED: For IPatientService
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages clinical observation records in the HELIX healthcare ecosystem.
    /// Observations include measurements, findings, and assessments such as vital signs and lab values.
    /// </summary>
    [Route("api/observations")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class ObservationController(IMediator mediator, IPatientService patientService) : AppControllerBase
    {
        // ==========================================
        // 1. PATIENT WORKFLOW
        // ==========================================

        [HttpGet("my-observations")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [ProducesResponseType(typeof(IEnumerable<ObservationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyObservations()
        {
            // Safely get the ID from their JWT login token
            Guid patientId = await User.GetPatientIdAsync(patientService);

            // Note: Ensure you have this specific Query defined in your MediatR features!
            var query = new GetObservationListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // 2. DOCTOR / CLINICAL WORKFLOW (Requires Consent to Read)
        // ==========================================

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor,Nurse")] // If you have a Nurse role, they usually need to read vitals too!
        [ProducesResponseType(typeof(IEnumerable<ObservationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientObservations(Guid patientId)
        {
            // THE CONSENT CHECK: Looking for the "Observations" (or "Vitals") scope
            if (!User.HasValidConsent(patientId, "Observations"))
            {
                return Forbid("You do not have active consent to view this patient's clinical observations.");
            }

            var query = new GetObservationListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor,Nurse")] // FIX 2: Only clinicians should record vitals/observations
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateObservationDto dto)
        {
            var command = new CreateObservationCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")] // Doctors update their notes, Admins for system corrections
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateObservationDto dto)
        {
            // FIX 3: Security check to prevent ID spoofing in the JSON body
            if (id != dto.Id)
            {
                var badResponse = new Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "The ID in the URL does not match the ID in the body."
                };
                return NewResult(badResponse);
            }

            var command = new UpdateObservationCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        // ==========================================
        // 3. ADMIN WORKFLOW (System Management)
        // ==========================================

        [HttpGet("all")] // RESTful path
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(IEnumerable<ObservationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetObservationListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        [ProducesResponseType(typeof(ObservationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetObservationByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))] // FIX 4: Deleting clinical records should be strictly controlled
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteObservationCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}